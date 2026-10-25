using System;
using System.Collections.Generic;
using Verse;

namespace Uillan.Aquacultura
{
    // Versioned serializable state. Never assume pre-existing saves contain this component.
    public sealed class CompTank : ThingComp
    {
        private const int CurrentSchema = 1;
        private int schemaVersion = CurrentSchema;
        private float waterQuality = 1f;
        private bool autoHarvest;
        private int breederPairsToKeep = 1;
        private bool allowMixedSpecies;
        private List<TankCohort> cohorts = new List<TankCohort>();

        public CompProperties_Tank Props => (CompProperties_Tank)props;
        public int Capacity => Props.capacity;
        public float WaterQuality => waterQuality;
        public IReadOnlyList<TankCohort> Cohorts => cohorts;
        public bool AutoHarvest => autoHarvest;
        public int BreederPairsToKeep => breederPairsToKeep;
        public bool AllowMixedSpecies => allowMixedSpecies;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref schemaVersion, "aqSchemaVersion", CurrentSchema);
            Scribe_Values.Look(ref waterQuality, "aqWaterQuality", 1f);
            Scribe_Values.Look(ref autoHarvest, "aqAutoHarvest", false);
            Scribe_Values.Look(ref breederPairsToKeep, "aqBreederPairsToKeep", 1);
            Scribe_Values.Look(ref allowMixedSpecies, "aqAllowMixedSpecies", false);
            Scribe_Collections.Look(ref cohorts, "aqCohorts", LookMode.Deep);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
                NormalizeAfterLoad();
        }

        private void NormalizeAfterLoad()
        {
            if (cohorts == null) cohorts = new List<TankCohort>();
            cohorts.RemoveAll(x => x == null);
            for (int i = 0; i < cohorts.Count; i++)
                cohorts[i].NormalizeAfterLoad();
            if (float.IsNaN(waterQuality) || float.IsInfinity(waterQuality))
                waterQuality = 1f;
            waterQuality = Math.Max(0f, Math.Min(1f, waterQuality));
            breederPairsToKeep = Math.Max(0, breederPairsToKeep);
            schemaVersion = Math.Max(1, schemaVersion);
        }

        public override string CompInspectStringExtra()
        {
            return "Aquacultura - capacity: " + Capacity
                + "\nWater quality: " + (int)(waterQuality * 100f) + "%"
                + "\nFish: " + TotalFish;
        }

        public int TotalFish
        {
            get
            {
                int total = 0;
                foreach (var cohort in cohorts)
                    if (cohort != null)
                        total += cohort.Males + cohort.Females + cohort.Juveniles;
                return total;
            }
        }
    }
}
