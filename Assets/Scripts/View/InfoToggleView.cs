using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LostControl.View
{
    /// <summary>
    /// The 'i' icon that opens and closes the minigame info box. Grey while the info is closed, normal while it is
    /// open, and bright while the pointer is over it. Reports clicks; the panel decides what happens.
    /// </summary>
    public class InfoToggleView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] UnityEngine.UI.Image icon;
        [SerializeField] Color openColor = new Color32(0xC8, 0xCC, 0xD4, 0xFF);
        [SerializeField] Color closedColor = new Color32(0x5C, 0x62, 0x6D, 0xFF);
        [SerializeField] Color hoverColor = new Color32(0xFF, 0xFF, 0xFF, 0xFF);

        bool open = true;
        bool hovered;

        public event Action Clicked;

        public void SetOpen(bool value)
        {
            open = value;
            UpdateColor();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Clicked?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            hovered = true;
            UpdateColor();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hovered = false;
            UpdateColor();
        }

        void OnDisable()
        {
            hovered = false;
            UpdateColor();
        }

        void UpdateColor()
        {
            icon.color = hovered ? hoverColor : open ? openColor : closedColor;
        }
    }
}
