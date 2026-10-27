using System.Collections.Generic;
using LostControl.Model;
using UnityEngine;

namespace LostControl.Config
{
    /// <summary>
    /// The texts that explain the game (D62, D63): the introduction shown before the level starts and the
    /// explanation of each minigame in the remote PC panel's info box. They replace the handbook.
    /// </summary>
    [CreateAssetMenu(fileName = "InfoTexts", menuName = "Lost Control/Info Texts")]
    public class InfoTexts : ScriptableObject
    {
        [Tooltip("Title of the introduction screen. [ASSUMPTION] Placeholder text.")]
        public string introductionTitle = "Your shift starts";

        [Tooltip("Introduction shown before the level starts. Supports TextMeshPro rich text. " +
                 "[ASSUMPTION] Draft text by the coding agent, based on the handbook's Basics page; the designer rewrites it.")]
        [TextArea(8, 40)] public string introduction =
            "<b>Your job</b>\n" +
            "Users send a ticket when their PC has a problem. Fix every PC before too many users are lost.\n\n" +
            "<b>Connecting</b>\n" +
            "Click a ticket in the list to connect to that PC. Connecting takes a few seconds, and you can only work on one PC at a time. " +
            "Disconnect with the button above the list. When you fix a PC, its session closes by itself.\n\n" +
            "<b>The list goes stale</b>\n" +
            "While you are connected, the other tickets stop updating: they are dimmed and show how they were when you last switched. " +
            "When a PC you can't see takes damage, you hear a beep and its ticket flashes red.\n\n" +
            "<b>Pressure and damage</b>\n" +
            "Every PC has a pressure meter that fills while its problem is unsolved, also while you are working on another PC. " +
            "When it is full, the PC takes damage: Fine, Slightly broken, More broken, Blackout. At Blackout the user is lost.\n\n" +
            "<b>How each PC is fixed</b>\n" +
            "The 'i' button on the remote PC explains the minigame of the PC you are connected to.";

        [Tooltip("Explanation per minigame. [ASSUMPTION] Draft texts by the coding agent; the designer rewrites them.")]
        public List<MinigameInfo> minigames = new List<MinigameInfo>
        {
            new MinigameInfo(MinigameType.OffFolder,
                "One folder looks slightly different from the others: its file, the side of its tab, a small tilt or its size. " +
                "Click it to finish the round.\n\n" +
                "<b>Careful:</b> clicking a wrong folder adds pressure.\n\n" +
                "The meter fills at a steady speed until the round is done."),
            new MinigameInfo(MinigameType.MatchColors,
                "All cards are face down, and every colour is on two cards. Click a card, then the card with the same colour. " +
                "Match every pair to fix the PC.\n\n" +
                "<b>Match:</b> both cards stay up and the pressure drops.\n" +
                "<b>No match:</b> both turn back after a moment.\n\n" +
                "Matched pairs stay when you leave."),
            new MinigameInfo(MinigameType.PacketFilter,
                "Packets travel along the cable into the PC. Block the packets with the port named above the cable by clicking them, " +
                "and let the others through.\n\n" +
                "<b>Each block</b> lowers the pressure; enough blocks fix the PC.\n" +
                "<b>Blocking a good packet</b> adds pressure.\n" +
                "<b>Every bad packet that gets in</b> adds pressure, also while you are away."),
            new MinigameInfo(MinigameType.Maze,
                "Grab the joystick and drag it to steer the dot from the start (bottom left) to the green exit (top right).\n\n" +
                "<b>Yellow pellets</b> on the way take pressure off when the dot picks them up.\n" +
                "<b>Walls</b> bounce the dot back a little.\n" +
                "<b>Let go</b> or leave this PC for a moment, and the dot slowly slides back the way it came.\n\n" +
                "The meter fills at a steady speed until the round is done."),
            new MinigameInfo(MinigameType.SsdTrim,
                "The blocks stick out on both sides. Hold the mouse button and draw a short cutting line across a part that " +
                "sticks out: the blocks are cut where the line crosses them. A line reaches two blocks at most. " +
                "Trim every side to the green marks.\n\n" +
                "<b>Cut too far</b> and you have to wait for that side to grow back into the mark.\n" +
                "<b>The blocks</b> slowly grow back, also while you are away.\n\n" +
                "The meter fills at a steady speed until every side is trimmed."),
        };

        /// <summary>The explanation for a minigame, or an empty string when there is none.</summary>
        public string For(MinigameType minigame)
        {
            foreach (var info in minigames)
                if (info.minigame == minigame) return info.text;
            return "";
        }
    }
}
