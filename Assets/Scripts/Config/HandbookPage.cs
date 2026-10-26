using System;
using UnityEngine;

namespace LostControl.Config
{
    /// <summary>One page of the handbook, shown under its own tab.</summary>
    [Serializable]
    public class HandbookPage
    {
        [Tooltip("Tab label.")]
        public string title;

        [Tooltip("Page text. Supports TextMeshPro rich text such as <b>bold</b>.")]
        [TextArea(6, 30)] public string body;

        public HandbookPage() { }

        public HandbookPage(string title, string body)
        {
            this.title = title;
            this.body = body;
        }
    }
}
