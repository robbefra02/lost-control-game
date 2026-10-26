using System.Collections.Generic;
using LostControl.Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LostControl.View
{
    /// <summary>
    /// The handbook panel: one tab per page, the open page below it. It is separate from the remote session,
    /// so it can be read at any time, also while a connection is loading.
    /// </summary>
    public class HandbookView : MonoBehaviour
    {
        [SerializeField] HandbookContent content;
        [Tooltip("Row the tab buttons are created in, left to right.")]
        [SerializeField] RectTransform tabRow;
        [SerializeField] HandbookTabView tabPrefab;
        [SerializeField] TMP_Text pageText;
        [SerializeField] ScrollRect scroll;

        readonly List<HandbookTabView> tabs = new List<HandbookTabView>();

        /// <summary>Index of the open page, or -1 before the first one opens.</summary>
        public int ShownPage { get; private set; } = -1;

        void Start()
        {
            for (int i = 0; i < content.pages.Count; i++)
            {
                var tab = Instantiate(tabPrefab, tabRow);
                tab.name = "Tab " + content.pages[i].title;
                int page = i;
                tab.Bind(content.pages[i].title, () => ShowPage(page));
                tabs.Add(tab);
            }
            if (tabs.Count > 0) ShowPage(0);
        }

        /// <summary>Opens a page, scrolled to its top.</summary>
        public void ShowPage(int index)
        {
            ShownPage = index;
            pageText.text = content.pages[index].body;
            for (int i = 0; i < tabs.Count; i++) tabs[i].SetSelected(i == index);
            scroll.StopMovement();
            scroll.content.anchoredPosition = Vector2.zero;
        }
    }
}
