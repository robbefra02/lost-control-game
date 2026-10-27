using UnityEngine;
using UnityEngine.EventSystems;

namespace LostControl.View
{
    /// <summary>
    /// On-screen joystick for the maze (D64). Grab it with the mouse anywhere on its base and drag in any direction;
    /// the knob follows within the base, and letting go snaps it back to the centre.
    /// </summary>
    public class JoystickView : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Tooltip("Round base; the knob moves within its radius.")]
        [SerializeField] RectTransform stickBase;
        [SerializeField] RectTransform knob;
        [SerializeField] UnityEngine.UI.Image knobImage;
        [SerializeField] Color idleColor = new Color32(0x9A, 0xA3, 0xAF, 0xFF);
        [SerializeField] Color heldColor = new Color32(0x4C, 0x8B, 0xF5, 0xFF);
        [Tooltip("[ASSUMPTION] Share of the radius (0-1) that counts as not steering, so a grab without a drag does not move the dot.")]
        [Range(0f, 0.9f)]
        [SerializeField] float deadZone = 0.15f;

        /// <summary>Steering direction; its length (0-1) is how far the knob is pushed. Zero when released.</summary>
        public Vector2 Direction { get; private set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            knobImage.color = heldColor;
            MoveKnob(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            MoveKnob(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Release();
        }

        void OnDisable()
        {
            Release();
        }

        void MoveKnob(PointerEventData eventData)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(stickBase, eventData.position, eventData.pressEventCamera, out var local)) return;
            float radius = stickBase.rect.width / 2f;
            var offset = Vector2.ClampMagnitude(local - stickBase.rect.center, radius);
            knob.anchoredPosition = offset;
            var direction = offset / radius;
            Direction = direction.magnitude < deadZone ? Vector2.zero : direction;
        }

        void Release()
        {
            Direction = Vector2.zero;
            knob.anchoredPosition = Vector2.zero;
            knobImage.color = idleColor;
        }
    }
}
