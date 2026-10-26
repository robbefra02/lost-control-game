using UnityEngine;
using UnityEngine.UI;

namespace LostControl.View
{
    /// <summary>Sizes a minigame's grid layout to square cells, as large as the grid's area allows.</summary>
    public static class GridCells
    {
        /// <summary>
        /// Fits columns x rows square cells into the grid's rect. Cheap enough to call every frame, so the grid
        /// follows window resizes; the layout only rebuilds when the size actually changes.
        /// </summary>
        public static void FitSquare(GridLayoutGroup grid, int columns, int rows)
        {
            var size = ((RectTransform)grid.transform).rect.size;
            var spacing = grid.spacing;
            float width = (size.x - spacing.x * (columns - 1)) / columns;
            float height = (size.y - spacing.y * (rows - 1)) / rows;
            float cell = Mathf.Max(Mathf.Min(width, height), 1f);
            grid.cellSize = new Vector2(cell, cell);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columns;
        }
    }
}
