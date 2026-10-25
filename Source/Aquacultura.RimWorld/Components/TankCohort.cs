using Verse;
namespace Uillan.Aquacultura
{
    // A cohort is data inside a tank, never a Pawn on the map.
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
        }
    }
}