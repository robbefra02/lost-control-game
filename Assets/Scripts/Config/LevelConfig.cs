using System.Collections.Generic;
using LostControl.Model;
using UnityEngine;

namespace LostControl.Config
{
    /// <summary>
    /// Every tunable value for one level (BUILD_PLAN.md section 3). Gameplay code reads its numbers from here only.
    /// Tooltips marked [ASSUMPTION] are defaults the designer has not confirmed yet.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Lost Control/Level Config")]
    public class LevelConfig : ScriptableObject
    {
        [Header("Randomness")]
        [Tooltip("Seed for all randomness in the level, so a playtest can be replayed exactly.")]
        public int randomSeed = 12345;

        [Header("Tickets")]
        [Tooltip("All tickets in this level. The level is won when every one is resolved. " +
                 "[ASSUMPTION] Arrival times 0, 8, 20, 30, 45, 55s; descriptions and infected areas are placeholders.")]
        public List<TicketDefinition> tickets = new List<TicketDefinition>
        {
            new TicketDefinition("PC acts strange", 0f, new Rect(0.05f, 0.55f, 0.30f, 0.35f)),
            new TicketDefinition("Virus warning shown", 8f, new Rect(0.60f, 0.60f, 0.30f, 0.30f)),
            new TicketDefinition("Computer is really slow", 20f, new Rect(0.35f, 0.10f, 0.30f, 0.30f)),
            new TicketDefinition("Pop-ups everywhere", 30f, new Rect(0.05f, 0.10f, 0.25f, 0.35f)),
            new TicketDefinition("Files won't open", 45f, new Rect(0.65f, 0.10f, 0.30f, 0.35f)),
            new TicketDefinition("Fan is very loud", 55f, new Rect(0.38f, 0.50f, 0.25f, 0.30f)),
        };

        [Header("Connecting")]
        [Tooltip("[ASSUMPTION] Seconds of connection progress before the ticket can be worked on.")]
        [Min(0f)] public float connectionTime = 2.5f;

        [Tooltip("[ASSUMPTION] Audio ping + list entry flash when a stale ticket gains a damage level. " +
                 "Not confirmed by the designer yet.")]
        public bool hiddenDamageCue = true;

        [Tooltip("[ASSUMPTION] Off: a ticket's list entry only updates while that ticket is connected, " +
                 "so the list is also stale before the first connection and after a resolve. " +
                 "On: with no session at all, every entry shows live state.")]
        public bool liveListWhenDisconnected = false;

        [Header("Virus")]
        [Tooltip("Fewest dots a virus starts with.")]
        [Min(1)] public int startDotsMin = 8;

        [Tooltip("Most dots a virus starts with.")]
        [Min(1)] public int startDotsMax = 9;

        [Tooltip("Seconds between spreads. Each spread, every dot copies itself to a nearby position.")]
        [Min(0.1f)] public float spreadInterval = 2f;

        [Tooltip("[ASSUMPTION] Max distance between a dot and its copy, in normalized motherboard units.")]
        [Min(0f)] public float spreadDistance = 0.05f;

        [Tooltip("[ASSUMPTION] Max dots per component. Spreading stops here and the damage countdown starts.")]
        [Min(1)] public int dotCap = 30;

        [Tooltip("[ASSUMPTION] Dot size on screen, in canvas pixels at 1920x1080. Bigger dots are easier to click.")]
        [Min(1f)] public float dotDiameter = 28f;

        [Header("Damage")]
        [Tooltip("Seconds continuously at cap for +1 damage level.")]
        [Min(0.1f)] public float damageInterval = 20f;

        [Tooltip("[ASSUMPTION] Pause by default. Reset allows clicking one dot every interval to block all damage.")]
        public CountdownBelowCap countdownBelowCap = CountdownBelowCap.Pause;

        [Header("Economy")]
        [Tooltip("Cash paid when a ticket is resolved at Fine.")]
        [Min(0)] public int payoutFine = 100;

        [Tooltip("Cash paid when a ticket is resolved at Slightly broken.")]
        [Min(0)] public int payoutSlightlyBroken = 75;

        [Tooltip("Cash paid when a ticket is resolved at More broken.")]
        [Min(0)] public int payoutMoreBroken = 50;

        [Tooltip("Cash paid when a ticket is resolved at Blackout.")]
        [Min(0)] public int payoutBlackout = 0;

        [Tooltip("Virus containers the player starts the level with.")]
        [Min(0)] public int startingVirusContainers = 2;

        [Tooltip("Seconds a virus container pauses the spread and damage countdown of one virus.")]
        [Min(0f)] public float virusContainerDuration = 10f;

        [Tooltip("[ASSUMPTION] Shop available during the level. Without it, cash has no use in level 1.")]
        public bool shopEnabled = true;

        [Tooltip("Shop price of one extra virus container.")]
        [Min(0)] public int virusContainerPrice = 100;

        [Header("Win / lose")]
        [Tooltip("Tickets at Blackout (users lost) that end the level in a loss.")]
        [Min(1)] public int lostUsersToLose = 2;

        /// <summary>Cash paid for resolving a ticket at the given damage level.</summary>
        public int GetPayout(DamageLevel level)
        {
            switch (level)
            {
                case DamageLevel.Fine: return payoutFine;
                case DamageLevel.SlightlyBroken: return payoutSlightlyBroken;
                case DamageLevel.MoreBroken: return payoutMoreBroken;
                default: return payoutBlackout;
            }
        }

        void OnValidate()
        {
            if (startDotsMax < startDotsMin) startDotsMax = startDotsMin;
            if (dotCap < startDotsMax) dotCap = startDotsMax;
        }
    }
}
