namespace Uillan.Aquacultura.Core
{
    public sealed class FishSpecies
    {
        public readonly string Id;
        public readonly bool Predator;
        public readonly int IncubationDays;
        public readonly int MaturityDays;
        public readonly float DailyGrowth;
        public readonly float EggsPerPair;
        public readonly float FoodPerFish;
        public FishSpecies(string id, bool predator, int incubationDays, int maturityDays,
            float dailyGrowth, float eggsPerPair, float foodPerFish)
        {
            Id = id;
            Predator = predator;
            IncubationDays = incubationDays;
            MaturityDays = maturityDays;
            DailyGrowth = dailyGrowth;
            EggsPerPair = eggsPerPair;
            FoodPerFish = foodPerFish;
        }
    }
}