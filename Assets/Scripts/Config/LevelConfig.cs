using System;
using System.Collections.Generic;
using LostControl.Model;
using UnityEngine;
using UnityEngine.Serialization;

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
        [Tooltip("The minigame seed: every minigame generates its rounds from it, so a playtest can be replayed exactly.")]
        [FormerlySerializedAs("randomSeed")]
        public int minigameSeed = 12345;

        [Header("Tickets")]
        [Tooltip("All tickets in this level. The level is won when every one is resolved. " +
                 "Five tickets (D69). [ASSUMPTION] Arrival times 0, 8, 20, 30, 45s. " +
                 "[ASSUMPTION] Minigame mix: off-folder, colors, packet filter, maze, SSD trimming.")]
        public List<TicketDefinition> tickets = new List<TicketDefinition>
        {
            new TicketDefinition("Weird icons in folders", 0f, MinigameType.OffFolder),
            new TicketDefinition("Virus warning shown", 8f, MinigameType.MatchColors),
            new TicketDefinition("Weird webpage on screen", 20f, MinigameType.PacketFilter),
            new TicketDefinition("Can't connect to fileserver", 30f, MinigameType.Maze),
            new TicketDefinition("Drive is full", 45f, MinigameType.SsdTrim),
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

        [Tooltip("Each time the player starts connecting to a ticket, every list entry shows its ticket's state " +
                 "at that moment once, then stays stale again.")]
        public bool refreshListOnSwitch = true;

        [Tooltip("Show the introduction before the level starts; the level waits until the player closes it.")]
        public bool showIntroduction = true;

        [Header("Damage")]
        [Tooltip("When a ticket's pressure meter fills, the PC takes one damage level and the meter drops back to this level (0-1).")]
        [Range(0f, 0.95f)] public float pressureAfterDamage = 0.5f;

        [Header("Find the off-folder")]
        public OffFolderSettings offFolder = new OffFolderSettings();

        [Header("Match the colors")]
        public MatchColorsSettings matchColors = new MatchColorsSettings();

        [Header("Packet filter")]
        public PacketFilterSettings packetFilter = new PacketFilterSettings();

        [Header("Maze")]
        public MazeSettings maze = new MazeSettings();

        [Header("SSD trimming")]
        public SsdTrimSettings ssdTrim = new SsdTrimSettings();

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

        /// <summary>Rounds a ticket with this minigame needs before it is resolved.</summary>
        public int GetRounds(MinigameType type)
        {
            switch (type)
            {
                case MinigameType.OffFolder: return offFolder.rounds;
                case MinigameType.MatchColors: return matchColors.rounds;
                case MinigameType.PacketFilter: return packetFilter.rounds;
                case MinigameType.Maze: return maze.rounds;
                case MinigameType.SsdTrim: return ssdTrim.rounds;
                default: throw new ArgumentOutOfRangeException(nameof(type), type, "No settings for this minigame.");
            }
        }

        void OnValidate()
        {
            // The off-folder needs at least one other folder to hide among.
            if (offFolder.columns * offFolder.rows < 2) offFolder.columns = 2;
            // Match the colors needs at least one pair.
            if (matchColors.columns * matchColors.rows < 2) matchColors.columns = 2;
            // The packet filter needs at least one blocked and one allowed port.
            if (packetFilter.ports == null || packetFilter.ports.Length < 2) packetFilter.ports = new PacketFilterSettings().ports;
            if (ssdTrim.overhangMax < ssdTrim.overhangMin) ssdTrim.overhangMax = ssdTrim.overhangMin;
        }
    }
}
