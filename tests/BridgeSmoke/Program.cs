using System;
using Verse;
using Uillan.Aquacultura;
internal static class Program
{
    static void Check(bool ok, string reason) { if (!ok) throw new Exception(reason); }
    static CompTank NewTank()
    {
        return new CompTank { props = new CompProperties_Tank { capacity = 8 } };
    }
    static void Tick(CompTank tank, int ticks)
    {
        Check(ticks % 250 == 0, "Tick count must be multiple of 250");
        for (int i = 0; i < ticks / 250; i++) tank.CompTickRare();
    }
    static CompTank Reload(CompTank tank)
    {
        Scribe.Save();
        tank.PostExposeData();
        var newTank = NewTank();
        Scribe.Load();
        newTank.PostExposeData();
        Scribe.Finish();
        newTank.PostExposeData();
        Scribe.Stop();
        return newTank;
    }
    static void Main()
    {
        var noGizmos = NewTank();
        Check(new System.Collections.Generic.List<Gizmo>(noGizmos.CompGetGizmosExtra()).Count == 0,
            "Free resource actions exposed to normal players");
        Prefs.DevMode = true;
        var devGizmos = new System.Collections.Generic.List<Gizmo>(noGizmos.CompGetGizmosExtra());
        Check(devGizmos.Count == 3, "Expected three developer-only actions");
        ((Command_Action)devGizmos[0]).action();
        ((Command_Action)devGizmos[1]).action();
        ((Command_Action)devGizmos[2]).action();
        Check(noGizmos.TotalEggs == 6 && noGizmos.FeedAvailable == 20f && noGizmos.DaysSimulated == 1,
            "Developer tests did not affect tank state");
        Prefs.DevMode = false;
        // Physical input contracts: no logical state may change before inventory
        // consumption is confirmed. Callbacks here are test doubles, not game jobs.
        var inputs = NewTank();
        int consumedRoe = 0;
        Func<int, bool> roeConsumer = count => { consumedRoe += count; return true; };
        Check(!inputs.TryStockEggs("tilapia", 6, null), "Stocked eggs without consumer");
        Check(!inputs.TryStockEggs("unknown", 6, roeConsumer), "Accepted unknown species");
        Check(!inputs.TryStockEggs("tilapia", 81, roeConsumer), "Consumed eggs above tank capacity");
        Check(!inputs.TryStockEggs("tilapia", 6, _ => false), "Stocked eggs despite failed consumption");
        Check(inputs.TotalEggs == 0 && consumedRoe == 0, "Invalid stocking mutated state or inventory");
        Check(inputs.TryStockEggs("tilapia", 6, roeConsumer), "Valid consumed roe rejected");
        Check(consumedRoe == 6 && inputs.TotalEggs == 6, "Stocking count diverged from consumed items");
        Check(!inputs.TryStockEggs("carp", 1, roeConsumer), "Monoculture allowed extra species");
        Check(!inputs.TryStockEggs("tilapia", 75, roeConsumer), "Accepted roe above remaining capacity");
        Check(!inputs.TryStockEggs("tilapia", 1, _ => false), "Failed restock changed tank");
        Check(consumedRoe == 6 && inputs.TotalEggs == 6, "Rejected restock consumed inventory");
        int consumedFeed = 0;
        Func<int, bool> feedConsumer = count => { consumedFeed += count; return true; };
        Check(!inputs.TryAddFeed(1, null), "Added feed without consumer");
        Check(!inputs.TryAddFeed(0, feedConsumer), "Accepted zero feed");
        Check(!inputs.TryAddFeed(100001, feedConsumer), "Consumed feed above storage cap");
        Check(!inputs.TryAddFeed(4, _ => false), "Added feed despite failed consumption");
        Check(inputs.FeedAvailable == 0f && consumedFeed == 0, "Invalid feed changed state");
        Check(inputs.TryAddFeed(10, feedConsumer), "Valid feed rejected");
        Check(!inputs.TryAddFeed(99991, feedConsumer), "Consumed feed beyond remaining capacity");
        Check(inputs.TryAddFeed(99990, feedConsumer), "Failed to fill feed storage exactly");
        Check(!inputs.TryAddFeed(1, feedConsumer), "Overfilled feed storage");
        Check(inputs.FeedAvailable == 100000f && consumedFeed == 100000,
            "Feed units diverged from consumed items");
        var loadedInputs = Reload(inputs);
        Check(loadedInputs.TotalEggs == 6 && loadedInputs.FeedAvailable == 100000f,
            "Committed physical inputs not preserved after save/load");
        var initial = NewTank();
        Check(!initial.TryStockEggs("unknown", 1, _ => true), "Accepted unknown fish");
        Check(!initial.TryStockEggs("tilapia", 81, _ => true), "Exceeded egg capacity");
        Check(initial.TryStockEggs("tilapia", 6, _ => true), "Unable to stock valid roe");
        Check(!initial.TryStockEggs("carp", 5, _ => true), "Monoculture constraint ignored");
        Check(initial.TryAddFeed(1000, _ => true), "Unable to supply feed for simulation");
        Tick(initial, 59999 / 250 * 250);
        Check(initial.DaysSimulated == 0, "Advanced too soon");
        Tick(initial, 250);
        Check(initial.DaysSimulated == 1, "Did not advance on day boundary");
        Check(initial.TotalEggs == 6, "Egg incubation too fast");
        var resumed = Reload(initial);
        Check(resumed.DaysSimulated == 1, "Day count lost on save");
        Check(resumed.TotalEggs == 6, "Egg count lost on save");
        Check(resumed.FeedAvailable == initial.FeedAvailable, "Feed lost on save");
        Check(resumed.WaterQuality == initial.WaterQuality, "Water lost on save");
        for (int i = 0; i < 30; i++)
        {
            Tick(initial, 60000);
            Tick(resumed, 60000);
        }
        Check(resumed.DaysSimulated == 31, "Daily tick count wrong");
        Check(resumed.TotalEggs == initial.TotalEggs, "Save changed deterministic incubation");
        Check(resumed.TotalFish == initial.TotalFish, "Save changed deterministic fish counts");
        Check(resumed.TotalFish > 0, "Roe never hatched");
        Check(resumed.WaterQuality == initial.WaterQuality, "Water simulation not deterministic");
        Check(resumed.TryHarvestAdults("tilapia", 10, _ => true) == 0,
            "Allowed harvesting reserve/immature fish");

        // Simulate a mature cohort to exercise the output commit contract independently
        // of biological maturation time. No game DLL or real item is used in this test.
        var harvest = NewTank();
        Check(harvest.TryStockEggs("tilapia", 1, _ => true), "Unable to create harvest test cohort");
        harvest.Cohorts[0].eggs = 0;
        harvest.Cohorts[0].males = 3;
        harvest.Cohorts[0].females = 4;
        Check(harvest.TryHarvestAdults("tilapia", 4, null) == 0,
            "Harvest without physical output callback removed fish");
        Check(harvest.TryHarvestAdults("tilapia", 4, _ => false) == 0,
            "Rejected physical output still removed fish");
        Check(harvest.TotalFish == 7, "Failed output modified population");
        int delivered = 0;
        Check(harvest.TryHarvestAdults("tilapia", 4, count =>
        {
            delivered += count;
            return true;
        }) == 4, "Committed harvest count incorrect");
        Check(delivered == 4 && harvest.TotalFish == 3,
            "Committed output and fish population diverged");
        Check(harvest.TryHarvestAdults("tilapia", 10, count =>
        {
            delivered += count;
            return true;
        }) == 1, "Harvest did not preserve one breeder pair");
        Check(delivered == 5 && harvest.TotalFish == 2,
            "Reserve pair or physical output count incorrect");
        Check(harvest.TryHarvestAdults("tilapia", 10, _ => true) == 0,
            "Reserve pair could be harvested again");
        var savedHarvest = Reload(harvest);
        Check(savedHarvest.TotalFish == 2 && savedHarvest.Cohorts[0].males == 1
            && savedHarvest.Cohorts[0].females == 1,
            "Harvested population not persisted correctly");
        Console.WriteLine("PASS: transactional roe/feed inputs, stock validation, monoculture, ticks, incubation, save/load, deterministic replay, transactional harvest");
    }
}