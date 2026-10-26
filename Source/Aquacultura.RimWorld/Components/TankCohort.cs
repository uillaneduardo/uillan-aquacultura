using System;
using Verse;
namespace Uillan.Aquacultura
{
    // Persistent data only. Never spawn a Pawn for each fish.
    public sealed class TankCohort : IExposable
    {
        public string speciesDefName = "";
        public int males, females, eggs, juveniles, fry;
        public float averageGrowth, eggProgress;

        public void ExposeData()
        {
            Scribe_Values.Look(ref speciesDefName, "speciesDefName", "");
            Scribe_Values.Look(ref males, "males", 0);
            Scribe_Values.Look(ref females, "females", 0);
            Scribe_Values.Look(ref eggs, "eggs", 0);
            Scribe_Values.Look(ref juveniles, "juveniles", 0);
            Scribe_Values.Look(ref fry, "aqFry", 0);
            Scribe_Values.Look(ref averageGrowth, "averageGrowth", 0f);
            Scribe_Values.Look(ref eggProgress, "aqEggProgress", 0f);
            if (Scribe.mode == LoadSaveMode.PostLoadInit) NormalizeAfterLoad();
        }

        public void NormalizeAfterLoad()
        {
            speciesDefName = speciesDefName ?? "";
            males = Math.Max(0, males);
            females = Math.Max(0, females);
            eggs = Math.Max(0, eggs);
            juveniles = Math.Max(0, juveniles);
            fry = Math.Max(0, fry);
            averageGrowth = SafeFraction(averageGrowth);
            eggProgress = SafeFraction(eggProgress);
        }

        private static float SafeFraction(float value)
        {
            if (float.IsNaN(value) || float.IsInfinity(value)) return 0f;
            return Math.Max(0f, Math.Min(1f, value));
        }
    }
}