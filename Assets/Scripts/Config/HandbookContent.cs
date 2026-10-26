using System.Collections.Generic;
using UnityEngine;

namespace LostControl.Config
{
    /// <summary>
    /// The handbook's pages (BUILD_PLAN_2.md section 3). Reference only: it explains the rules, and no puzzle needs it yet.
    /// </summary>
    [CreateAssetMenu(fileName = "Handbook", menuName = "Lost Control/Handbook")]
    public class HandbookContent : ScriptableObject
    {
        [Tooltip("Pages in tab order. [ASSUMPTION] Draft text written by the coding agent from the current rules; " +
                 "the designer rewrites it.")]
        public List<HandbookPage> pages = new List<HandbookPage>
        {
            new HandbookPage("Basics",
                "<b>Your job</b>\n" +
                "Users send a ticket when their PC has a problem. Fix every PC before too many users are lost.\n\n" +
                "<b>Connecting</b>\n" +
                "Click a ticket in the list to connect to that PC. Connecting takes a few seconds, and you can only work on one PC at a time. " +
                "Disconnect with the button above the list. When you fix a PC, its session closes by itself.\n\n" +
                "<b>The list goes stale</b>\n" +
                "While you are connecting or connected, the other tickets stop updating: they are dimmed and only show what you last saw. " +
                "When a PC you can't see takes damage, you hear a beep and its ticket flashes red. Disconnect to see every ticket live again.\n\n" +
                "<b>Rounds</b>\n" +
                "Each PC is fixed by finishing its minigame, some of them a few times in a row. The list and the minigame panel show which round you are on. " +
                "Finished rounds stay finished when you leave.\n\n" +
                "<b>Pressure</b>\n" +
                "While a round is unsolved, the PC's pressure meter fills, also while you are working on another PC. " +
                "When it is full, the PC takes damage and the meter drops back to half. Finishing a round empties it.\n\n" +
                "<b>Damage</b>\n" +
                "Fine, Slightly broken, More broken, Blackout. At Blackout the user is lost and the ticket closes. " +
                "The level ends when every ticket is closed, or when too many users are lost (see Game data)."),
            new HandbookPage("Find the off-folder",
                "One folder in the grid looks slightly different from the others. Click it to finish the round.\n\n" +
                "<b>What can be different</b>\n" +
                "The file on the folder, the side of the tab, a small tilt, or the size. " +
                "In each round only one of these is different.\n\n" +
                "<b>Careful</b>\n" +
                "Clicking a folder that is not the different one adds pressure.\n\n" +
                "<b>Pressure</b>\n" +
                "The meter fills at a steady speed while the round is unsolved."),
            new HandbookPage("Match the colors",
                "All cards start face down, and every colour is on exactly two cards. Click a card to see its colour, " +
                "then click the card with the same colour. Match every pair to fix the PC.\n\n" +
                "<b>Match</b>\n" +
                "Both cards stay face up, and the pressure meter drops.\n\n" +
                "<b>No match</b>\n" +
                "Both cards show for a moment, then turn face down again. A wrong pair costs nothing extra.\n\n" +
                "<b>Leaving</b>\n" +
                "Matched pairs stay when you leave this PC. A card you picked but did not match turns face down again.\n\n" +
                "<b>Pressure</b>\n" +
                "The meter fills at a steady speed until every pair is matched. Each match pushes it back down."),
            new HandbookPage("Extinguish the firewall",
                "The firewall is on fire. Over this PC's screen your cursor becomes a hose: hold the left mouse button " +
                "to spray a thin stream of water to the right. Put out all the fire to finish the round.\n\n" +
                "<b>Water</b>\n" +
                "The stream puts out the first burning spot in its path. Move up and down to reach every row.\n\n" +
                "<b>Fire</b>\n" +
                "Fire that gets no water keeps growing, also while you are working on another PC. " +
                "Spots that burn at full strength set the spots next to them alight.\n\n" +
                "<b>Pressure</b>\n" +
                "The meter shows how much of the firewall is burning. When it is full, the PC takes damage and the fire dies down to half."),
        };
    }
}
