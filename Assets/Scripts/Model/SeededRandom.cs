using System;

namespace LostControl.Model
{
    /// <summary>Deterministic random numbers, so a level plays out the same way for the same seed.</summary>
    public class SeededRandom
    {
        readonly Random random;

        public SeededRandom(int seed)
        {
            random = new Random(seed);
        }

        /// <summary>Float in [min, max).</summary>
        public float Range(float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }

        /// <summary>Int in [min, max], both inclusive.</summary>
        public int RangeInclusive(int min, int max)
        {
            return random.Next(min, max + 1);
        }
    }
}
