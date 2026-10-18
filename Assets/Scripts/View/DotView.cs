using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LostControl.View
{
    /// <summary>One clickable virus dot. Reports clicks; the model decides what happens.</summary>
    public class DotView : MonoBehaviour, IPointerClickHandler
    {
        public int DotId { get; set; }
        public event Action<int> Clicked;

        public void OnPointerClick(PointerEventData eventData)
        {
            Clicked?.Invoke(DotId);
        }
    }
}
