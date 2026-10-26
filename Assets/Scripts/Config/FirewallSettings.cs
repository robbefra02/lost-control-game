using System;
using UnityEngine;

namespace LostControl.Config
{
    /// <summary>Tunables for Extinguish the firewall (BUILD_PLAN_2.md section 3, D52). Areas are normalized board coordinates (0-1, origin bottom-left).</summary>
    [Serializable]
    public class FirewallSettings
    {
        [Tooltip("[ASSUMPTION] Rounds a ticket with this minigame needs before it is resolved.")]
        [Min(1)] public int rounds = 2;

        [Header("Area")]
        [Tooltip("[ASSUMPTION] The firewall: a wall on the right of the fire. Water that reaches it is gone.")]
        public Rect wallArea = new Rect(0.80f, 0f, 0.05f, 1f);

        [Tooltip("[ASSUMPTION] Where fire can burn: in front of the wall, from the floor up.")]
        public Rect fireArea = new Rect(0.45f, 0f, 0.35f, 0.9f);

        [Tooltip("[ASSUMPTION] Columns of fire spots, counted away from the wall.")]
        [Min(1)] public int columns = 7;

        [Tooltip("[ASSUMPTION] Rows of fire spots, counted up from the floor.")]
        [Min(1)] public int rows = 8;

        [Header("Fire")]
        [Tooltip("[ASSUMPTION] Fewest spots burning at the foot of the wall when a round starts.")]
        [Min(1)] public int startFiresMin = 1;

        [Tooltip("[ASSUMPTION] Most spots burning at the foot of the wall when a round starts.")]
        [Min(1)] public int startFiresMax = 2;

        [Tooltip("[ASSUMPTION] Intensity (0-1) of the starting spots.")]
        [Range(0.01f, 1f)] public float startIntensity = 0.5f;

        [Tooltip("[ASSUMPTION] Intensity a burning spot gains per second while no water hits it, also while the player is away.")]
        [Min(0f)] public float growthPerSecond = 0.1f;

        [Tooltip("[ASSUMPTION] Seconds a spot at full intensity needs to set a neighbour alight (above, beside or below it).")]
        [Min(0.1f)] public float spreadDelay = 2f;

        [Tooltip("[ASSUMPTION] Intensity of a spot the fire just spread to.")]
        [Range(0.01f, 1f)] public float spreadIntensity = 0.2f;

        [Header("Damage")]
        [Tooltip("[ASSUMPTION] Fire size (0-1: the whole area burning at full strength) above which the PC takes damage. " +
                 "The pressure meter shows the fire size up to this threshold.")]
        [Range(0.01f, 1f)] public float damageThreshold = 0.3f;

        [Tooltip("Seconds above the threshold per damage level. The time counts on when the fire dips below it.")]
        [Min(0.1f)] public float damageInterval = 4f;

        [Header("Water")]
        [Tooltip("[ASSUMPTION] Drops the hose fires per second while the button is held.")]
        [Min(1f)] public float dropsPerSecond = 40f;

        [Tooltip("[ASSUMPTION] Sideways speed of a drop, in board widths per second.")]
        [Min(0.01f)] public float dropSpeed = 0.9f;

        [Tooltip("[ASSUMPTION] Gravity on a drop, in board heights per second squared.")]
        [Min(0f)] public float gravity = 2f;

        [Tooltip("[ASSUMPTION] Intensity one drop takes from the burning spot it hits.")]
        [Range(0.001f, 1f)] public float waterPerDrop = 0.03f;
    }
}
