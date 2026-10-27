using System;
using LostControl.Config;

namespace LostControl.Model
{
    /// <summary>
    /// SSD trimming (D65): a stack of wide blocks whose ends stick out left and right past a marked target width.
    /// Holding the mouse on one end cuts it back at a steady speed, but one cut only takes up to maxCut; then the
    /// player has to click again. Every side slowly grows back, also while the player is away. The round is done
    /// once every side ends within the tolerance around the mark; a side cut too far has to grow back into it.
    /// The meter fills on a clock.
    /// </summary>
    public class SsdTrimModel : MinigameModel
    {
        readonly SsdTrimSettings settings;
        float[] leftOverhang = new float[0];
        float[] rightOverhang = new float[0];
        int cutBlock = -1;
        bool cutRight;
        float cutLeft;

        public override MinigameType Type => MinigameType.SsdTrim;
        public int Blocks => leftOverhang.Length;
        public float TargetWidth => settings.targetWidth;
        public float Tolerance => settings.tolerance;

        /// <summary>The block and side being cut right now, or -1.</summary>
        public int CutBlock => cutBlock;
        public bool CutRight => cutRight;

        public SsdTrimModel(SsdTrimSettings settings, MinigameSeed seed, int ticketId) : base(seed, ticketId)
        {
            this.settings = settings;
        }

        /// <summary>How far a block's side sticks out past the mark; negative when cut in too far.</summary>
        public float GetOverhang(int block, bool right)
        {
            return right ? rightOverhang[block] : leftOverhang[block];
        }

        /// <summary>The side is trimmed to the mark, within the tolerance.</summary>
        public bool IsTrimmed(int block, bool right)
        {
            return Math.Abs(GetOverhang(block, right)) <= settings.tolerance;
        }

        /// <summary>The mouse went down on one side of a block: a new cut starts there.</summary>
        public void StartCut(int block, bool right)
        {
            if (!IsRoundOpen || block < 0 || block >= Blocks) return;
            cutBlock = block;
            cutRight = right;
            cutLeft = settings.maxCut;
        }

        /// <summary>The mouse went up: the cut ends, however long it was.</summary>
        public void StopCut()
        {
            cutBlock = -1;
        }

        protected override void OnTick(float deltaTime, bool connected)
        {
            AddPressure(deltaTime / settings.meterFillTime);
            if (!connected) StopCut();

            float regrow = settings.regrowPerSecond * deltaTime;
            for (int i = 0; i < Blocks; i++)
            {
                leftOverhang[i] += regrow;
                rightOverhang[i] += regrow;
            }

            if (cutBlock >= 0)
            {
                float cut = Math.Min(settings.cutSpeed * deltaTime, cutLeft);
                if (cutRight) rightOverhang[cutBlock] -= cut;
                else leftOverhang[cutBlock] -= cut;
                cutLeft -= cut;
                if (cutLeft <= 0f) StopCut();
            }

            for (int i = 0; i < Blocks; i++)
                if (!IsTrimmed(i, false) || !IsTrimmed(i, true)) return;
            StopCut();
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
            StopCut();
        }
    }
}
