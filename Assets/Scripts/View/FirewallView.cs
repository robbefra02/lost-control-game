using System.Collections.Generic;
using LostControl.Model;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LostControl.View
{
    /// <summary>
    /// Draws Extinguish the firewall: the wall, its fire spots, the hose and the water stream. Over the board the
    /// system cursor is hidden and the hose follows the pointer; holding the left mouse button sprays.
    /// </summary>
    public class FirewallView : MinigameView
    {
        [Tooltip("Area that maps to normalized board coordinates (0-1).")]
        [SerializeField] RectTransform area;
        [Tooltip("Background of the firewall, stretched to the wall area.")]
        [SerializeField] RectTransform wall;
        [Tooltip("Parent of the fire spots, stretched over the area.")]
        [SerializeField] RectTransform fireLayer;
        [Tooltip("Hose nozzle; its pivot is the tip, placed at the pointer.")]
        [SerializeField] RectTransform hose;
        [Tooltip("Water stream, stretched from the nozzle to where the water stops.")]
        [SerializeField] RectTransform stream;
        [Tooltip("Colour of a spot that just caught fire.")]
        [SerializeField] Color emberColor = new Color32(0x8B, 0x2A, 0x10, 0xFF);
        [Tooltip("Colour of a spot at full intensity.")]
        [SerializeField] Color flameColor = new Color32(0xFF, 0xB0, 0x20, 0xFF);

        readonly List<UnityEngine.UI.Image> spots = new List<UnityEngine.UI.Image>();
        FirewallModel model;

        public override MinigameType Type => MinigameType.Firewall;

        public override void Show(MinigameModel minigame)
        {
            model?.SetHose(false, Vector2.zero, false);
            model = minigame as FirewallModel;
            Cursor.visible = true;
            if (model == null) return;

            Stretch(wall, model.WallArea);
            int count = model.Rows * model.Columns;
            while (spots.Count < count) spots.Add(CreateSpot(spots.Count));
            for (int i = 0; i < spots.Count; i++) spots[i].gameObject.SetActive(i < count);
            for (int row = 0; row < model.Rows; row++)
                for (int column = 0; column < model.Columns; column++)
                    Stretch(spots[row * model.Columns + column].rectTransform, model.GetSpotRect(row, column));
        }

        void Update()
        {
            if (model == null) return;

            var mouse = Mouse.current;
            bool onBoard = false;
            var position = Vector2.zero;
            if (mouse != null && RectTransformUtility.ScreenPointToLocalPointInRectangle(area, mouse.position.ReadValue(), null, out var local))
            {
                onBoard = area.rect.Contains(local);
                position = Rect.PointToNormalized(area.rect, local);
            }
            model.SetHose(onBoard, position, onBoard && mouse.leftButton.isPressed);
            Cursor.visible = !onBoard;
        }

        void LateUpdate()
        {
            if (model == null) return;

            for (int row = 0; row < model.Rows; row++)
            {
                for (int column = 0; column < model.Columns; column++)
                {
                    float value = model.GetIntensity(row, column);
                    var color = Color.Lerp(emberColor, flameColor, value);
                    color.a = value > 0f ? Mathf.Lerp(0.5f, 1f, value) : 0f;
                    spots[row * model.Columns + column].color = color;
                }
            }

            bool hoseShown = model.HoseOnBoard && model.IsRoundOpen;
            hose.gameObject.SetActive(hoseShown);
            if (hoseShown)
            {
                hose.anchorMin = model.HosePosition;
                hose.anchorMax = model.HosePosition;
                hose.anchoredPosition = Vector2.zero;
            }

            bool water = hoseShown && model.IsSpraying;
            stream.gameObject.SetActive(water);
            if (water)
            {
                float half = model.StreamHeight / 2f;
                var nozzle = model.HosePosition;
                Stretch(stream, Rect.MinMaxRect(nozzle.x, nozzle.y - half, Mathf.Max(model.StreamEnd, nozzle.x), nozzle.y + half));
            }
        }

        void OnDisable()
        {
            Cursor.visible = true;
            model?.SetHose(false, Vector2.zero, false);
        }

        UnityEngine.UI.Image CreateSpot(int index)
        {
            var go = new GameObject("Fire " + index, typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image));
            go.layer = gameObject.layer;
            go.transform.SetParent(fireLayer, false);
            var image = go.GetComponent<UnityEngine.UI.Image>();
            image.raycastTarget = false;
            return image;
        }

        static void Stretch(RectTransform rect, Rect normalized)
        {
            rect.anchorMin = normalized.min;
            rect.anchorMax = normalized.max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
