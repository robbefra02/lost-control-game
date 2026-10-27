using System.Collections.Generic;
using LostControl.Model;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LostControl.View
{
    /// <summary>
    /// Draws SSD trimming: the stack of blocks, centred, and two green marks with the tolerance around the target
    /// width. The player holds the mouse button and draws a cutting line anywhere in the area (D67); it stops growing
    /// at the model's cut line length. On release, every block whose middle the line crosses is cut where it crosses.
    /// Blocks the line would cut are lit up while drawing. Needs a raycast target on this object (a transparent image).
    /// </summary>
    public class SsdTrimView : MinigameView, IInitializePotentialDragHandler, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Tooltip("Area that maps to the model's widths (0-1, centred).")]
        [SerializeField] RectTransform area;
        [Tooltip("Parent of the blocks, stretched over the area.")]
        [SerializeField] RectTransform blockLayer;
        [Tooltip("Band at the left end of the target width, as wide as the tolerance on both sides of it.")]
        [SerializeField] RectTransform leftMark;
        [SerializeField] RectTransform rightMark;
        [Tooltip("[ASSUMPTION] Height of the whole stack as a share of the area.")]
        [Range(0.1f, 1f)]
        [SerializeField] float stackHeight = 0.6f;
        [Tooltip("Share of each block's slot left empty as a gap between blocks.")]
        [Range(0f, 0.8f)]
        [SerializeField] float blockGap = 0.25f;
        [SerializeField] Color blockColor = new Color32(0x6B, 0x72, 0x80, 0xFF);
        [Tooltip("[ASSUMPTION] Colour of a block whose two ends are both trimmed to the marks.")]
        [SerializeField] Color trimmedColor = new Color32(0x46, 0xB3, 0x5A, 0xFF);
        [Tooltip("Colour of a block the line being drawn would cut.")]
        [SerializeField] Color cuttingColor = new Color32(0x8F, 0x97, 0xA6, 0xFF);
        [Tooltip("[ASSUMPTION] Colour of the cutting line.")]
        [SerializeField] Color cutLineColor = new Color32(0xE5, 0x48, 0x4D, 0xFF);
        [Tooltip("Thickness of the cutting line, in canvas pixels.")]
        [SerializeField] float cutLineThickness = 3f;

        readonly List<UnityEngine.UI.Image> blocks = new List<UnityEngine.UI.Image>();
        UnityEngine.UI.Image cutLine;
        SsdTrimModel model;
        bool drawing;
        // Ends of the line being drawn, in the area's local space.
        Vector2 lineStart;
        Vector2 lineEnd;

        public override MinigameType Type => MinigameType.SsdTrim;

        float SlotHeight => stackHeight / model.Blocks;
        float StackBottom => 0.5f - stackHeight / 2f;

        public override void Show(MinigameModel minigame)
        {
            drawing = false;
            model = minigame as SsdTrimModel;
            if (model == null) return;

            while (blocks.Count < model.Blocks) blocks.Add(CreateBlock(blocks.Count));
            for (int i = 0; i < blocks.Count; i++) blocks[i].gameObject.SetActive(i < model.Blocks);

            float half = model.TargetWidth / 2f;
            float top = 0.5f + stackHeight / 2f + 0.04f;
            float bottom = 0.5f - stackHeight / 2f - 0.04f;
            Stretch(leftMark, 0.5f - half - model.Tolerance, bottom, 0.5f - half + model.Tolerance, top);
            Stretch(rightMark, 0.5f + half - model.Tolerance, bottom, 0.5f + half + model.Tolerance, top);
        }

        void LateUpdate()
        {
            if (model == null) return;

            if (!model.IsRoundOpen) drawing = false;
            blockLayer.gameObject.SetActive(model.IsRoundOpen);
            for (int i = 0; i < model.Blocks; i++)
            {
                float yMin = StackBottom + i * SlotHeight + SlotHeight * blockGap / 2f;
                float yMax = yMin + SlotHeight * (1f - blockGap);
                Stretch(blocks[i].rectTransform, model.GetEnd(i, false), yMin, model.GetEnd(i, true), yMax);

                bool trimmed = model.IsTrimmed(i, false) && model.IsTrimmed(i, true);
                blocks[i].color = drawing && Crosses(i, out _) ? cuttingColor : trimmed ? trimmedColor : blockColor;
            }
            DrawCutLine();
        }

        void OnDisable()
        {
            drawing = false;
            if (cutLine != null) cutLine.gameObject.SetActive(false);
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            // The line follows the mouse from the first pixel, so a short line can be drawn.
            eventData.useDragThreshold = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (model == null || !model.IsRoundOpen || !ToArea(eventData, out lineStart)) return;
            lineEnd = lineStart;
            drawing = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!drawing || !ToArea(eventData, out var point)) return;
            lineEnd = lineStart + Vector2.ClampMagnitude(point - lineStart, MaxLineLength());
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!drawing) return;
            OnDrag(eventData);
            drawing = false;
            for (int i = 0; i < model.Blocks; i++)
                if (Crosses(i, out float x)) model.Cut(i, x);
        }

        // The model's line length is in block thicknesses.
        float MaxLineLength()
        {
            return model.CutLineLength * area.rect.height * SlotHeight * (1f - blockGap);
        }

        // True when the line crosses the middle of the block within its length; x is where (0-1 across the area).
        bool Crosses(int block, out float x)
        {
            x = 0f;
            var from = Normalized(lineStart);
            var to = Normalized(lineEnd);
            float middle = StackBottom + (block + 0.5f) * SlotHeight;
            if (Mathf.Approximately(from.y, to.y) || (from.y - middle) * (to.y - middle) > 0f) return false;

            x = Mathf.Lerp(from.x, to.x, (middle - from.y) / (to.y - from.y));
            return x > model.GetEnd(block, false) && x < model.GetEnd(block, true);
        }

        bool ToArea(PointerEventData eventData, out Vector2 local)
        {
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(area, eventData.position, eventData.pressEventCamera, out local);
        }

        Vector2 Normalized(Vector2 local)
        {
            var rect = area.rect;
            return new Vector2((local.x - rect.xMin) / rect.width, (local.y - rect.yMin) / rect.height);
        }

        void DrawCutLine()
        {
            if (cutLine == null)
            {
                if (!drawing) return;
                cutLine = CreateImage("Cut line", area);
                var rect = cutLine.rectTransform;
                rect.anchorMin = rect.anchorMax = area.pivot;
                rect.pivot = new Vector2(0f, 0.5f);
            }

            cutLine.gameObject.SetActive(drawing);
            if (!drawing) return;
            var line = lineEnd - lineStart;
            cutLine.color = cutLineColor;
            cutLine.rectTransform.SetAsLastSibling();
            cutLine.rectTransform.anchoredPosition = lineStart;
            cutLine.rectTransform.sizeDelta = new Vector2(line.magnitude, cutLineThickness);
            cutLine.rectTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(line.y, line.x) * Mathf.Rad2Deg);
        }

        UnityEngine.UI.Image CreateBlock(int index)
        {
            return CreateImage("Block " + index, blockLayer);
        }

        UnityEngine.UI.Image CreateImage(string name, RectTransform parent)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image));
            go.layer = gameObject.layer;
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<UnityEngine.UI.Image>();
            image.raycastTarget = false;
            return image;
        }

        static void Stretch(RectTransform rect, float xMin, float yMin, float xMax, float yMax)
        {
            rect.anchorMin = new Vector2(xMin, yMin);
            rect.anchorMax = new Vector2(xMax, yMax);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
