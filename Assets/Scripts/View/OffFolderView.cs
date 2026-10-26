using System.Collections.Generic;
using LostControl.Model;
using UnityEngine;
using UnityEngine.UI;

namespace LostControl.View
{
    /// <summary>
    /// Draws Find the off-folder as a grid of folder icons and passes folder clicks to the model.
    /// The icons are redrawn whenever the model starts a new round; the grid hides once no round is open.
    /// </summary>
    public class OffFolderView : MinigameView
    {
        [SerializeField] GridLayoutGroup grid;
        [SerializeField] FolderIconView iconPrefab;

        readonly List<FolderIconView> icons = new List<FolderIconView>();
        OffFolderModel model;
        int shownRound = -1;

        public override MinigameType Type => MinigameType.OffFolder;

        public override void Show(MinigameModel minigame)
        {
            if (model != null) model.WrongClick -= OnWrongClick;
            model = minigame as OffFolderModel;
            shownRound = -1;
            if (model == null) return;

            model.WrongClick += OnWrongClick;
            while (icons.Count < model.FolderCount) icons.Add(CreateIcon(icons.Count));
            for (int i = 0; i < icons.Count; i++) icons[i].gameObject.SetActive(i < model.FolderCount);
        }

        void LateUpdate()
        {
            if (model == null) return;

            grid.gameObject.SetActive(model.IsRoundOpen);
            GridCells.FitSquare(grid, model.Columns, model.Rows);
            if (model.Round == shownRound) return;
            shownRound = model.Round;
            for (int i = 0; i < model.FolderCount; i++) icons[i].SetLook(model.GetLook(i));
        }

        FolderIconView CreateIcon(int index)
        {
            var icon = Instantiate(iconPrefab, grid.transform);
            icon.name = "Folder " + index;
            icon.Index = index;
            icon.Clicked += OnIconClicked;
            return icon;
        }

        void OnIconClicked(int index)
        {
            model?.ClickFolder(index);
        }

        void OnWrongClick(int index)
        {
            icons[index].Flash();
        }
    }
}
