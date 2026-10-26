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
        Check(resumed.TryHarvestAdults("tilapia", 10) == 0, "Allowed harvesting reserve/immature fish");
        Console.WriteLine("PASS: stock validation, monoculture, ticks, incubation, save/load, deterministic replay, harvest reserve");
    }
}