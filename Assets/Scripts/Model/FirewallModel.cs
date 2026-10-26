using System;
using System.Collections.Generic;
using LostControl.Config;
using UnityEngine;

namespace LostControl.Model
{
    /// <summary>
    /// Extinguish the firewall (D52): fire burns in front of a wall on the right of the board. It starts at the foot
    /// of the wall and grows upwards and away from it, also while the player is away. The hose fires drops sideways
    /// to the right; gravity bends them down. A drop that reaches a burning spot puts out part of it and is gone.
    /// The meter shows the fire's size up to the damage threshold; above it the PC takes a damage level every
    /// damageInterval seconds. All fire out finishes the round.
    /// </summary>
    public class FirewallModel : MinigameModel
    {
        readonly FirewallSettings settings;
        readonly List<Vector2> dropPositions = new List<Vector2>();
        readonly List<Vector2> dropVelocities = new List<Vector2>();
        readonly List<int> neighbours = new List<int>();
        float[] intensity = new float[0];
        float[] spreadTimers = new float[0];
        float[] water = new float[0];
        SeededRandom random;
        float dropTimer;
        float damageTimer;

        public override MinigameType Type => MinigameType.Firewall;
        public int Rows => settings.rows;
        public int Columns => settings.columns;
        public Rect WallArea => settings.wallArea;

        /// <summary>0-1: how much of the fire area burns, weighted by intensity.</summary>
        public float FireSize { get; private set; }

        /// <summary>The pointer is over the board, so the hose is drawn there.</summary>
        public bool HoseOnBoard { get; private set; }

        /// <summary>Hose nozzle in normalized board coordinates.</summary>
        public Vector2 HosePosition { get; private set; }

        public bool IsSpraying { get; private set; }

        /// <summary>Drops in flight, in normalized board coordinates.</summary>
        public IReadOnlyList<Vector2> Drops => dropPositions;

        public FirewallModel(FirewallSettings settings, MinigameSeed seed, int ticketId) : base(seed, ticketId)
        {
            this.settings = settings;
        }

        /// <summary>0-1 for one spot. Column 0 is next to the wall, row 0 is on the floor.</summary>
        public float GetIntensity(int row, int column)
        {
            return intensity[row * Columns + column];
        }

        /// <summary>Area of one fire spot in normalized board coordinates. Column 0 is next to the wall, row 0 is on the floor.</summary>
        public Rect GetSpotRect(int row, int column)
        {
            var area = settings.fireArea;
            float width = area.width / Columns;
            float height = area.height / Rows;
            return new Rect(area.xMax - (column + 1) * width, area.yMin + row * height, width, height);
        }

        /// <summary>Where the pointer is and whether the button is down. Water only flows while connected.</summary>
        public void SetHose(bool onBoard, Vector2 position, bool spraying)
        {
            HoseOnBoard = onBoard;
            HosePosition = position;
            IsSpraying = onBoard && spraying && IsRoundOpen;
        }

        /// <summary>Above the threshold (a full meter) the PC takes a level every damageInterval; below it the time is kept.</summary>
        public override bool CheckDamage(float deltaTime, float pressureAfterDamage)
        {
            if (Pressure < 1f) return false;
            damageTimer += deltaTime;
            if (damageTimer < settings.damageInterval) return false;
            damageTimer -= settings.damageInterval;
            return true;
        }

        protected override void OnTick(float deltaTime, bool connected)
        {
            if (!connected)
            {
                IsSpraying = false;
                dropPositions.Clear();
                dropVelocities.Clear();
            }

            SpawnDrops(deltaTime);
            MoveDrops(deltaTime);
            for (int i = 0; i < intensity.Length; i++)
            {
                if (water[i] > 0f) intensity[i] = Math.Max(intensity[i] - water[i], 0f);
                else if (intensity[i] > 0f) intensity[i] = Math.Min(intensity[i] + settings.growthPerSecond * deltaTime, 1f);
            }
            Spread(deltaTime);

            UpdateSize();
            if (FireSize <= 0f) CompleteRound();
        }

        // The fire starts in the bottom row, in the spots right next to the wall.
        protected override void Generate(SeededRandom random, int round)
        {
            this.random = random;
            int count = Rows * Columns;
            intensity = new float[count];
            spreadTimers = new float[count];
            water = new float[count];
            dropPositions.Clear();
            dropVelocities.Clear();
            dropTimer = 0f;
            damageTimer = 0f;

            int fires = random.RangeInclusive(Math.Min(settings.startFiresMin, Columns), Math.Min(settings.startFiresMax, Columns));
            for (int column = 0; column < fires; column++) intensity[column] = settings.startIntensity;
            UpdateSize();
        }

        void SpawnDrops(float deltaTime)
        {
            if (!IsSpraying)
            {
                dropTimer = 0f;
                return;
            }
            dropTimer += deltaTime * settings.dropsPerSecond;
            while (dropTimer >= 1f)
            {
                dropTimer -= 1f;
                dropPositions.Add(HosePosition);
                dropVelocities.Add(new Vector2(settings.dropSpeed, 0f));
            }
        }

        // Moves every drop along its arc and collects the water each spot receives this tick. Drops that hit fire,
        // the wall, the floor or the edge of the board are gone.
        void MoveDrops(float deltaTime)
        {
            Array.Clear(water, 0, water.Length);
            for (int d = dropPositions.Count - 1; d >= 0; d--)
            {
                var velocity = dropVelocities[d] + new Vector2(0f, -settings.gravity * deltaTime);
                var position = dropPositions[d] + velocity * deltaTime;
                dropVelocities[d] = velocity;
                dropPositions[d] = position;

                bool gone = position.x >= settings.wallArea.xMin || position.y <= 0f || position.x > 1f;
                int spot = SpotAt(position);
                if (!gone && spot >= 0 && intensity[spot] > 0f)
                {
                    water[spot] += settings.waterPerDrop;
                    gone = true;
                }
                if (!gone) continue;
                dropPositions.RemoveAt(d);
                dropVelocities.RemoveAt(d);
            }
        }

        int SpotAt(Vector2 position)
        {
            var area = settings.fireArea;
            if (!area.Contains(position)) return -1;
            int column = Mathf.Clamp((int)((area.xMax - position.x) / area.width * Columns), 0, Columns - 1);
            int row = Mathf.Clamp((int)((position.y - area.yMin) / area.height * Rows), 0, Rows - 1);
            return row * Columns + column;
        }

        // A spot at full intensity lights one random unburnt neighbour every spreadDelay. The wall and the floor
        // bound the area, so fire that starts at the foot of the wall grows upwards and away from it.
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
                if (row < Rows - 1) AddIfUnburnt(i + Columns);
                if (row > 0) AddIfUnburnt(i - Columns);
                if (column < Columns - 1) AddIfUnburnt(i + 1);
                if (column > 0) AddIfUnburnt(i - 1);
                if (neighbours.Count > 0) intensity[neighbours[random.RangeInclusive(0, neighbours.Count - 1)]] = settings.spreadIntensity;
            }
        }

        void AddIfUnburnt(int index)
        {
            if (intensity[index] <= 0f) neighbours.Add(index);
        }

        void UpdateSize()
        {
            float total = 0f;
            foreach (float value in intensity) total += value;
            FireSize = intensity.Length > 0 ? total / intensity.Length : 0f;
            Pressure = Math.Min(FireSize / settings.damageThreshold, 1f);
        }
    }
}
