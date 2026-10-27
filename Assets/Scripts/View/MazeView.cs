using System.Collections.Generic;
using LostControl.Model;
using UnityEngine;

namespace LostControl.View
{
    /// <summary>
    /// Draws the Maze (walls, start, exit and the dot) with square cells as large as its area allows, and passes the
    /// held d-pad arrows to the model (D61: the maze is played with the mouse). The d-pad only exists while this view
    /// is shown, i.e. while connected.
    /// </summary>
    public class MazeView : MinigameView
    {
        [Tooltip("Area the maze is fitted into.")]
        [SerializeField] RectTransform area;
        [Tooltip("The d-pad arrows; the dot moves in the sum of the held arrows' directions.")]
        [SerializeField] DPadButtonView[] dPad;
        [Tooltip("Sized to the maze and centred in the area; walls, markers and the dot use its normalized coordinates.")]
        [SerializeField] RectTransform maze;
        [Tooltip("Parent of the wall pieces, stretched over the maze.")]
        [SerializeField] RectTransform wallLayer;
        [SerializeField] RectTransform startMarker;
        [SerializeField] RectTransform exitMarker;
        [SerializeField] UnityEngine.UI.Image dot;
        [SerializeField] Color wallColor = new Color32(0xC8, 0xCC, 0xD4, 0xFF);
        [SerializeField] Color dotColor = new Color32(0x4C, 0x8B, 0xF5, 0xFF);
        [Tooltip("Dot colour at the start of the flash after a wall hit.")]
        [SerializeField] Color hitColor = new Color32(0xE5, 0x48, 0x4D, 0xFF);
        [SerializeField] float hitFlashDuration = 0.4f;
        [Tooltip("Wall thickness, in canvas pixels.")]
        [SerializeField] float wallThickness = 4f;

        readonly List<UnityEngine.UI.Image> walls = new List<UnityEngine.UI.Image>();
        MazeModel model;
        int shownRound = -1;
        float flashTimer;

        public override MinigameType Type => MinigameType.Maze;

        public override void Show(MinigameModel minigame)
        {
            if (model != null)
            {
                model.SetMove(Vector2.zero);
                model.WallHit -= OnWallHit;
            }
            model = minigame as MazeModel;
            shownRound = -1;
            flashTimer = 0f;
            if (model == null) return;
            model.WallHit += OnWallHit;
        }

        void Update()
        {
            if (model == null) return;

            var direction = Vector2.zero;
            foreach (var arrow in dPad)
                if (arrow.IsHeld) direction += arrow.Direction;
            model.SetMove(direction);
        }

        void LateUpdate()
        {
            if (model == null) return;

            maze.gameObject.SetActive(model.IsRoundOpen);
            float cell = Mathf.Min(area.rect.width / model.Columns, area.rect.height / model.Rows);
            maze.sizeDelta = new Vector2(cell * model.Columns, cell * model.Rows);
            if (model.Round != shownRound)
            {
                shownRound = model.Round;
                BuildWalls();
            }

            var size = new Vector2(model.Columns, model.Rows);
            dot.rectTransform.anchorMin = model.DotPosition / size;
            dot.rectTransform.anchorMax = model.DotPosition / size;
            dot.rectTransform.anchoredPosition = Vector2.zero;
            dot.rectTransform.sizeDelta = Vector2.one * (2f * model.DotRadius * cell);

            flashTimer = Mathf.Max(flashTimer - Time.deltaTime, 0f);
            dot.color = Color.Lerp(dotColor, hitColor, hitFlashDuration > 0f ? flashTimer / hitFlashDuration : 0f);
        }

        void OnDisable()
        {
            model?.SetMove(Vector2.zero);
        }

        void BuildWalls()
        {
            var size = new Vector2(model.Columns, model.Rows);
            SetCell(startMarker, Vector2Int.zero, size);
            SetCell(exitMarker, model.Exit, size);

            int used = 0;
            for (int y = 0; y <= model.Rows; y++)
                for (int x = 0; x < model.Columns; x++)
                    if (model.HorizontalWall(x, y)) PlaceWall(used++, new Vector2(x, y), new Vector2(x + 1, y), size);
            for (int x = 0; x <= model.Columns; x++)
                for (int y = 0; y < model.Rows; y++)
                    if (model.VerticalWall(x, y)) PlaceWall(used++, new Vector2(x, y), new Vector2(x, y + 1), size);
            for (int i = used; i < walls.Count; i++) walls[i].gameObject.SetActive(false);
        }

        // A wall piece spans from one grid corner to the next and is wallThickness thick, centred on the line.
        void PlaceWall(int index, Vector2 from, Vector2 to, Vector2 size)
        {
            while (walls.Count <= index)
            {
                var go = new GameObject("Wall " + walls.Count, typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image));
                go.layer = gameObject.layer;
                go.transform.SetParent(wallLayer, false);
                var image = go.GetComponent<UnityEngine.UI.Image>();
                image.raycastTarget = false;
                walls.Add(image);
            }
            var wall = walls[index];
            wall.gameObject.SetActive(true);
            wall.color = wallColor;
            var rect = wall.rectTransform;
            rect.anchorMin = from / size;
            rect.anchorMax = to / size;
            rect.offsetMin = Vector2.one * (-wallThickness / 2f);
            rect.offsetMax = Vector2.one * (wallThickness / 2f);
        }

        static void SetCell(RectTransform rect, Vector2Int cell, Vector2 size)
        {
            rect.anchorMin = cell / size;
            rect.anchorMax = (cell + Vector2.one) / size;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        void OnWallHit()
        {
            flashTimer = hitFlashDuration;
        }
    }
}
