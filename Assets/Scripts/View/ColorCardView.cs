using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LostControl.View
{
    /// <summary>One card in Match the colors. Draws its colour or its back and reports clicks; the model decides what happens.</summary>
    public class ColorCardView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] UnityEngine.UI.Image face;
        [SerializeField] Color backColor = new Color32(0x4A, 0x50, 0x5B, 0xFF);
        [Tooltip("[ASSUMPTION] Opacity of matched cards, so the cards still in play stand out.")]
        [Range(0f, 1f)]
        [SerializeField] float matchedAlpha = 0.35f;

        public int Index { get; set; }
        public event Action<int> Clicked;

        public void SetFace(bool faceUp, Color color, bool matched)
        {
            var shown = faceUp ? color : backColor;
            if (matched) shown.a *= matchedAlpha;
            face.color = shown;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Clicked?.Invoke(Index);
        }
    }
}
