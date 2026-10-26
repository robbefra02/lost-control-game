using System;
using UnityEngine;

namespace LostControl.Config
{
    /// <summary>Tunables for Extinguish the firewall (BUILD_PLAN_2.md section 3).</summary>
    [Serializable]
    public class FirewallSettings
    {
        [Tooltip("[ASSUMPTION] Rounds a ticket with this minigame needs before it is resolved.")]
        [Min(1)] public int rounds = 2;

        [Tooltip("[ASSUMPTION] Area of the firewall in normalized board coordinates (0-1, origin bottom-left). The fire burns inside it.")]
        public Rect wallArea = new Rect(0.62f, 0.05f, 0.33f, 0.9f);

        [Tooltip("[ASSUMPTION] Rows of fire spots on the wall.")]
        [Min(1)] public int rows = 8;

        [Tooltip("[ASSUMPTION] Columns of fire spots on the wall.")]
        [Min(1)] public int columns = 3;

        [Tooltip("[ASSUMPTION] Fewest spots burning when a round starts.")]
        [Min(1)] public int startFiresMin = 3;

        [Tooltip("[ASSUMPTION] Most spots burning when a round starts.")]
        [Min(1)] public int startFiresMax = 5;

        [Tooltip("[ASSUMPTION] Intensity (0-1) of the spots burning when a round starts.")]
        [Range(0.01f, 1f)] public float startIntensity = 0.5f;

        [Tooltip("[ASSUMPTION] Intensity a burning spot gains per second while no water hits it, also while the player is away.")]
        [Min(0f)] public float growthPerSecond = 0.05f;

        [Tooltip("[ASSUMPTION] Seconds a spot at full intensity needs to set one neighbouring spot alight.")]
        [Min(0.1f)] public float spreadDelay = 4f;

        [Tooltip("[ASSUMPTION] Intensity of a spot the fire just spread to.")]
        [Range(0.01f, 1f)] public float spreadIntensity = 0.2f;

        [Tooltip("[ASSUMPTION] Intensity the water removes per second from a spot it hits.")]
        [Min(0f)] public float waterPerSecond = 0.6f;

        [Tooltip("[ASSUMPTION] Height of the water stream in normalized board units. One spot is wall height / rows high.")]
        [Range(0.005f, 0.5f)] public float streamHeight = 0.04f;
    }
}
