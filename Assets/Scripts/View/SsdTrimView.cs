using System.Collections.Generic;
using LostControl.Model;
using UnityEngine;

namespace LostControl.View
{
    /// <summary>
    /// Draws SSD trimming: the stack of blocks, centred, and two green marks with the tolerance around the target
    /// width. Pressing on a block's left half cuts its left end, its right half its right end, until the mouse is
    /// released or the cut reaches its maximum length.
    /// </summary>
    public class SsdTrimView : MinigameView
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
        [SerializeField] Color cuttingColor = new Color32(0x8F, 0x97, 0xA6, 0xFF);

        readonly List<UnityEngine.UI.Image> blocks = new List<UnityEngine.UI.Image>();
        SsdTrimModel model;

        public override MinigameType Type => MinigameType.SsdTrim;

        public override void Show(MinigameModel minigame)
        {
            model?.StopCut();
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

            blockLayer.gameObject.SetActive(model.IsRoundOpen);
            float half = model.TargetWidth / 2f;
            float slot = stackHeight / model.Blocks;
            float stackBottom = 0.5f - stackHeight / 2f;
            for (int i = 0; i < model.Blocks; i++)
            {
                float yMin = stackBottom + i * slot + slot * blockGap / 2f;
                float yMax = yMin + slot * (1f - blockGap);
                float xMin = 0.5f - half - model.GetOverhang(i, false);
                float xMax = 0.5f + half + model.GetOverhang(i, true);
                Stretch(blocks[i].rectTransform, xMin, yMin, xMax, yMax);

                bool trimmed = model.IsTrimmed(i, false) && model.IsTrimmed(i, true);
                blocks[i].color = model.CutBlock == i ? cuttingColor : trimmed ? trimmedColor : blockColor;
            }
        }

        void OnDisable()
        {
            model?.StopCut();
        }

        UnityEngine.UI.Image CreateBlock(int index)
        {
            var go = new GameObject("Block " + index, typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image), typeof(SsdBlockView));
            go.layer = gameObject.layer;
            go.transform.SetParent(blockLayer, false);
            var view = go.GetComponent<SsdBlockView>();
            view.Index = index;
            view.Pressed += OnBlockPressed;
            view.Released += OnBlockReleased;
            return go.GetComponent<UnityEngine.UI.Image>();
        }

        // Left half of the stack cuts the left end, right half the right end.
        void OnBlockPressed(int index, Vector2 screenPosition)
        {
            if (model == null || !RectTransformUtility.ScreenPointToLocalPointInRectangle(area, screenPosition, null, out var local)) return;
            float x = Rect.PointToNormalized(area.rect, local).x;
            model.StartCut(index, x >= 0.5f);
        }

        void OnBlockReleased()
        {
            model?.StopCut();
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
