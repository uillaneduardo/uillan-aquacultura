using System;
using System.Collections.Generic;
using Verse;
namespace Uillan.Aquacultura
{
    // M1-b source only: never reference it from XML before compiling for each game version.
    public sealed class CompTank : ThingComp
    {
        private int schemaVersion = 1;
        private float waterQuality = 1f;
        private bool autoHarvest;
        private int breederPairsToKeep = 1;
        private bool allowMixedSpecies;
        private List<TankCohort> cohorts = new List<TankCohort>();
        public CompProperties_Tank Props => (CompProperties_Tank)props;
        public int Capacity => Props.capacity;
        public float WaterQuality => waterQuality;
        public IReadOnlyList<TankCohort> Cohorts => cohorts;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref schemaVersion, "aqSchemaVersion", 1);
            Scribe_Values.Look(ref waterQuality, "aqWaterQuality", 1f);
            Scribe_Values.Look(ref autoHarvest, "aqAutoHarvest", false);
            Scribe_Values.Look(ref breederPairsToKeep, "aqBreederPairsToKeep", 1);
            Scribe_Values.Look(ref allowMixedSpecies, "aqAllowMixedSpecies", false);
            Scribe_Collections.Look(ref cohorts, "aqCohorts", LookMode.Deep);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (cohorts == null) cohorts = new List<TankCohort>();
                cohorts.RemoveAll(x => x == null);
                waterQuality = Math.Max(0f, Math.Min(1f, waterQuality));
                breederPairsToKeep = Math.Max(0, breederPairsToKeep);
                schemaVersion = Math.Max(1, schemaVersion);
            }
        }
        public override string CompInspectStringExtra()
        {
            return "Aquacultura (development) - planned capacity: " + Capacity;
        }
    }
}