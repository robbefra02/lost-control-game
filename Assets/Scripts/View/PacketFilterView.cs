using System.Collections.Generic;
using LostControl.Model;
using TMPro;
using UnityEngine;

namespace LostControl.View
{
    /// <summary>
    /// Draws the Packet filter: the round's rule, the cable with its packets and the PC they flow into, and passes
    /// packet clicks to the model. Packet views are synced to the model's packets by id; the PC flashes on a leak.
    /// </summary>
    public class PacketFilterView : MinigameView
    {
        [Tooltip("Packets are placed along this rect, from its left edge (entering) to its right edge (at the PC).")]
        [SerializeField] RectTransform cable;
        [SerializeField] PacketView packetPrefab;
        [SerializeField] TMP_Text ruleLabel;
        [SerializeField] TMP_Text progressLabel;
        [Tooltip("The PC at the end of the cable; flashes when a bad packet gets in.")]
        [SerializeField] UnityEngine.UI.Image pc;
        [SerializeField] Color pcColor = new Color32(0x38, 0x3C, 0x44, 0xFF);
        [SerializeField] Color leakColor = new Color32(0xE5, 0x48, 0x4D, 0xFF);
        [SerializeField] float leakFlashDuration = 0.5f;

        readonly Dictionary<int, PacketView> packetViews = new Dictionary<int, PacketView>();
        readonly HashSet<int> liveIds = new HashSet<int>();
        readonly List<int> goneIds = new List<int>();
        PacketFilterModel model;
        float flashTimer;

        public override MinigameType Type => MinigameType.PacketFilter;

        public override void Show(MinigameModel minigame)
        {
            if (model != null) model.Leaked -= OnLeaked;
            model = minigame as PacketFilterModel;
            foreach (var view in packetViews.Values) Destroy(view.gameObject);
            packetViews.Clear();
            flashTimer = 0f;
            if (model == null) return;
            model.Leaked += OnLeaked;
        }

        void LateUpdate()
        {
            if (model == null) return;

            var ports = model.BlockedPorts;
            ruleLabel.text = ports.Count == 1 ? $"Block port {ports[0]}" : $"Block ports {string.Join(", ", ports)}";
            progressLabel.text = $"Blocked {model.BlockedCount} / {model.BlocksNeeded}";
            SyncPackets();

            flashTimer = Mathf.Max(flashTimer - Time.deltaTime, 0f);
            pc.color = Color.Lerp(pcColor, leakColor, leakFlashDuration > 0f ? flashTimer / leakFlashDuration : 0f);
        }

        void SyncPackets()
        {
            liveIds.Clear();
            foreach (var packet in model.Packets)
            {
                liveIds.Add(packet.id);
                if (!packetViews.TryGetValue(packet.id, out var view))
                {
                    view = Instantiate(packetPrefab, cable);
                    view.name = "Packet " + packet.id;
                    view.Bind(packet.id, packet.port);
                    view.Clicked += OnPacketClicked;
                    packetViews.Add(packet.id, view);
                }
                var rect = (RectTransform)view.transform;
                rect.anchorMin = new Vector2(packet.progress, 0.5f);
                rect.anchorMax = new Vector2(packet.progress, 0.5f);
                rect.anchoredPosition = Vector2.zero;
            }

            goneIds.Clear();
            foreach (var id in packetViews.Keys)
                if (!liveIds.Contains(id)) goneIds.Add(id);
            foreach (var id in goneIds)
            {
                Destroy(packetViews[id].gameObject);
                packetViews.Remove(id);
            }
        }

        void OnPacketClicked(int id)
        {
            model?.ClickPacket(id);
        }

        void OnLeaked(int port)
        {
            flashTimer = leakFlashDuration;
        }
    }
}
