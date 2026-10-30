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
        var initial = NewTank();
        Check(!initial.TryStockEggs("unknown", 1), "Accepted unknown fish");
        Check(!initial.TryStockEggs("tilapia", 81), "Exceeded egg capacity");
        Check(initial.TryStockEggs("tilapia", 6), "Unable to stock valid roe");
        Check(!initial.TryStockEggs("carp", 5), "Monoculture constraint ignored");
        initial.AddFeed(1000f);
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
        Check(harvest.TryStockEggs("tilapia", 1), "Unable to create harvest test cohort");
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
        Console.WriteLine("PASS: stock validation, monoculture, ticks, incubation, save/load, deterministic replay, transactional harvest");
    }
}