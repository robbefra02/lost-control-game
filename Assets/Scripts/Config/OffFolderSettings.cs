using System;
using LostControl.Model;
using UnityEngine;

namespace LostControl.Config
{
    /// <summary>Tunables for Find the off-folder (BUILD_PLAN_2.md section 3).</summary>
    [Serializable]
    public class OffFolderSettings
    {
        [Tooltip("[ASSUMPTION] Rounds a ticket with this minigame needs before it is resolved.")]
        [Min(1)] public int rounds = 3;

        [Tooltip("Folders per row.")]
        [Min(1)] public int columns = 6;

        [Tooltip("Rows of folders.")]
        [Min(1)] public int rows = 6;

        [Tooltip("[ASSUMPTION] Seconds for the pressure meter to fill from empty while a round is open.")]
        [Min(0.1f)] public float meterFillTime = 30f;

        [Tooltip("[ASSUMPTION] Pressure added by clicking a folder that is not the off-folder (1 = the whole meter).")]
        [Range(0f, 1f)] public float wrongClickPressure = 0.2f;

        [Tooltip("[ASSUMPTION] Ways the off-folder can differ. Each round picks one at random from this list.")]
        public FolderDifference[] differences =
        {
            FolderDifference.Glyph,
            FolderDifference.TabSide,
            FolderDifference.Shade,
            FolderDifference.Rotation,
            FolderDifference.Size,
        };

        [Tooltip("[ASSUMPTION] How much lighter or darker the off-folder is (0-1).")]
        [Range(0f, 1f)] public float shadeDifference = 0.15f;

        [Tooltip("[ASSUMPTION] How far the off-folder is tilted, in degrees.")]
        [Range(0f, 45f)] public float rotationDifference = 10f;

        [Tooltip("[ASSUMPTION] How much bigger or smaller the off-folder is (0.1 = 10%).")]
        [Range(0f, 0.5f)] public float sizeDifference = 0.12f;

        [Tooltip("[ASSUMPTION] Multiplier on the shade, rotation and size differences per round, round 1 first. " +
                 "The last value repeats. All 1 = no difficulty ramp.")]
        public float[] differenceScalePerRound = { 1f };

        /// <summary>Multiplier on the continuous differences for a round (from 0).</summary>
        public float DifferenceScale(int round)
        {
            if (differenceScalePerRound == null || differenceScalePerRound.Length == 0) return 1f;
            return differenceScalePerRound[Mathf.Min(round, differenceScalePerRound.Length - 1)];
        }
    }
}
