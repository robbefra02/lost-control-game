using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LostControl.View
{
    /// <summary>One block of the SSD stack. Reports where the mouse went down and when it came up; the view decides the side.</summary>
    public class SsdBlockView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public int Index { get; set; }

        /// <summary>Block index and the screen position of the press.</summary>
        public event Action<int, Vector2> Pressed;
        public event Action Released;

        public void OnPointerDown(PointerEventData eventData)
        {
            Pressed?.Invoke(Index, eventData.position);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Released?.Invoke();
        }
    }
}
