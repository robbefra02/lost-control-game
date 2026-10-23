using System;
using LostControl.Config;

namespace LostControl.Model
{
    /// <summary>
    /// Find the off-folder: a grid of identical folder icons in which exactly one differs in one way.
    /// Clicking it finishes the round; clicking any other folder adds pressure. The meter also fills on a clock.
    /// </summary>
    public class OffFolderModel : MinigameModel
    {
        /// <summary>Number of file glyph shapes a folder can show. The view draws one shape per glyph index.</summary>
        public const int GlyphCount = 5;

        /// <summary>How one folder icon looks. The view turns these numbers into shapes and colours.</summary>
        public struct FolderLook
        {
            /// <summary>File glyph shape, 0 to GlyphCount - 1.</summary>
            public int glyph;
            public bool tabRight;
            /// <summary>-1 to 1: darker below 0, lighter above 0.</summary>
            public float shade;
            /// <summary>Degrees.</summary>
            public float rotation;
            /// <summary>1 is normal size.</summary>
            public float scale;
        }

        readonly OffFolderSettings settings;
        FolderLook baseLook;
        FolderLook oddLook;

        public override MinigameType Type => MinigameType.OffFolder;
        public int Columns => settings.columns;
        public int Rows => settings.rows;
        public int FolderCount => Columns * Rows;

        /// <summary>Grid index of the folder that differs this round.</summary>
        public int OddIndex { get; private set; }

        public FolderDifference Difference { get; private set; }

        /// <summary>The player clicked a folder that is not the off one. Passes its grid index.</summary>
        public event Action<int> WrongClick;

        public OffFolderModel(OffFolderSettings settings, MinigameSeed seed, int ticketId) : base(seed, ticketId)
        {
            this.settings = settings;
        }

        public FolderLook GetLook(int index)
        {
            return index == OddIndex ? oddLook : baseLook;
        }

        public void ClickFolder(int index)
        {
            if (!IsRoundOpen || index < 0 || index >= FolderCount) return;

            if (index == OddIndex)
            {
                CompleteRound();
                return;
            }
            AddPressure(settings.wrongClickPressure);
            WrongClick?.Invoke(index);
        }

        protected override void OnTick(float deltaTime, bool connected)
        {
            AddPressure(deltaTime / settings.meterFillTime);
        }

        protected override void Generate(SeededRandom random, int round)
        {
            baseLook = new FolderLook
            {
                glyph = random.RangeInclusive(0, GlyphCount - 1),
                tabRight = random.RangeInclusive(0, 1) == 1,
                scale = 1f,
            };
            OddIndex = random.RangeInclusive(0, FolderCount - 1);

            var differences = settings.differences;
            Difference = differences != null && differences.Length > 0
                ? differences[random.RangeInclusive(0, differences.Length - 1)]
                : FolderDifference.Glyph;

            // Continuous differences can shrink or grow per round; the sign is random so lighter and darker both occur.
            float strength = settings.DifferenceScale(round) * (random.RangeInclusive(0, 1) == 1 ? 1f : -1f);
            oddLook = baseLook;
            switch (Difference)
            {
                case FolderDifference.Glyph:
                    oddLook.glyph = (baseLook.glyph + random.RangeInclusive(1, GlyphCount - 1)) % GlyphCount;
                    break;
                case FolderDifference.TabSide:
                    oddLook.tabRight = !baseLook.tabRight;
                    break;
                case FolderDifference.Shade:
                    oddLook.shade = strength * settings.shadeDifference;
                    break;
                case FolderDifference.Rotation:
                    oddLook.rotation = strength * settings.rotationDifference;
                    break;
                case FolderDifference.Size:
                    oddLook.scale = 1f + strength * settings.sizeDifference;
                    break;
            }
        }
    }
}
