using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LostControl.View
{
    /// <summary>One packet on the cable. Shows its port and reports clicks; the model decides what happens.</summary>
    public class PacketView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] TMP_Text portLabel;

        public int PacketId { get; private set; }
        public event Action<int> Clicked;

        public void Bind(int packetId, int port)
        {
            PacketId = packetId;
            portLabel.text = port.ToString();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Clicked?.Invoke(PacketId);
        }
    }
}
