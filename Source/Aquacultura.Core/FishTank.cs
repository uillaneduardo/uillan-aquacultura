using System;
using System.Collections.Generic;
namespace Uillan.Aquacultura.Core
{
    public sealed class FishCohort
    {
        public string SpeciesId = "";
        public int Eggs;
        public int Fry;
        public int Juveniles;
        public int Males;
        public int Females;
        public float Progress;
        public float EggProgress;
        public int Population { get { return Fry + Juveniles + Males + Females; } }
        public int AdultCount { get { return Males + Females; } }
    }
    public sealed class FishTank
    {
        public int AdultEquivalentCapacity = 8;
        public float WaterQuality = 1f;
        public float FeedAvailable;
        public float TemperatureFitness = 1f;
        public float Oxygenation = 1f;
        public bool TreatmentPowered;
        public readonly List<FishCohort> Cohorts = new List<FishCohort>();
        public int FishCount
        {
            get
            {
                int sum = 0;
                foreach (var c in Cohorts) sum += c.Population;
                return sum;
            }
        }
        public float UsedCapacity
        {
            get
            {
                float sum = 0f;
                foreach (var c in Cohorts)
                    sum += c.AdultCount + c.Juveniles * 0.5f + c.Fry * 0.1f;
                return sum;
            }
        }
    }
    public sealed class DayReport
    {
        public int EggsProduced;
        public int EggsHatched;
        public int FishMatured;
        public int Predated;
        public int StarvationDeaths;
    }
}