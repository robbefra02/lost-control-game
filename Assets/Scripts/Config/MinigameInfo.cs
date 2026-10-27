using System;
using LostControl.Model;
using UnityEngine;

namespace LostControl.Config
{
    /// <summary>How one minigame works, shown in the info box of the remote PC panel.</summary>
    [Serializable]
    public class MinigameInfo
    {
        public MinigameType minigame;

        [Tooltip("Explanation text. Supports TextMeshPro rich text such as <b>bold</b>.")]
        [TextArea(4, 20)] public string text;

        public MinigameInfo() { }

        public MinigameInfo(MinigameType minigame, string text)
        {
            this.minigame = minigame;
            this.text = text;
        }
    }
}
