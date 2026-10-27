using System;
using UnityEngine;

namespace LostControl.Config
{
    /// <summary>Tunables for the Maze (BUILD_PLAN_2.md section 3). Distances are in maze cells.</summary>
    [Serializable]
    public class MazeSettings
    {
        [Tooltip("Rounds a ticket with this minigame needs before it is resolved (the designer: one maze).")]
        [Min(1)] public int rounds = 1;

        [Tooltip("Maze size (columns x rows) per round, round 1 first. The last size repeats. " +
                 "[ASSUMPTION] 8x4: the designer asked for a little longer than 6x4.")]
        public Vector2Int[] sizePerRound = { new Vector2Int(8, 4) };

        [Tooltip("[ASSUMPTION] Seconds for the pressure meter to fill from empty while a round is open.")]
        [Min(0.1f)] public float meterFillTime = 30f;

        [Tooltip("[ASSUMPTION] Speed of the dot with the joystick pushed all the way, in cells per second.")]
        [Min(0.1f)] public float dotSpeed = 2f;

        [Tooltip("[ASSUMPTION] Speed at which the dot slides back along its path when nobody steers it, in cells per second.")]
        [Min(0f)] public float driftSpeed = 0.5f;

        [Tooltip("Seconds without steering (or away from the PC) before the dot starts to slide back.")]
        [Min(0f)] public float driftDelay = 2f;

        [Tooltip("[ASSUMPTION] Radius of the dot, in cells. Touching a wall with it bounces the dot back.")]
        [Range(0.05f, 0.45f)] public float dotRadius = 0.2f;

        [Tooltip("[ASSUMPTION] How far a wall hit bounces the dot back along the path it came, in cells.")]
        [Min(0f)] public float bounceDistance = 0.5f;

        [Tooltip("[ASSUMPTION] Speed of the bounce, in cells per second. Keys do nothing until it is over.")]
        [Min(0.1f)] public float bounceSpeed = 4f;

        [Tooltip("Pellets in each maze, on the way to the exit or on a short detour (the designer: at most 3). " +
                 "[ASSUMPTION] Always this many when the maze has room.")]
        [Min(0)] public int pellets = 3;

        [Tooltip("[ASSUMPTION] How many cells off the way to the exit a pellet may lie (0 = only on the way).")]
        [Min(0)] public int pelletDetour = 2;

        [Tooltip("[ASSUMPTION] Pressure a pellet takes off the meter when the dot picks it up.")]
        [Range(0f, 1f)] public float pelletRelief = 0.15f;

        [Tooltip("[ASSUMPTION] Radius of a pellet, in cells. The dot picks it up when they touch.")]
        [Range(0.05f, 0.45f)] public float pelletRadius = 0.15f;

        /// <summary>Maze size for a round (from 0).</summary>
        public Vector2Int SizeFor(int round)
        {
            if (sizePerRound == null || sizePerRound.Length == 0) return new Vector2Int(8, 4);
            var size = sizePerRound[Mathf.Min(round, sizePerRound.Length - 1)];
            return new Vector2Int(Mathf.Max(size.x, 2), Mathf.Max(size.y, 2));
        }
    }
}
