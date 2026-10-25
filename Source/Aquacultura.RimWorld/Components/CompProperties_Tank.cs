using Verse;
namespace Uillan.Aquacultura
{
    // Not added to XML until the real RimWorld assembly has been built and verified.
    public sealed class CompProperties_Tank : CompProperties
    {
        public int capacity = 8;
        public CompProperties_Tank() { compClass = typeof(CompTank); }
    }
}