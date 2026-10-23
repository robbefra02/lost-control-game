using System;
using LostControl.Model;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LostControl.View
{
    /// <summary>One clickable folder in Find the off-folder. Draws a FolderLook and reports clicks; the model decides what happens.</summary>
    public class FolderIconView : MonoBehaviour, IPointerClickHandler
    {
        [Serializable]
        struct GlyphShape
        {
            [Tooltip("Sprite for the file glyph; empty draws a plain rectangle.")]
            public Sprite sprite;
            [Tooltip("Glyph size as a fraction of the folder icon.")]
            public Vector2 size;
            public float rotation;
        }

        [Tooltip("Transparent hit area behind the icon; flashes on a wrong click.")]
        [SerializeField] UnityEngine.UI.Image background;
        [Tooltip("Rotated and scaled as a whole for the rotation and size differences.")]
        [SerializeField] RectTransform icon;
        [SerializeField] UnityEngine.UI.Image body;
        [SerializeField] UnityEngine.UI.Image tab;
        [SerializeField] UnityEngine.UI.Image glyph;
        [SerializeField] Color folderColor = new Color32(0xE3, 0xB6, 0x4C, 0xFF);
        [SerializeField] Color tabColor = new Color32(0xC9, 0x9A, 0x33, 0xFF);
        [Tooltip("Background colour at the start of the wrong-click flash.")]
        [SerializeField] Color flashColor = new Color32(0x9A, 0x2C, 0x2C, 0xFF);
        [SerializeField] float flashDuration = 0.5f;

        [Tooltip("One shape per glyph index (OffFolderModel.GlyphCount).")]
        [SerializeField] GlyphShape[] glyphShapes =
        {
            new GlyphShape { size = new Vector2(0.26f, 0.40f) },
            new GlyphShape { size = new Vector2(0.46f, 0.24f) },
            new GlyphShape { size = new Vector2(0.34f, 0.34f) },
            new GlyphShape { size = new Vector2(0.28f, 0.28f), rotation = 45f },
            new GlyphShape { size = new Vector2(0.34f, 0.34f) },
        };

        float flashTimer;

        public int Index { get; set; }
        public event Action<int> Clicked;

        public void SetLook(OffFolderModel.FolderLook look)
        {
            icon.localEulerAngles = new Vector3(0f, 0f, look.rotation);
            icon.localScale = new Vector3(look.scale, look.scale, 1f);

            var shadeTarget = look.shade > 0f ? Color.white : Color.black;
            float shade = Mathf.Abs(look.shade);
            body.color = Color.Lerp(folderColor, shadeTarget, shade);
            tab.color = Color.Lerp(tabColor, shadeTarget, shade);

            var tabRect = tab.rectTransform;
            tabRect.anchorMin = new Vector2(look.tabRight ? 0.58f : 0.08f, tabRect.anchorMin.y);
            tabRect.anchorMax = new Vector2(look.tabRight ? 0.92f : 0.42f, tabRect.anchorMax.y);

            var shape = glyphShapes[look.glyph % glyphShapes.Length];
            glyph.sprite = shape.sprite;
            var glyphRect = glyph.rectTransform;
            glyphRect.anchorMin = glyphRect.anchorMax = new Vector2(0.5f, 0.45f);
            glyphRect.sizeDelta = Vector2.zero;
            glyphRect.anchorMin -= shape.size / 2f;
            glyphRect.anchorMax += shape.size / 2f;
            glyphRect.localEulerAngles = new Vector3(0f, 0f, shape.rotation);
        }

        public void Flash()
        {
            flashTimer = flashDuration;
        }

        void Update()
        {
            if (flashTimer <= 0f && background.color.a <= 0f) return;
            flashTimer = Mathf.Max(flashTimer - Time.deltaTime, 0f);
            var clear = new Color(flashColor.r, flashColor.g, flashColor.b, 0f);
            background.color = Color.Lerp(clear, flashColor, flashTimer / flashDuration);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Clicked?.Invoke(Index);
        }
    }
}
