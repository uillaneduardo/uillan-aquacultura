using System;
using Uillan.Aquacultura.Core;
internal static class Program
{
    static void Check(bool value, string message) { if (!value) throw new Exception(message); }
    static int Main()
    {
        var sim = new FishSimulation(FishSimulation.Defaults());
        var seed = new Random(125);
        var t = new FishTank { AdultEquivalentCapacity = 8, FeedAvailable = 200, TreatmentPowered = true };
        t.Cohorts.Add(new FishCohort { SpeciesId = "tilapia", Males = 2, Females = 2 });
        int eggs = 0;
        for (int i = 0; i < 10; i++) { var x = sim.AdvanceDay(t, seed); eggs += x.EggsProduced; t.FeedAvailable += 10; }
        Check(eggs > 0, "Same-species couple failed to produce eggs");
        Check(t.Cohorts[0].Eggs >= 0 && t.Cohorts[0].Fry >= 0, "Negative population");
        Check(t.WaterQuality > 0 && t.WaterQuality <= 1, "Water quality out of range");
        var alone = new FishTank { FeedAvailable = 100 };
        alone.Cohorts.Add(new FishCohort { SpeciesId = "carp", Males = 4 });
        for (int i = 0; i < 10; i++) sim.AdvanceDay(alone, new Random(i));
        Check(alone.Cohorts[0].Eggs == 0, "Fish reproduced without a female");
        var p = new FishTank { FeedAvailable = 0 };
        p.Cohorts.Add(new FishCohort { SpeciesId = "piranha", Males = 2, Females = 1 });
        p.Cohorts.Add(new FishCohort { SpeciesId = "tilapia", Fry = 5 });
        var eaten = sim.AdvanceDay(p, new Random(1));
        Check(eaten.Predated > 0, "Predation did not occur");
        var stable = new FishTank { FeedAvailable = 1000, TreatmentPowered = true };
        stable.Cohorts.Add(new FishCohort { SpeciesId = "carp", Males = 1, Females = 1 });
        var sim2 = new FishSimulation(FishSimulation.Defaults());
        var stable2 = new FishTank { FeedAvailable = 1000, TreatmentPowered = true };
        stable2.Cohorts.Add(new FishCohort { SpeciesId = "carp", Males = 1, Females = 1 });
        for (int i = 0; i < 20; i++) { sim.AdvanceDay(stable, new Random(i)); sim2.AdvanceDay(stable2, new Random(i)); }
        Check(stable.Cohorts[0].Eggs == stable2.Cohorts[0].Eggs, "Determinism failed");
        Console.WriteLine("PASS: couples, single-sex, predation, water bounds, deterministic simulation");
        return 0;
    }
}