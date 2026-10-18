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

        [Tooltip("Kind of problem on this ticket. Level 1 has viruses only.")]
        public ThreatType threat = ThreatType.Virus;

        [Tooltip("Area of the infected component in normalized motherboard coordinates (0-1). " +
                 "[ASSUMPTION] The virus starts and spreads only inside this area.")]
        public Rect infectedArea = new Rect(0.1f, 0.1f, 0.3f, 0.3f);

        public TicketDefinition() { }

        public TicketDefinition(string description, float arrivalTime, Rect infectedArea)
        {
            this.description = description;
            this.arrivalTime = arrivalTime;
            this.infectedArea = infectedArea;
        }
    }
}
