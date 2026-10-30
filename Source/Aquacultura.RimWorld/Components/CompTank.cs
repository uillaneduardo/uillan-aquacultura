using System;
using System.Collections.Generic;
using Uillan.Aquacultura.Core;
using Verse;

namespace Uillan.Aquacultura
{
    /// <summary>
    /// Adapter between RimWorld ticks/Scribe and the testable biological core.
    /// Activation in XML is intentionally held until matching binaries are built.
    /// </summary>
    public sealed class CompTank : ThingComp
    {
        private const int CurrentSchema = 2;
        private const int TicksPerDay = 60000;
        private const int TicksPerRareUpdate = 250;
        private static readonly FishSimulation Simulation =
            new FishSimulation(FishSimulation.Defaults());

        private int schemaVersion = CurrentSchema;
        private int accumulatedTicks;
        private int nextRandomSeed = 712367;
        private int daysSimulated;
        private float waterQuality = 1f;
        private float feedAvailable;
        private bool treatmentPowered;
        private bool autoHarvest;
        private int breederPairsToKeep = 1;
        private bool allowMixedSpecies;
        private List<TankCohort> cohorts = new List<TankCohort>();

        public CompProperties_Tank Props { get { return (CompProperties_Tank)props; } }
        public int Capacity { get { return Props.capacity; } }
        public int DaysSimulated { get { return daysSimulated; } }
        public float WaterQuality { get { return waterQuality; } }
        public float FeedAvailable { get { return feedAvailable; } }
        public IReadOnlyList<TankCohort> Cohorts { get { return cohorts; } }
        public bool AutoHarvest { get { return autoHarvest; } }
        public int BreederPairsToKeep { get { return breederPairsToKeep; } }
        public bool AllowMixedSpecies { get { return allowMixedSpecies; } }

        public int TotalFish
        {
            get
            {
                int sum = 0;
                foreach (var c in cohorts)
                    if (c != null) sum += c.males + c.females + c.fry + c.juveniles;
                return sum;
            }
        }

        // Called by the game's rare ticker. No per-fish ticking and no side effects during render.
        public override void CompTickRare()
        {
            base.CompTickRare();
            accumulatedTicks += TicksPerRareUpdate;
            if (accumulatedTicks >= TicksPerDay)
            {
                accumulatedTicks -= TicksPerDay;
                SimulateDay();
            }
        }

        public void AddFeed(float amount)
        {
            if (!float.IsNaN(amount) && !float.IsInfinity(amount) && amount > 0f)
                feedAvailable = Math.Min(100000f, feedAvailable + amount);
        }

        public void SetTreatmentPowered(bool powered) { treatmentPowered = powered; }

        // Safe API for future jobs that consume real roe items. No item creation here.
        public bool TryStockEggs(string speciesId, int quantity)
        {
            if (quantity <= 0 || !Simulation.IsSupported(speciesId)) return false;
            var existing = cohorts.Find(c => c.speciesDefName == speciesId);
            if (existing == null && !allowMixedSpecies && cohorts.Count > 0) return false;
            int currentEggs = 0;
            foreach (var c in cohorts) currentEggs += c.eggs;
            int room = Capacity * 10 - currentEggs;
            if (quantity > room) return false;
            if (existing == null)
            {
                existing = new TankCohort { speciesDefName = speciesId };
                cohorts.Add(existing);
            }
            existing.eggs += quantity;
            return true;
        }

        // The caller must successfully deliver physical fish items before the population
        // changes. No inventory-free harvest API is exposed to production jobs.
        // deliverItems must return true only after it has committed the exact item count.
        // This is a bridge contract, not yet a RimWorld WorkGiver/JobDriver.
        public int TryHarvestAdults(string speciesId, int requested, Func<int, bool> deliverItems)
        {
            if (requested <= 0 || deliverItems == null) return 0;
            var c = cohorts.Find(x => x.speciesDefName == speciesId);
            if (c == null) return 0;
            int males = Math.Max(0, c.males - breederPairsToKeep);
            int females = Math.Max(0, c.females - breederPairsToKeep);
            int takenMale = Math.Min(males, requested);
            int takenFemale = Math.Min(females, requested - takenMale);
            int total = takenMale + takenFemale;
            if (total == 0 || !deliverItems(total)) return 0;
            c.males -= takenMale;
            c.females -= takenFemale;
            return total;
        }

