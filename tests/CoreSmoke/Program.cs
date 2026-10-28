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
        // Mixed species must share one egg budget, including pre-existing roe.
        var fullEggs = new FishTank { AdultEquivalentCapacity = 8, FeedAvailable = 100, TreatmentPowered = true };
        fullEggs.Cohorts.Add(new FishCohort { SpeciesId = "tilapia", Eggs = 80, Males = 1, Females = 1 });
        fullEggs.Cohorts.Add(new FishCohort { SpeciesId = "carp", Males = 2, Females = 2 });
        var fullReport = sim.AdvanceDay(fullEggs, new Random(7));
        Check(fullReport.EggsProduced == 0, "Mixed species spawned beyond tank-wide egg cap");
        Check(fullEggs.Cohorts[0].Eggs + fullEggs.Cohorts[1].Eggs == 80,
            "Tank-wide egg count changed despite full storage");
        var nearlyFullEggs = new FishTank { AdultEquivalentCapacity = 8, FeedAvailable = 100, TreatmentPowered = true };
        nearlyFullEggs.Cohorts.Add(new FishCohort { SpeciesId = "tilapia", Eggs = 79, Males = 1, Females = 1 });
        nearlyFullEggs.Cohorts.Add(new FishCohort { SpeciesId = "carp", Males = 2, Females = 2 });
        var partialReport = sim.AdvanceDay(nearlyFullEggs, new Random(7));
        Check(partialReport.EggsProduced <= 1, "Mixed species exceeded remaining egg budget");
        Check(nearlyFullEggs.Cohorts[0].Eggs + nearlyFullEggs.Cohorts[1].Eggs <= 80,
            "Mixed species exceeded tank-wide egg cap");
        // Adult fish must not be immortal when food or water quality becomes critical.
        var starvingAdults = new FishTank { AdultEquivalentCapacity = 200 };
        starvingAdults.Cohorts.Add(new FishCohort { SpeciesId = "tilapia", Males = 100, Females = 100 });
        var starvingReport = sim.AdvanceDay(starvingAdults, new Random(123));
        int survivingAdults = starvingAdults.Cohorts[0].AdultCount;
        Check(starvingReport.StarvationDeaths > 0, "Adults survived starvation without losses");
        Check(starvingReport.StarvationDeaths == 200 - survivingAdults,
            "Starvation report omitted adult deaths");
        Check(starvingAdults.Cohorts[0].Males >= 0 && starvingAdults.Cohorts[0].Females >= 0,
            "Adult population became negative after starvation");
        var starvingReplay = new FishTank { AdultEquivalentCapacity = 200 };
        starvingReplay.Cohorts.Add(new FishCohort { SpeciesId = "tilapia", Males = 100, Females = 100 });
        var replayReport = sim.AdvanceDay(starvingReplay, new Random(123));
        Check(starvingReplay.Cohorts[0].Males == starvingAdults.Cohorts[0].Males
            && starvingReplay.Cohorts[0].Females == starvingAdults.Cohorts[0].Females
            && replayReport.StarvationDeaths == starvingReport.StarvationDeaths,
            "Adult mortality was not deterministic with the same seed");
        var poorWater = new FishTank { AdultEquivalentCapacity = 200, FeedAvailable = 1000, WaterQuality = .1f };
        poorWater.Cohorts.Add(new FishCohort { SpeciesId = "carp", Males = 100, Females = 100 });
        var poorWaterReport = sim.AdvanceDay(poorWater, new Random(123));
        Check(poorWaterReport.StarvationDeaths > 0, "Poor water quality did not harm adult fish");
        var healthyAdults = new FishTank
            { AdultEquivalentCapacity = 200, FeedAvailable = 1000, TreatmentPowered = true };
        healthyAdults.Cohorts.Add(new FishCohort { SpeciesId = "carp", Males = 100, Females = 100 });
        var healthyReport = sim.AdvanceDay(healthyAdults, new Random(123));
        Check(healthyReport.StarvationDeaths == 0 && healthyAdults.Cohorts[0].AdultCount == 200,
            "Healthy adults died from stress despite adequate conditions");
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
        Console.WriteLine("PASS: couples, tank-wide egg cap, adult stress mortality, single-sex, predation, water bounds, deterministic simulation");
        return 0;
    }
}