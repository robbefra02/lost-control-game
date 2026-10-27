using UnityEngine;
using UnityEngine.EventSystems;

namespace LostControl.View
{
    /// <summary>One arrow of the maze's d-pad. Held while the mouse button is down on it; letting go or leaving it releases it.</summary>
    public class DPadButtonView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [Tooltip("Direction the dot moves while this arrow is held.")]
        [SerializeField] Vector2 direction = Vector2.up;
        [SerializeField] UnityEngine.UI.Image background;
        [SerializeField] Color normalColor = new Color32(0x38, 0x3C, 0x44, 0xFF);
        [SerializeField] Color heldColor = new Color32(0x34, 0x4E, 0x78, 0xFF);

        public Vector2 Direction => direction;
        public bool IsHeld { get; private set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            SetHeld(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            SetHeld(false);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetHeld(false);
        }

        void OnDisable()
        {
            SetHeld(false);
        }

        void SetHeld(bool held)
        {
            IsHeld = held;
            background.color = held ? heldColor : normalColor;
        }
    }
}
