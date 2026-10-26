using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LostControl.View
{
    /// <summary>One tab button of the handbook. Reports clicks and shows whether its page is open.</summary>
    public class HandbookTabView : MonoBehaviour
    {
        [SerializeField] Button button;
        [SerializeField] UnityEngine.UI.Image background;
        [SerializeField] TMP_Text label;
        [SerializeField] Color normalColor = new Color32(0x38, 0x3C, 0x44, 0xFF);
        [SerializeField] Color selectedColor = new Color32(0x34, 0x4E, 0x78, 0xFF);

        public void Bind(string title, Action onClicked)
        {
            label.text = title;
            button.onClick.AddListener(() => onClicked());
        }

        public void SetSelected(bool selected)
        {
            background.color = selected ? selectedColor : normalColor;
        }
    }
}
