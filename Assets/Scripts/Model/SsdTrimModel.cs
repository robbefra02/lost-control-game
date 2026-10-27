using System;
using LostControl.Config;

namespace LostControl.Model
{
    /// <summary>
    /// SSD trimming (D65, D67): a stack of wide blocks whose ends stick out left and right past a marked target width.
    /// The player draws short cutting lines across the ends; the view reports where a line crosses a block, and the
    /// end on that side is cut off there. A click without a line cuts nothing. Every side slowly grows back, also
    /// while the player is away. The round is done once every side ends within the tolerance around the mark; a side
    /// cut too far has to grow back into it. The meter fills on a clock.
    /// </summary>
    public class SsdTrimModel : MinigameModel
    {
        readonly SsdTrimSettings settings;
        float[] leftOverhang = new float[0];
        float[] rightOverhang = new float[0];

        public override MinigameType Type => MinigameType.SsdTrim;
        public int Blocks => leftOverhang.Length;
        public float TargetWidth => settings.targetWidth;
        public float Tolerance => settings.tolerance;

        /// <summary>Longest cutting line, in block thicknesses.</summary>
        public float CutLineLength => settings.cutLineLength;

        public SsdTrimModel(SsdTrimSettings settings, MinigameSeed seed, int ticketId) : base(seed, ticketId)
        {
            this.settings = settings;
        }

        /// <summary>How far a block's side sticks out past the mark; negative when cut in too far.</summary>
        public float GetOverhang(int block, bool right)
        {
            return right ? rightOverhang[block] : leftOverhang[block];
        }

        /// <summary>Where a block's side ends, from 0 (left edge of the minigame) to 1 (right edge).</summary>
        public float GetEnd(int block, bool right)
        {
            float half = settings.targetWidth / 2f;
            return right ? 0.5f + half + rightOverhang[block] : 0.5f - half - leftOverhang[block];
        }

        /// <summary>The side is trimmed to the mark, within the tolerance.</summary>
        public bool IsTrimmed(int block, bool right)
        {
            return Math.Abs(GetOverhang(block, right)) <= settings.tolerance;
        }

        /// <summary>
        /// A cutting line crossed the block at x (0-1 across the minigame): the side of the block that x is on is cut
        /// off there. Nothing happens when x lies beyond the end of the block.
        /// </summary>
        public void Cut(int block, float x)
        {
            if (!IsRoundOpen || block < 0 || block >= Blocks) return;
            float half = settings.targetWidth / 2f;
            if (x < 0.5f)
            {
                if (x > GetEnd(block, false)) leftOverhang[block] = 0.5f - half - x;
            }
            else if (x < GetEnd(block, true))
            {
                rightOverhang[block] = x - 0.5f - half;
            }
        }

        protected override void OnTick(float deltaTime, bool connected)
        {
            AddPressure(deltaTime / settings.meterFillTime);

            float regrow = settings.regrowPerSecond * deltaTime;
            for (int i = 0; i < Blocks; i++)
            {
                leftOverhang[i] += regrow;
                rightOverhang[i] += regrow;
            }

            for (int i = 0; i < Blocks; i++)
                if (!IsTrimmed(i, false) || !IsTrimmed(i, true)) return;
            CompleteRound();
        }

        protected override void Generate(SeededRandom random, int round)
        {
            int count = Math.Max(settings.blocks, 1);
            leftOverhang = new float[count];
            rightOverhang = new float[count];
            for (int i = 0; i < count; i++)
            {
                leftOverhang[i] = random.Range(settings.overhangMin, settings.overhangMax);
                rightOverhang[i] = random.Range(settings.overhangMin, settings.overhangMax);
            }
        }
    }
}
