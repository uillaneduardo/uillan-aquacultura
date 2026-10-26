using System;
using System.Collections.Generic;
namespace Uillan.Aquacultura.Core
{
    /// <summary>Discrete, deterministic daily update. Consumes a seeded Random supplied by caller.</summary>
    public sealed class FishSimulation
    {
        private readonly Dictionary<string, FishSpecies> species;
        public FishSimulation(IEnumerable<FishSpecies> specs)
        {
            species = new Dictionary<string, FishSpecies>(StringComparer.Ordinal);
            foreach (var s in specs) species.Add(s.Id, s);
        }
        public bool IsSupported(string speciesId)
        {
            return speciesId != null && species.ContainsKey(speciesId);
        }
        public static IEnumerable<FishSpecies> Defaults()
        {
            yield return new FishSpecies("tilapia", false, 4, 16, .07f, 1.1f, .2f);
            yield return new FishSpecies("carp", false, 5, 22, .05f, .7f, .2f);
            yield return new FishSpecies("catfish", true, 6, 20, .055f, .55f, .28f);
            yield return new FishSpecies("trout", true, 7, 23, .05f, .5f, .33f);
            yield return new FishSpecies("tambaqui", false, 5, 24, .047f, .65f, .3f);
            yield return new FishSpecies("piranha", true, 6, 25, .04f, .45f, .34f);
        }
        private static float Clamp(float n) { return Math.Max(0f, Math.Min(1f, n)); }
        private static int NaturalDeath(int population, float chance, Random random)
        {
            int deaths = 0;
            for (int i = 0; i < population; i++)
                if (random.NextDouble() < chance) deaths++;
            return deaths;
        }
        public DayReport AdvanceDay(FishTank tank, Random random)
        {
            if (tank == null || random == null) throw new ArgumentNullException();
            if (tank.AdultEquivalentCapacity <= 0) throw new ArgumentOutOfRangeException("AdultEquivalentCapacity");
            var report = new DayReport();
            float water = Clamp(tank.WaterQuality);
            float comfort = Math.Min(water, Math.Min(Clamp(tank.TemperatureFitness), Clamp(tank.Oxygenation)));
            float feedDemand = 0f;
            foreach (var c in tank.Cohorts)
            {
                FishSpecies s;
                if (!species.TryGetValue(c.SpeciesId, out s)) continue;
                feedDemand += c.Population * s.FoodPerFish;
            }
            float feedFactor = feedDemand == 0 ? 1f : Clamp(Math.Max(0f, tank.FeedAvailable) / feedDemand);
            float consumed = Math.Min(Math.Max(0f, tank.FeedAvailable), feedDemand);
            tank.FeedAvailable = Math.Max(0f, tank.FeedAvailable - consumed);
            float suitability = comfort * feedFactor;
            bool overcrowded = tank.UsedCapacity >= tank.AdultEquivalentCapacity;
            foreach (var c in tank.Cohorts)
            {
                FishSpecies s;
                if (!species.TryGetValue(c.SpeciesId, out s)) continue;
                // Capacity limits recruitment, not merely spawning; eggs cannot expand indefinitely.
                int hatch = 0;
                if (c.Eggs > 0 && suitability >= .4f)
                {
                    c.EggProgress += 1f / s.IncubationDays;
                    if (c.EggProgress >= 1f)
                    {
                        hatch = Math.Min(c.Eggs, Math.Max(0,
                            (int)Math.Floor((tank.AdultEquivalentCapacity - tank.UsedCapacity) * 10f)));
                        c.Eggs -= hatch;
                        c.Fry += hatch;
                        report.EggsHatched += hatch;
                        c.EggProgress = 0f;
                    }
                }
                if (suitability > .3f && c.Fry + c.Juveniles > 0)
                {
                    c.Progress += s.DailyGrowth * suitability;
                    if (c.Progress >= 1f)
                    {
                        // One cohort holds aggregated fish; gradual stage movement is an MVP simplification.
                        if (c.Fry > 0)
                        {
                            int n = Math.Max(1, c.Fry / 4);
                            n = Math.Min(n, c.Fry);
                            c.Fry -= n;
                            c.Juveniles += n;
                        }
                        else if (c.Juveniles > 0)
                        {
                            int n = Math.Max(1, c.Juveniles / 4);
                            n = Math.Min(n, c.Juveniles);
                            c.Juveniles -= n;
                            int females = n / 2 + (n % 2 == 1 && random.Next(2) == 0 ? 1 : 0);
                            c.Females += females;
                            c.Males += n - females;
                            report.FishMatured += n;
                        }
                        c.Progress -= 1f;
                    }
                }
                if (!overcrowded && suitability >= .65f && c.Males > 0 && c.Females > 0)
                {
                    int pairs = Math.Min(c.Males, c.Females);
                    int spawn = (int)(pairs * s.EggsPerPair * suitability);
                    if (random.NextDouble() < (pairs * s.EggsPerPair * suitability - spawn)) spawn++;
                    // Avoid runaway egg accumulation in a full tank.
                    int eggLimit = tank.AdultEquivalentCapacity * 10;
                    int added = Math.Min(Math.Max(0, eggLimit - c.Eggs), spawn);
                    c.Eggs += added;
                    report.EggsProduced += added;
                }
                if (feedFactor < .2f || comfort < .2f)
                {
                    float risk = Math.Min(.5f, (.2f - Math.Min(feedFactor, comfort)) * 1.5f);
                    int deadFry = NaturalDeath(c.Fry, risk, random);
                    int deadYoung = NaturalDeath(c.Juveniles, risk * .5f, random);
                    c.Fry -= deadFry; c.Juveniles -= deadYoung;
                    report.StarvationDeaths += deadFry + deadYoung;
                }
            }
            // Predators consume vulnerable fish from other cohorts; avoid simultaneous mutation iteration.
            foreach (var predator in tank.Cohorts)
            {
                FishSpecies kind;
                if (!species.TryGetValue(predator.SpeciesId, out kind) || !kind.Predator || predator.AdultCount < 1)
                    continue;
                if (feedFactor >= .75f) continue;
                int budget = Math.Min(predator.AdultCount, 3);
                foreach (var prey in tank.Cohorts)
                {
                    if (budget == 0) break;
                    if (prey == predator) continue;
                    int take = Math.Min(prey.Fry, budget);
                    prey.Fry -= take; budget -= take; report.Predated += take;
                }
            }
            tank.WaterQuality = Clamp(water - Math.Min(.12f, tank.UsedCapacity / tank.AdultEquivalentCapacity * .025f)
                + (tank.TreatmentPowered ? .055f : 0f));
            return report;
        }
    }
}