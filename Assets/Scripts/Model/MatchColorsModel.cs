using System;
using LostControl.Config;

namespace LostControl.Model
{
    /// <summary>
    /// Match the colors: a memory game. Every card is face down and has one partner of the same colour.
    /// The first pick turns a card face up; a second pick of the same colour keeps both up, any other colour
    /// shows both for a moment and turns them back. All pairs matched finishes the round. Leaving the PC forgets
    /// an unfinished pick; matched pairs stay. The meter fills on a clock.
    /// </summary>
    public class MatchColorsModel : MinigameModel
    {
        readonly MatchColorsSettings settings;
        int[] colors = new int[0];
        bool[] matched = new bool[0];
        int firstPick = -1;
        int secondPick = -1;
        float revealTimer;

        public override MinigameType Type => MinigameType.MatchColors;
        public int Columns => settings.columns;
        public int Rows => settings.rows;
        /// <summary>Cards in the grid. An odd grid leaves its last cell empty, so every card has a partner.</summary>
        public int CardCount => PairCount * 2;
        public int PairCount => Columns * Rows / 2;

        /// <summary>The player picked two cards of different colours. Passes both grid indices.</summary>
        public event Action<int, int> Mismatch;

        public MatchColorsModel(MatchColorsSettings settings, MinigameSeed seed, int ticketId) : base(seed, ticketId)
        {
            this.settings = settings;
        }

        /// <summary>Colour of a card, 0 to PairCount - 1. Its partner has the same colour.</summary>
        public int GetColor(int index)
        {
            return colors[index];
        }

        public bool IsMatched(int index)
        {
            return matched[index];
        }

        /// <summary>The card's colour is visible: it is matched, picked, or part of a mismatch being shown.</summary>
        public bool IsFaceUp(int index)
        {
            return matched[index] || index == firstPick || index == secondPick;
        }

        public void PickCard(int index)
        {
            if (!IsRoundOpen || index < 0 || index >= CardCount) return;
            if (matched[index] || index == firstPick || secondPick >= 0) return;

            if (firstPick < 0)
            {
                firstPick = index;
                return;
            }

            if (colors[index] == colors[firstPick])
            {
                matched[index] = true;
                matched[firstPick] = true;
                firstPick = -1;
                if (Array.TrueForAll(matched, m => m)) CompleteRound();
                return;
            }

            secondPick = index;
            revealTimer = settings.mismatchRevealTime;
            Mismatch?.Invoke(firstPick, secondPick);
        }

        protected override void OnTick(float deltaTime, bool connected)
        {
            AddPressure(deltaTime / settings.meterFillTime);

            if (!connected)
            {
                ForgetPicks();
                return;
            }
            if (secondPick < 0) return;
            revealTimer -= deltaTime;
            if (revealTimer <= 0f) ForgetPicks();
        }

        // Pairs in order (0, 0, 1, 1, ...), then a Fisher-Yates shuffle from this round's stream.
        protected override void Generate(SeededRandom random, int round)
        {
            int count = CardCount;
            colors = new int[count];
            matched = new bool[count];
            for (int i = 0; i < count; i++) colors[i] = i / 2;
            for (int i = count - 1; i > 0; i--)
            {
                int j = random.RangeInclusive(0, i);
                (colors[i], colors[j]) = (colors[j], colors[i]);
            }
            ForgetPicks();
        }

        void ForgetPicks()
        {
            firstPick = -1;
            secondPick = -1;
        }
    }
}
