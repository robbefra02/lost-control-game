using System;
using UnityEngine;

namespace LostControl.Config
{
    /// <summary>
    /// Tunables for SSD trimming (D65, D67). Widths are fractions of the minigame's width; overhangs are measured
    /// outwards from the edge of the marked target width.
    /// </summary>
    [Serializable]
    public class SsdTrimSettings
    {
        [Tooltip("[ASSUMPTION] Rounds a ticket with this minigame needs before it is resolved.")]
        [Min(1)] public int rounds = 1;

        [Tooltip("[ASSUMPTION] Blocks in the stack.")]
        [Min(1)] public int blocks = 5;

        [Tooltip("[ASSUMPTION] Width every block has to be trimmed to, centred (the marked area).")]
        [Range(0.1f, 0.8f)] public float targetWidth = 0.4f;

        [Tooltip("[ASSUMPTION] How far a trimmed side may stick out or be cut in past the mark and still count.")]
        [Range(0.005f, 0.1f)] public float tolerance = 0.03f;

        [Tooltip("[ASSUMPTION] Shortest overhang a side starts with.")]
        [Range(0f, 0.3f)] public float overhangMin = 0.08f;

        [Tooltip("[ASSUMPTION] Longest overhang a side starts with.")]
        [Range(0f, 0.3f)] public float overhangMax = 0.22f;

        [Tooltip("Longest cutting line the player can draw, in block thicknesses (the designer asked for just over twice; " +
                 "[ASSUMPTION] 2.1).")]
        [Min(0.1f)] public float cutLineLength = 2.1f;

        [Tooltip("[ASSUMPTION] How fast every side grows back, per second, also while the player is away.")]
        [Min(0f)] public float regrowPerSecond = 0.0015f;

        [Tooltip("Seconds for the pressure meter to fill from empty while a round is open (the designer asked for " +
                 "forty-something, then 5 s more).")]
        [Min(0.1f)] public float meterFillTime = 47f;
    }
}
