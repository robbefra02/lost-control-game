using System;
using LostControl.Model;
using UnityEngine;

namespace LostControl.Config
{
    /// <summary>One ticket in a level: one user's PC with one problem.</summary>
    [Serializable]
    public class TicketDefinition
    {
        [Tooltip("Short issue description shown in the ticket list. [ASSUMPTION] Placeholder text.")]
        public string description = "PC acts strange";

        [Tooltip("Seconds after level start that this ticket arrives.")]
        [Min(0f)] public float arrivalTime;

        [Tooltip("The minigame the player plays to fix this PC.")]
        public MinigameType minigame = MinigameType.OffFolder;

        public TicketDefinition() { }

        public TicketDefinition(string description, float arrivalTime, MinigameType minigame)
        {
            this.description = description;
            this.arrivalTime = arrivalTime;
            this.minigame = minigame;
        }
    }
}
