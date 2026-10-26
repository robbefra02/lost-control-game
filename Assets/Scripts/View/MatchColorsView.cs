using System.Collections.Generic;
using LostControl.Model;
using UnityEngine;
using UnityEngine.UI;

namespace LostControl.View
{
    /// <summary>
    /// Draws Match the colors as a grid of cards and passes card clicks to the model.
    /// Every frame each card mirrors the model: face up or down, matched or not; the grid hides once no round is open.
    /// </summary>
    public class MatchColorsView : MinigameView
    {
        [SerializeField] GridLayoutGroup grid;
        [SerializeField] ColorCardView cardPrefab;

        [Tooltip("[ASSUMPTION] Card colour per pair, in the model's colour order. Needs one colour per pair (8 for a 4x4 grid).")]
        [SerializeField] Color[] pairColors =
        {
            new Color32(0xE5, 0x48, 0x4D, 0xFF),
            new Color32(0xF0, 0x8C, 0x2E, 0xFF),
            new Color32(0xF2, 0xD1, 0x3A, 0xFF),
            new Color32(0x46, 0xB3, 0x5A, 0xFF),
            new Color32(0x3B, 0xC4, 0xD6, 0xFF),
            new Color32(0x3E, 0x6F, 0xE0, 0xFF),
            new Color32(0x9B, 0x59, 0xD0, 0xFF),
            new Color32(0xF0, 0x7A, 0xB8, 0xFF),
        };

        readonly List<ColorCardView> cards = new List<ColorCardView>();
        MatchColorsModel model;

        public override MinigameType Type => MinigameType.MatchColors;

        public override void Show(MinigameModel minigame)
        {
            model = minigame as MatchColorsModel;
            if (model == null) return;

            while (cards.Count < model.CardCount) cards.Add(CreateCard(cards.Count));
            for (int i = 0; i < cards.Count; i++) cards[i].gameObject.SetActive(i < model.CardCount);
        }

        void LateUpdate()
        {
            if (model == null) return;

            grid.gameObject.SetActive(model.IsRoundOpen);
            GridCells.FitSquare(grid, model.Columns, model.Rows);
            for (int i = 0; i < model.CardCount; i++)
                cards[i].SetFace(model.IsFaceUp(i), pairColors[model.GetColor(i) % pairColors.Length], model.IsMatched(i));
        }

        ColorCardView CreateCard(int index)
        {
            var card = Instantiate(cardPrefab, grid.transform);
            card.name = "Card " + index;
            card.Index = index;
            card.Clicked += OnCardClicked;
            return card;
        }

        void OnCardClicked(int index)
        {
            model?.PickCard(index);
        }
    }
}
