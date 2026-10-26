using System;
using System.Collections.Generic;
using LostControl.Config;
using UnityEngine;

namespace LostControl.Model
{
    /// <summary>
    /// Extinguish the firewall: a grid of fire spots on a wall at the right of the board. Spots that no water hits
    /// keep growing, also while the player is away, and a spot at full intensity sets a neighbour alight. The hose
    /// sprays a thin stream to the right of the pointer; per row it touches, it puts out the first burning spot.
    /// The pressure meter is the fire itself; all spots out finishes the round.
    /// </summary>
    public class FirewallModel : MinigameModel
    {
        readonly FirewallSettings settings;
        readonly List<int> hits = new List<int>();
        readonly List<int> neighbours = new List<int>();
        float[] intensity = new float[0];
        float[] spreadTimers = new float[0];
        SeededRandom random;

        public override MinigameType Type => MinigameType.Firewall;
        public int Rows => settings.rows;
        public int Columns => settings.columns;
        public Rect WallArea => settings.wallArea;
        public float StreamHeight => settings.streamHeight;

        /// <summary>The pointer is over the board, so the hose is drawn there.</summary>
        public bool HoseOnBoard { get; private set; }

        /// <summary>Hose nozzle in normalized board coordinates.</summary>
        public Vector2 HosePosition { get; private set; }

        public bool IsSpraying { get; private set; }

        /// <summary>X where the water stops: the nearest burning spot it hits, or the board's right edge.</summary>
        public float StreamEnd { get; private set; } = 1f;

        public FirewallModel(FirewallSettings settings, MinigameSeed seed, int ticketId) : base(seed, ticketId)
        {
            this.settings = settings;
        }

        /// <summary>0-1 for one spot. Row 0 is the bottom row.</summary>
        public float GetIntensity(int row, int column)
        {
            return intensity[row * Columns + column];
        }

        /// <summary>Area of one fire spot in normalized board coordinates. Row 0 is the bottom row.</summary>
        public Rect GetSpotRect(int row, int column)
        {
            var wall = settings.wallArea;
            float width = wall.width / Columns;
            float height = wall.height / Rows;
            return new Rect(wall.xMin + column * width, wall.yMin + row * height, width, height);
        }

        /// <summary>Where the pointer is and whether the button is down. Water only flows while connected.</summary>
        public void SetHose(bool onBoard, Vector2 position, bool spraying)
        {
            HoseOnBoard = onBoard;
            HosePosition = position;
            IsSpraying = onBoard && spraying && IsRoundOpen;
            FindHits();
        }

        /// <summary>The whole fire shrinks evenly until the meter is at this level.</summary>
        public override void LowerPressure(float level)
        {
            if (Pressure <= level || Pressure <= 0f) return;
            float scale = level / Pressure;
            for (int i = 0; i < intensity.Length; i++) intensity[i] *= scale;
            UpdatePressure();
        }

        protected override void OnTick(float deltaTime, bool connected)
        {
            if (!connected) IsSpraying = false;
            FindHits();

            for (int i = 0; i < intensity.Length; i++)
            {
                if (hits.Contains(i)) intensity[i] = Math.Max(intensity[i] - settings.waterPerSecond * deltaTime, 0f);
                else if (intensity[i] > 0f) intensity[i] = Math.Min(intensity[i] + settings.growthPerSecond * deltaTime, 1f);
            }
            Spread(deltaTime);

            UpdatePressure();
            if (Pressure <= 0f) CompleteRound();
        }

        protected override void Generate(SeededRandom random, int round)
        {
            this.random = random;
            int count = Rows * Columns;
            intensity = new float[count];
            spreadTimers = new float[count];

            int fires = random.RangeInclusive(Math.Min(settings.startFiresMin, count), Math.Min(settings.startFiresMax, count));
            for (int lit = 0; lit < fires;)
            {
                int spot = random.RangeInclusive(0, count - 1);
                if (intensity[spot] > 0f) continue;
                intensity[spot] = settings.startIntensity;
                lit++;
            }
            UpdatePressure();
            FindHits();
        }

        // Per row the stream overlaps, the first burning spot right of the nozzle takes the water.
        void FindHits()
        {
            hits.Clear();
            StreamEnd = 1f;
            if (!IsSpraying) return;

            float bottom = HosePosition.y - settings.streamHeight / 2f;
            float top = HosePosition.y + settings.streamHeight / 2f;
            for (int row = 0; row < Rows; row++)
            {
                var rowRect = GetSpotRect(row, 0);
                if (rowRect.yMax <= bottom || rowRect.yMin >= top) continue;
                for (int column = 0; column < Columns; column++)
                {
                    var spot = GetSpotRect(row, column);
                    int index = row * Columns + column;
                    if (spot.xMax <= HosePosition.x || intensity[index] <= 0f) continue;
                    hits.Add(index);
                    StreamEnd = Math.Min(StreamEnd, Math.Max(spot.xMin, HosePosition.x));
                    break;
                }
            }
        }

        // A spot at full intensity lights one random unburnt neighbour (up, down, left, right) every spreadDelay.
        void Spread(float deltaTime)
        {
            for (int i = 0; i < intensity.Length; i++)
            {
                if (intensity[i] < 1f)
                {
                    spreadTimers[i] = 0f;
                    continue;
                }
                spreadTimers[i] += deltaTime;
                if (spreadTimers[i] < settings.spreadDelay) continue;
                spreadTimers[i] = 0f;

                int row = i / Columns;
                int column = i % Columns;
                neighbours.Clear();
                if (row > 0) AddIfUnburnt(i - Columns);
                if (row < Rows - 1) AddIfUnburnt(i + Columns);
                if (column > 0) AddIfUnburnt(i - 1);
                if (column < Columns - 1) AddIfUnburnt(i + 1);
                if (neighbours.Count > 0) intensity[neighbours[random.RangeInclusive(0, neighbours.Count - 1)]] = settings.spreadIntensity;
            }
        }

        void AddIfUnburnt(int index)
        {
            if (intensity[index] <= 0f) neighbours.Add(index);
        }

        void UpdatePressure()
        {
            float total = 0f;
            foreach (float value in intensity) total += value;
            Pressure = intensity.Length > 0 ? total / intensity.Length : 0f;
        }
    }
}
