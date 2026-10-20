using System.Collections.Generic;
using UnityEngine;

namespace LostControl.Model
{
    /// <summary>
    /// One virus: a set of dots on one component of a PC's motherboard.
    /// Positions are normalized motherboard coordinates (0-1, origin bottom-left).
    /// </summary>
    public class VirusModel
    {
        public struct Dot
        {
            public int id;
            public Vector2 position;
        }

        readonly List<Dot> dots = new List<Dot>();
        readonly List<int> spreadOrder = new List<int>();
        readonly SeededRandom random;
        readonly float spreadInterval;
        readonly float spreadDistance;
        int nextId;
        float spreadTimer;

        /// <summary>Area of the infected component. Dots start and spread only inside it.</summary>
        public Rect Area { get; }
        public int Cap { get; }
        public IReadOnlyList<Dot> Dots => dots;
        public int DotCount => dots.Count;
        public bool IsAtCap => dots.Count >= Cap;
        public bool IsCleared => dots.Count == 0;

        public VirusModel(Rect area, int startDots, int cap, float spreadInterval, float spreadDistance, SeededRandom random)
        {
            Area = area;
            Cap = cap;
            this.spreadInterval = spreadInterval;
            this.spreadDistance = spreadDistance;
            this.random = random;

            for (int i = 0; i < startDots && dots.Count < cap; i++)
                AddDot(new Vector2(random.Range(area.xMin, area.xMax), random.Range(area.yMin, area.yMax)));
        }

        /// <summary>Advances the spread clock. Spreads run on a fixed rhythm; one at cap adds nothing.</summary>
        public void Tick(float deltaTime)
        {
            if (IsCleared) return;

            spreadTimer += deltaTime;
            while (spreadTimer >= spreadInterval)
            {
                spreadTimer -= spreadInterval;
                Spread();
            }
        }

        /// <summary>Removes the dot with this id. Returns false if it was already gone.</summary>
        public bool RemoveDot(int id)
        {
            for (int i = 0; i < dots.Count; i++)
            {
                if (dots[i].id != id) continue;
                dots.RemoveAt(i);
                return true;
            }
            return false;
        }

        /// <summary>Removes every dot at once (the PC blacked out).</summary>
        public void Clear()
        {
            dots.Clear();
        }

        // Every existing dot copies itself once, in random order, until the cap is reached.
        void Spread()
        {
            int count = dots.Count;
            spreadOrder.Clear();
            for (int i = 0; i < count; i++) spreadOrder.Add(i);
            for (int i = count - 1; i > 0; i--)
            {
                int j = random.RangeInclusive(0, i);
                (spreadOrder[i], spreadOrder[j]) = (spreadOrder[j], spreadOrder[i]);
            }

            foreach (int index in spreadOrder)
            {
                if (IsAtCap) return;
                AddDot(NearbyPosition(dots[index].position));
            }
        }

        void AddDot(Vector2 position)
        {
            dots.Add(new Dot { id = nextId++, position = position });
        }

        // A random spot within spreadDistance of the parent. Spots outside the component are re-rolled,
        // so copies do not pile up on its edges; clamping is only the fallback after many misses.
        Vector2 NearbyPosition(Vector2 parent)
        {
            const int attempts = 10;
            Vector2 candidate = parent;
            for (int i = 0; i < attempts; i++)
            {
                float angle = random.Range(0f, 2f * Mathf.PI);
                float distance = random.Range(0f, spreadDistance);
                candidate = parent + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
                if (Area.Contains(candidate)) return candidate;
            }
            return ClampToArea(candidate);
        }

        Vector2 ClampToArea(Vector2 position)
        {
            return new Vector2(
                Mathf.Clamp(position.x, Area.xMin, Area.xMax),
                Mathf.Clamp(position.y, Area.yMin, Area.yMax));
        }
    }
}
