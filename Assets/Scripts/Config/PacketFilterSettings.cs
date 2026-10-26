using System;
using UnityEngine;

namespace LostControl.Config
{
    /// <summary>Tunables for the Packet filter (BUILD_PLAN_2.md section 3, D53).</summary>
    [Serializable]
    public class PacketFilterSettings
    {
        [Tooltip("[ASSUMPTION] Rounds a ticket with this minigame needs before it is resolved.")]
        [Min(1)] public int rounds = 2;

        [Tooltip("[ASSUMPTION] Port numbers packets can carry. Each round's rule blocks some of them.")]
        public int[] ports = { 20, 21, 22, 23, 25, 53, 80, 110, 143, 443, 3389 };

        [Tooltip("[ASSUMPTION] How many ports the rule blocks, per round (round 1 first). The last value repeats.")]
        public int[] blockedPortsPerRound = { 1 };

        [Tooltip("[ASSUMPTION] Bad packets the player must block to finish a round. Packets that leak through do not count.")]
        [Min(1)] public int blocksPerRound = 4;

        [Tooltip("[ASSUMPTION] Seconds between two packets entering the cable. The stream never speeds up.")]
        [Min(0.1f)] public float spawnInterval = 1.5f;

        [Tooltip("[ASSUMPTION] Chance (0-1) that a new packet carries a blocked port.")]
        [Range(0f, 1f)] public float badChance = 0.35f;

        [Tooltip("[ASSUMPTION] Seconds a packet needs to travel the cable into the PC.")]
        [Min(0.1f)] public float travelTime = 4f;

        [Tooltip("[ASSUMPTION] Pressure added when a bad packet reaches the PC (1 = the whole meter).")]
        [Range(0f, 1f)] public float leakPressure = 0.15f;

        [Tooltip("[ASSUMPTION] Pressure added when the player blocks a good packet (1 = the whole meter).")]
        [Range(0f, 1f)] public float wrongBlockPressure = 0.2f;

        /// <summary>Number of blocked ports for a round (from 0), at least 1 and fewer than all ports.</summary>
        public int BlockedPortCount(int round)
        {
            int count = blockedPortsPerRound == null || blockedPortsPerRound.Length == 0
                ? 1
                : blockedPortsPerRound[Mathf.Min(round, blockedPortsPerRound.Length - 1)];
            return Mathf.Clamp(count, 1, Mathf.Max(ports.Length - 1, 1));
        }
    }
}
