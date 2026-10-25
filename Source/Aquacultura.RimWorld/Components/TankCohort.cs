using System;
using Verse;

namespace Uillan.Aquacultura
{
    public sealed class TankCohort : IExposable
    {
        public string speciesDefName = "";
        public int males;
        public int females;
        public int eggs;
        public int juveniles;
        public float averageGrowth;
        public void ExposeData()
        {
            Scribe_Values.Look(ref speciesDefName, "speciesDefName", "");
            Scribe_Values.Look(ref males, "males", 0);
            Scribe_Values.Look(ref females, "females", 0);
            Scribe_Values.Look(ref eggs, "eggs", 0);
            Scribe_Values.Look(ref juveniles, "juveniles", 0);
            Scribe_Values.Look(ref averageGrowth, "averageGrowth", 0f);
            if (Scribe.mode == LoadSaveMode.PostLoadInit) NormalizeAfterLoad();
        }

        public void NormalizeAfterLoad()
        {
            speciesDefName = speciesDefName ?? "";
            males = Math.Max(0, males);
            females = Math.Max(0, females);
            eggs = Math.Max(0, eggs);
            juveniles = Math.Max(0, juveniles);
            if (float.IsNaN(averageGrowth) || float.IsInfinity(averageGrowth))
                averageGrowth = 0f;
            averageGrowth = Math.Max(0f, Math.Min(1f, averageGrowth));
        }
    }
}