        private void SimulateDay()
        {
            var state = new FishTank();
            state.AdultEquivalentCapacity = Capacity;
            state.WaterQuality = waterQuality;
            state.FeedAvailable = feedAvailable;
            state.TreatmentPowered = treatmentPowered;
            foreach (var saved in cohorts)
            {
                state.Cohorts.Add(new FishCohort
                {
                    SpeciesId = saved.speciesDefName,
                    Eggs = saved.eggs, Fry = saved.fry, Juveniles = saved.juveniles,
                    Males = saved.males, Females = saved.females,
                    Progress = saved.averageGrowth, EggProgress = saved.eggProgress
                });
            }
            // Persist seed across saves; a reload cannot re-roll a day.
            var rng = new Random(nextRandomSeed);
            nextRandomSeed = (int)((nextRandomSeed * 1664525L + 1013904223L) & 0x7fffffff);
            Simulation.AdvanceDay(state, rng);
            waterQuality = state.WaterQuality;
            feedAvailable = state.FeedAvailable;
            daysSimulated++;
            for (int i = 0; i < cohorts.Count; i++)
            {
                var m = state.Cohorts[i];
                var c = cohorts[i];
                c.eggs = m.Eggs;
                c.fry = m.Fry;
                c.juveniles = m.Juveniles;
                c.males = m.Males;
                c.females = m.Females;
                c.averageGrowth = m.Progress;
                c.eggProgress = m.EggProgress;
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var gizmo in base.CompGetGizmosExtra()) yield return gizmo;
            if (!Prefs.DevMode) yield break;
            yield return new Command_Action
            {
                defaultLabel = "DEV: add 6 tilapia roe",
                defaultDesc = "Test-only stocking; creates no physical items.",
                action = () => TryStockEggs("tilapia", 6)
            };
            yield return new Command_Action
            {
                defaultLabel = "DEV: add fish feed",
                defaultDesc = "Test-only food supply, not linked to the colony inventory.",
                action = () => AddFeed(20f)
            };
            yield return new Command_Action
            {
                defaultLabel = "DEV: simulate one day",
                defaultDesc = "Test-only advance. The normal tick accumulator is not changed.",
                action = () => SimulateDay()
            };
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref schemaVersion, "aqSchemaVersion", CurrentSchema);
            Scribe_Values.Look(ref accumulatedTicks, "aqAccumulatedTicks", 0);
            Scribe_Values.Look(ref nextRandomSeed, "aqRandomSeed", 712367);
            Scribe_Values.Look(ref daysSimulated, "aqDaysSimulated", 0);
            Scribe_Values.Look(ref waterQuality, "aqWaterQuality", 1f);
            Scribe_Values.Look(ref feedAvailable, "aqFeedAvailable", 0f);
            Scribe_Values.Look(ref treatmentPowered, "aqTreatmentPowered", false);
            Scribe_Values.Look(ref autoHarvest, "aqAutoHarvest", false);
            Scribe_Values.Look(ref breederPairsToKeep, "aqBreederPairsToKeep", 1);
            Scribe_Values.Look(ref allowMixedSpecies, "aqAllowMixedSpecies", false);
            Scribe_Collections.Look(ref cohorts, "aqCohorts", LookMode.Deep);
            if (Scribe.mode == LoadSaveMode.PostLoadInit) NormalizeAfterLoad();
        }

        private void NormalizeAfterLoad()
        {
            if (cohorts == null) cohorts = new List<TankCohort>();
            cohorts.RemoveAll(c => c == null);
            foreach (var c in cohorts) c.NormalizeAfterLoad();
            if (float.IsNaN(waterQuality) || float.IsInfinity(waterQuality)) waterQuality = 1f;
            waterQuality = Math.Max(0f, Math.Min(1f, waterQuality));
            if (float.IsNaN(feedAvailable) || float.IsInfinity(feedAvailable)) feedAvailable = 0f;
            feedAvailable = Math.Max(0f, Math.Min(100000f, feedAvailable));
            breederPairsToKeep = Math.Max(0, breederPairsToKeep);
            accumulatedTicks = Math.Max(0, accumulatedTicks % TicksPerDay);
            daysSimulated = Math.Max(0, daysSimulated);
            nextRandomSeed &= 0x7fffffff;
            schemaVersion = CurrentSchema;
        }

        public override string CompInspectStringExtra()
        {
            return "Aquacultura: " + TotalFish + "/" + Capacity + " fish (adult-equivalent capacity)"
                + "\nWater quality: " + (int)(waterQuality * 100f) + "%"
                + "\nEggs: " + TotalEggs
                + "\nFeed: " + feedAvailable.ToString("0.0")
                + "\nDays simulated: " + daysSimulated;
        }

        public int TotalEggs
        {
            get
            {
                int result = 0;
                foreach (var c in cohorts) result += c.eggs;
                return result;
            }
        }
    }
}