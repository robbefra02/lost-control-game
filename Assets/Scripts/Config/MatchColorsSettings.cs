using System;
using UnityEngine;

namespace LostControl.Config
{
    /// <summary>Tunables for Match the colors (BUILD_PLAN_2.md section 3).</summary>
    [Serializable]
    public class MatchColorsSettings
    {
        [Tooltip("[ASSUMPTION] Rounds a ticket with this minigame needs before it is resolved.")]
        [Min(1)] public int rounds = 2;

        [Tooltip("Cards per row. Every card needs a partner: with an odd columns x rows the last cell stays empty.")]
        [Min(1)] public int columns = 4;

        [Tooltip("Rows of cards.")]
        [Min(1)] public int rows = 4;

        [Tooltip("[ASSUMPTION] Seconds for the pressure meter to fill from empty while a round is open.")]
        [Min(0.1f)] public float meterFillTime = 30f;

        [Tooltip("[ASSUMPTION] Seconds two cards that do not match stay face up before they turn back. Clicks are ignored meanwhile.")]
        [Min(0f)] public float mismatchRevealTime = 0.75f;
    }
}
