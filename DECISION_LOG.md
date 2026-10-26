# Lost Control (working title) - Decision Log

Design Portfolio material for KdG MCT2, 2.1 Principles of Game Design.
Format per entry: decision, alternative(s), reason, trade-off. Where the reason was not stated during the discussion, it says **reason to be written by designer**. Fill those in yourself: you will be asked to defend them in the exam interview.

Dates are when the decision was recorded.

Recreated on 2026-10-22 from the original in `G:\My Drive\Courses\2.1 Principles of game design\Game\DECISION_LOG.md`. D01-D25 are copied unchanged; lines marked **Update** were added later. D26 onward record the decisions made while building the prototype. Commit hashes refer to github.com/robbefra02/lost-control-game.

---

## 2026-09-27 - Initial concept (pre Milestone 1)

**D01 - Theme: player is shrunk into a computer and defends it from hackers, viruses and malware.**
- Planned content: breaking motherboard traces, relocating bits with a backup image, defending the RJ45 port, repairing the firewall, shooting wifi viruses.
- Game over when the PC takes too much damage.
- Feedback received: this described theme and features, not a player experience. The five scenarios were five different games (scope). The theme alone is not distinct from existing games (Tron, ReBoot, Mega Man Battle Network). The interesting core was identified as **triage**: more problems than one player can handle at once.

## 2026-10-08 - Milestone 1 (Base Concept)

**D02 - Design claim built on triage.**
- The player has to constantly decide which system to save and which to sacrifice. The game raises slightly more threats than the player can handle.
- Alternative: a theme-led concept (D01).
- Reason: the scenarios in D01 had no shared decision; triage gives every scenario the same core choice.

**D03 - Core mechanics: spreading threats, travel cost, system damage, upgrades.**
- Trace-cutting (D01) dropped as a mechanic.
- Store-bought health regeneration was proposed, then challenged: buying health back makes sacrifices free and undermines the claim. Resolved in D15.

**D04 - Constraint: threats communicated only through audio/visual cues, no text.**
- Replaced in D10.

## 2026-10-18 - Toward Milestone 2

**D05 - Singleplayer first.**
- Alternatives: co-op, versus.
- Reason: singleplayer is the MVP; multiplayer can be added later if wanted.
- Trade-off: no social dynamics in the prototype.

**D06 - 2D interface instead of 3D.**
- Alternative: 3D walkable PC interior (would use Blender skills).
- Reason: 2D gives a full overview of tasks, fits the servicedesk feeling (main screen is a ticket queue), and 3D assets would take too long.
- Trade-off: lose spatial travel and the physical "inside the PC" presence; UI design becomes more important (an area of less experience/interest).

**D07 - Fantasy shift: player is a servicedesk agent handling tickets from many PCs, not a tiny defender inside one PC.**
- Follows from D06.
- Consequence: damage and game over are per-user PC instead of one shared PC (see D17).

**D08 - Layout: ticket list + connected ticket's PC screen (top right) + motherboard view (bottom right), with a game data column.**
- Alternatives: pure ticket queue; motherboard map with a moving technician token.
- Reason: middle option keeps the queue feeling and still shows where the problem is physically.

**D09 - Connection time replaces travel time.**
- Alternative: switching cost with no wait; one locked session.
- Reason: keeps the opportunity cost of moving between problems in a 2D interface.
- Trade-off: risk of wasteful friction if too long (to be tuned).
- **Update 2026-10-21:** set to 3 s (D33).

**D10 - While connected to one ticket, the other tickets do not update on screen (stale view). Replaces the no-text constraint (D04).**
- Alternative: live overview of all tickets.
- Reason: with live information the best order is always visible and the choice becomes nominal. Hiding it makes checking a decision with a cost (reconnecting).
- Trade-off: players can lose a ticket they never saw degrade, which can feel unfair. Mitigation proposed: audio cue when a hidden ticket gains a damage level (pending confirmation).
- **Update 2026-10-19:** mitigation built as a placeholder beep plus a red flash on that ticket's list entry (D28). Kept by the designer in the Milestone 2 text.

**D11 - Text is allowed in tickets.**
- Reason: a ticket system without text does not feel like a ticket system.
- Planned for later levels: ticket descriptions are sometimes vague or wrong, as in real company ticket logging.

**D12 - Level 1 contains only the virus threat. No minigames; interaction is clicking and dragging.**
- Alternatives: multiple threat types from the start; a minigame per threat.
- Reason: get one threat working first; the decision is the game, not the fixing.
- Later threats (e.g. trash files to clean up) introduced level by level.

**D13 - Virus interaction: click a red dot to remove it; all dots removed resolves the ticket.**

**D14 - Damage has 4 levels: Fine, Slightly broken, More broken, Blackout.**
- Planned component effects for later levels: sound card = distorted cues, GPU = flickering screen and motherboard view, NIC = threats appear without warning, HDD = lose an upgrade. These only affect the PC the virus is on (D17).

**D15 - Repairs cannot be bought. Cash comes from resolved tickets, scaled by damage level: 100 / 75 / 50 / 0.**
- Alternative: buy health back (D03).
- Reason: a resolved ticket is closed. Payout scaled by damage rewards speed, which supports the pressure.
- Trade-off: no way to recover from mistakes within a ticket.
- **Update 2026-10-22:** cash is not in the prototype yet (build plan step 8).

**D16 - Freeze token (renamed "virus container" in D25): usable item that pauses a virus for 10s.**
- Its role: the fast, temporary "contain" option next to clicking (slow, permanent).
- **Update 2026-10-22:** not in the prototype yet (build plan step 8).

**D17 - Game over when 2 tickets reach Blackout (2 users lost). Win when all level 1 tickets are resolved.**
- Alternative: one shared PC health pool (D01/M1), which no longer fits the multi-PC fantasy (D07).
- Reason to be written by designer.
- **Update 2026-10-20:** the win condition became "every ticket closed" (resolved or blacked out) with fewer than 2 users lost (D31).

**D18 - Virus capped per component; damage countdown starts only when the virus is at cap.**
- Alternatives: virus copies itself only once (static after one spread), or unlimited duplication (exponential, out of control).
- Reason: "copy once" removed escalation; unlimited spread is uncontrollable. A cap keeps growth readable and bounded.
- Open: does the countdown pause or reset when the player clicks below cap? Reset would allow clicking one dot every 19s to block all damage (degenerate strategy). Default: pause.
- **Update 2026-10-19:** both modes built as a config switch (`countdownBelowCap`), default Pause. Simulation evidence: clicking one dot every 19 s for 3 minutes ends in Blackout with Pause, but keeps the PC at Fine with Reset. The degenerate strategy is real; Pause stays the default.
- **Update 2026-10-26:** the countdown no longer exists. D39 moved damage to a pressure meter and D47 applies damage the moment the meter fills, so the pause/reset switch was removed.

**D19 - Component-based priority deferred until the virus works.**
- Consequence: in level 1, priority comes only from arrival timing, meetings and cross-PC spread. Risk: if tickets feel identical, the triage choice may feel weak.

**D20 - (Deferred in D24) Cross-PC spread: the virus has a random chance of spreading to another computer, on a different component than the source ticket's infected part.**
- Reason to be written by designer. (Effect on the design: ignoring a ticket can create new problems elsewhere, so neglect has a cost beyond that one ticket.)
- Open: new extra ticket or second infection on an existing ticket?

**D21 - (Deferred in D24) In meeting: a ticket can become unavailable (grayed out, meeting icon). The player cannot connect to it, but damage keeps being taken. A ticket can never enter a meeting while the player is connected to it.**
- Reason: simulates real users being in meetings or away from their PC; adds timing pressure.
- Trade-off: the player can lose ground on a ticket without any way to act; must be tuned so it feels tense, not unfair.

**D22 - Level 1 has 6 tickets (was 4).**
- Reason: more headroom so there is always an action available; with 4 tickets and 10-30s spacing, tickets rarely overlapped and no triage happened.

**D23 - Player starts level 1 with 2 virus containers. Cash is paid per solved ticket immediately, not at the end of the level.**
- Reason to be written by designer.
- Open: is a shop available during level 1? Without one, cash has no function in level 1.

**D24 - Meetings (D21) and cross-PC spread (D20) removed from level 1; kept as extras for later levels.**
- Alternative: build them into the first prototype.
- Reason: they are stress extras; first test whether the core (stale view + connection time + virus) creates triage on its own. Add them only if playtests show too little pressure.
- Trade-off: in level 1, tickets differ only in arrival time. This raises the design risk that players simply handle tickets first-in-first-out and never make a real triage choice.

**D25 - "Freeze token" renamed to "virus container".**
- Reason: "freeze" was used for several different things during design (stale ticket view, meetings, the item); one clear name per concept avoids confusion for players and in code. It also fits the theme better.

## 2026-10-19 - Building the prototype (steps 3-6)

**D26 - Virus spread slowed after the step 3 self-test.** (commit db81e4c)
- Change (tuned by the designer in the config): start dots 8-9 → 3-6, spread interval 2 s → 5 s, spread distance 0.05 → 0.2. Cap stays 30.
- Alternatives: linear spread (each spread adds a fixed number of dots instead of doubling); a smaller cap; bigger dots.
- Reason: a virus at its cap felt hopeless. Because every dot copies itself, the virus only shrinks at cap if the player removes more than half the cap per spread, i.e. more than cap / (2 × interval) clicks per second: 7.5 clicks/s with the original values, 3 clicks/s now.
- Trade-off: the cliff is lower but still there. Simulated clearing from cap: 3.0 clicks/s never, 3.5/s in 14.6 s, 4/s in 10 s, 5/s in 7 s. Growth is slower too: a virus reaches its cap after about 20 s instead of 4 s. Linear spread would remove the cliff if playtests show players stuck just under 3 clicks/s.

**D27 - The virus-clicking fix stays, even though one ticket on its own feels easy.**
- Designer's observation: "It felt easy but this will be necessary when tasks start to combine." Doubt raised: clicking viruses might not be the right first minigame because it does not feel difficult.
- Alternative: replace it with a harder minigame.
- Reason (from the discussion, follows D12): the design claim is about choosing between tickets, not about how hard one fix is. Difficulty should come from tickets competing for attention. The virus also fits the stale view: it gets worse while the player is away, and its growth is easy to read when reconnecting.
- Trade-off: in isolation the game has no execution challenge. If the full game also feels easy, the first levers are the system numbers (ticket spacing, connection time, spread), not a new minigame. To be judged in the step 7 playtests.

**D28 - What "stale" means, and the hidden damage cue.** (commit 71b84e9)
- Stale was first defined as "not connected": the list also stayed frozen before the first connection, while connecting and after resolving. Set by the coding agent as an assumption; superseded by D34.
- Hidden damage cue (D10 mitigation): when a ticket the player is not connected to gains a damage level, a placeholder beep plays and its list entry flashes red. The entry keeps showing the old damage level, so the player learns that it got worse, not how bad it is now. Can be switched off in the config (`hiddenDamageCue`).
- Alternative for the cue: update the stale entry's damage level when it changes (no hidden state left).
- Reason to be written by designer.
- Trade-off: the cue leaks which ticket degraded, which slightly weakens the hidden information.

## 2026-10-20 - Milestone 2 playable core (step 7) and playtest fixes

**D29 - Game data column, end screen and restart.** (commit 69f0e87)
- Game data column shows the level timer, resolved tickets (x/6) and lost users (x/2). Cash and virus containers will be added with step 8.
- End screen with the result, a summary line and a Restart button. Restart replays the same level (same random seed), so every playtester gets the same level.
- The lost-users counter is live, even for tickets the player cannot see. Set by the coding agent as an assumption.
- Alternative: count only the losses the player has seen.
- Reason to be written by designer.
- Trade-off: the live counter leaks hidden information (the player learns that a hidden PC blacked out), in exchange for never losing without warning.

**D30 - While a connection is loading, other clicks are ignored: the first clicked ticket always finishes loading.** (commit 2417ff7)
- Alternative: the latest click wins (switch target mid-load).
- Reason to be written by designer.
- Trade-off: no cancelling; a misclick costs the full connection time.

**D31 - Blackout closes the ticket and is never a resolve.** (commit 2417ff7)
- At Blackout the dots disappear, a player connected to that PC is kicked out ("#n blacked out" on the PC screen), and the ticket cannot be opened again. The list shows it as Lost.
- A blacked-out ticket does not count as resolved. The level ends when every ticket is closed (resolved or blacked out): lost at 2 lost users, otherwise won ("All tickets resolved", or "All tickets closed" when a user was lost).
- Alternative (the step 4 behaviour): a blacked-out ticket stays open and can still be cleared, for 0 payout.
- Reason to be written by designer.
- Trade-off: a blacked-out PC can never be salvaged, and "Resolved" can no longer reach 6/6 once a user is lost.

**D32 - Spread copies that land outside the infected component pick a new spot instead of sticking to its edge.** (commit 2417ff7)
- Alternative: clamp them onto the edge (the original behaviour).
- Reason: the designer noticed dots spawning close to the edge too often; it looked less random than intended.
- Evidence: dots exactly on the component edge 35.0% → 0.2%; within 5% of an edge 47.3% → 17.0% (uniform random would be about 19%).

## 2026-10-21 - Changes for the Milestone 2 submission

**D33 - Connection time 2.5 s → 3 s.** (commit 16fb650)
- Reason (stated): make the build match the Milestone 2 text.
- Trade-off: every check or switch costs 0.5 s more. The balance hypothesis (players stop checking above about 3 s) now sits exactly at the current value.

**D34 - Disconnect button; disconnecting is instant and shows every ticket live.** (commit 16fb650)
- A Disconnect button sits on the right of the ticket list header. It only works while connected (greyed out otherwise, and while a connection is loading). Disconnecting takes no time.
- With no session, every list entry shows its live state (`liveListWhenDisconnected` on). Enabled by the coding agent so the button has a purpose; it matches the design claim "decide when to keep working and when to disconnect to check on the others". Replaces the strict stale rule from D28.
- Alternatives: no disconnect at all (switching only, list always frozen); a disconnect that leaves the list frozen.
- Reason: checking becomes free and acting costs connection time, which is the triage cost in D10.
- Trade-off: being disconnected is a free full overview; its only cost is that no ticket is being fixed meanwhile, plus 3 s to reconnect.

**D35 - Frozen tickets are greyed out strongly; no pause icon.** (commits 16fb650, 6d9f5ce)
- While connecting or connected, every other open ticket is labelled "Stale" and drawn at 20% opacity (55% → 35% → 20%). A pause icon next to "Ticket list" was added and removed the same day.
- Reason for the stronger grey: show clearly that the list is frozen (Milestone 2, communication decision). Reason for removing the pause icon: to be written by designer.
- Trade-off: at 20% the last-known state is hard to read, so it is less useful as information.

**D36 - After a loss, the end screen shows "Tip: when viewing a computer, the statuses of tickets are frozen!" above Restart. Not shown after a win.** (commit 5d63510)
- Alternatives: a spotlight tutorial at the start (mentioned in the Milestone 2 draft); no explicit teaching.
- Reason to be written by designer.
- Trade-off: the rule is only explained after a failure, and it is stated outright instead of discovered.

## 2026-10-23 - After the first playtest: minigames replace the virus (BUILD_PLAN_2.md)

Not built yet. These decisions shape `BUILD_PLAN_2.md`, steps 11-17.

**D37 - The virus dots are cut.**
- Alternatives: rework the spread (a fixed number of new dots per interval instead of doubling, which removes the point where it becomes impossible); keep the dots as one of several ticket types.
- Evidence (designer's own playtest of 5d63510): clearing the starting dots is quick, but after a few doublings it takes more than 15 clicks in a few seconds. The game was too easy and too hard at once.
- Reason for cutting instead of reworking: to be written by designer.
- Trade-off: the tuned virus (D26) and the link to the virus theme (D01) are gone; every replacement minigame must be balanced from zero.

**D38 - Four new minigames: Maze, Match the colors, Find the off-folder, Extinguish the firewall. All four are built before the next playtest.**
- Alternatives: one new minigame as the single Milestone 3 change; the smallest fix for the dots plus logging, then a playtest.
- Reason (stated): the game needs more and different types of minigames.
- Trade-off: a much larger build before the next test with other players; Milestone 3 asks for one evidence-based change, so the write-up has to present D37 and D38 as one change (remove the dots, replace them with variety).

**D39 - One pressure meter per ticket replaces the virus cap.**
- Each minigame fills the meter in its own way (fire size, a clock, wrong clicks). A full meter runs the damage countdown (+1 level per 20 s; it pauses below full, as in D18).
- Alternatives: a plain damage timer per ticket from arrival; a fail moment per minigame that adds +1 damage level each time.
- Reason to be written by designer.
- Trade-off: one rule to learn across all minigames, but the meter hides how differently the games build up pressure.
- **Update 2026-10-26:** a full meter no longer runs a countdown; see D47.

**D40 - Tickets need several rounds. Finished rounds are kept, the round in progress keeps getting worse while the player is away, and finishing a round empties the meter. The round count is set per minigame.**
- Alternatives:
  - the current round resets when the player leaves
  - all progress can decay
  - a finished round lowers the meter by a fixed chunk
  - only the last round matters
  - one global round count, or a count per ticket
- Reason (stated): repeating a minigame gives the player moments to check in with other tickets.
- Trade-off: rounds create safe exit points, which may make leaving cheaper than intended; this is what the suggested Milestone 3 question tests.

**D41 - A handbook replaces the PC screen in the upper panel. Reference only for now; puzzles that depend on it come later.**
- Alternatives: merge the PC screen into the minigame area for more space; keep the PC screen; build puzzles that need the handbook straight away.
- Reason (stated): no ideas for challenges inside an emulated OS; a handbook helps the player learn the game and gives them something to look things up in. Later it could become a core concept with puzzles that only make sense with the handbook.
- Trade-off: the minigames keep the smaller bottom panel. Risk: puzzles solved with a manual are the core of Keep Talking and Nobody Explodes; the ECTS sheet fails games that present large parts of existing games as their own, so the later step must stay clearly different.

**D42 - The maze is steered with the keyboard (WASD/arrows).**
- Alternatives: hold the mouse button and the dot follows the cursor; the dot always follows the cursor.
- Reason to be written by designer.
- Trade-off: the game is no longer mouse-only; going from the ticket list (mouse) to the maze (keys) costs a hand movement.

**D43 - Match the colors is a memory game: face-down cards, the second pick must match the first or both flip back.**
- Alternatives: all cards face-up with close shades; cards that reshuffle every few seconds.
- Reason to be written by designer.
- Trade-off: leaving a ticket mid-round costs what the player remembered about the cards.

**D44 - A wrong click in Find the off-folder adds a chunk to the pressure meter.**
- Problem (raised by the coding agent): without a cost, clicking every folder is the fastest strategy (a degenerate strategy, Session 10).
- Alternatives: a new grid after a wrong click; a short lockout.
- Reason for this option to be written by designer.
- Trade-off: careless clicks can start damage on their own; the size of the chunk needs playtesting.

**D45 - One minigame seed drives all minigame randomness, including the maze layouts. Each round is generated from the seed, the ticket and the round number, so a restart replays the same level.**
- Alternative: a handmade set of maze layouts.
- Reason (stated): the seed must be usable by every minigame for randomness and progressive generation.
- Trade-off: generated content needs checks (e.g. every maze must be solvable); difficulty per round still has to be defined (BUILD_PLAN_2 open question 1).

**D46 - The shop item becomes a "pause token" (working name): it pauses one ticket's pressure and damage countdown for 10 s, and keeps running after disconnect. Cash payout and the shop stay.**
- Alternatives: an item that empties the meter once; an item that skips a round.
- Reason to be written by designer.
- Trade-off: same as the virus container (D16): the token buys time, not progress.

## 2026-10-26 - After playing step 11

**D47 - A full pressure meter costs one damage level at once, then drops back to half.**
- Replaces the 20 s countdown at a full meter (D39). With the 30 s meter of Find the off-folder, an ignored ticket now takes damage 30, 45 and 60 s after it arrives (Blackout at 60 s instead of 90 s). An idle player loses at 1:08 instead of 1:50.
- Alternatives: keep the countdown but shorten it; empty the meter completely after each damage level.
- Reason (stated): taking damage should be faster.
- Trade-off: there is no grace time at a full meter, and a wrong click near the top causes damage straight away. Every damage level after the first comes twice as fast as the first.

**D48 - Playtest logging (step 12) is postponed; the handbook (step 13) comes first.**
- Reason to be written by designer.
- Trade-off: playtests before step 12 give only observation, no CSV data.

**D49 - The off-folder never differs in colour, only in shape: file glyph, tab side, tilt or size.**
- Removed: a slightly lighter or darker folder (one of the five differences built in step 11).
- Kept on purpose: the similar file glyph shapes (e.g. a tall page next to a wide sheet).
- Reason (stated): finding the off-folder should be about the icon's shape, not its colour.
- Trade-off: one less kind of round, so each remaining kind comes up more often.

**D50 - Match the colors is solved once (one round), and every matched pair lowers the pressure meter by a quarter.**
- Replaces the step 14 default of 2 rounds, where progress inside a round did nothing to the meter.
- Alternatives: several rounds with no relief (step 14); relief only from finishing the grid.
- Reason to be written by designer.
- Trade-off: the 8 pairs can remove up to two full meters, so a player who keeps matching (one pair every 7.5 s with the 30 s meter) holds the meter level. A player who leaves mid-grid keeps the matched pairs but gets no relief while away.

**D51 - Resolving a ticket ends its session automatically.**
- The board shows "#n resolved", and with no session the whole ticket list shows live again (D34).
- Replaces: the player stayed connected to a resolved ticket until they disconnected or connected elsewhere (a coding-agent default from step 7).
- Reason to be written by designer.
- Trade-off: every fix gives a free live overview of all tickets; staying on a fixed PC is no longer possible.

**D52 - Extinguish the firewall redesigned: water arcs with gravity, the fire grows from the foot of the wall, and damage comes every 4 s above a fire-size threshold.**
- The firewall is a wall on the right of the fire. The fire starts at its foot and grows upwards and away from it. The hose shoots water sideways and gravity bends it down; fire the water touches burns lower until it is gone.
- Damage is the firewall's own rule: above a certain fire size, the PC takes a damage level every 4 s the fire stays above it. There is no "meter drops to half" (D47) for this minigame.
- Replaces the step 15 build: a straight stream that put out the first burning spot on the wall's face, with the D47 damage rule.
- Reason (stated): the step 15 version was not what the designer had in mind.
- Trade-off: aiming is now a skill (aim higher the further away the fire is); once the fire passes the threshold, damage comes fast (Blackout 12 s later if nothing is done).
- Evidence (model run with the default values): an ignored firewall crosses the threshold at about 41 s, takes damage at 45 and 49 s and blacks out at 53 s. A bot with perfect aim clears both rounds in about 3 s when it starts at once, and in about 20 s after 25 s of neglect.
- **Update 2026-10-26:** replaced by the packet filter (D53).

**D53 - The firewall minigame is replaced by a packet filter.**
- Packets with port numbers travel along one cable into the PC. Each round's rule (shown above the cable, picked from the seed) names the blocked port. Clicking a packet blocks it.
- Pressure comes only from bad packets that leak in and from blocking good ones; the stream keeps flowing while the player is away. The round ends after N bad packets are blocked; leaks don't count, so a ticket never fixes itself.
- Alternatives (proposed by the coding agent): close the ports (whack-a-mole), patch the wall (drag patches over holes), remove the minigame and keep three.
- Reason (stated): the designer did not like the fire minigame and could not think of a better fit. The packet filter was the recommended alternative: a firewall literally filters traffic, it adds a new skill (sorting decisions), it builds pressure on its own while the player is away, and the rule could later move to the handbook (D41).
- Risk the designer must be able to explain: real-time clicking again, like the cut virus dots (D37). It differs because the stream never speeds up and every packet asks for a decision (read the port) instead of pure speed.
- Trade-off: a round's length is set by how fast bad packets arrive, not by how fast the player is (a perfect player still needs about 15 s per round).
- Evidence (model run, defaults): ignored, damage at about 24, 45 and 69 s; a perfect bot resolves both rounds in about 30 s with no leaks.

**D54 - Denser packets: twice as many on the cable, and twice as many bad packets to block per round.**
- A packet every 0.75 s instead of 1.5 s; 8 blocks per round instead of 4. The exact numbers are coding-agent assumptions; the designer asked for "more dense" with more blocks needed.
- Reason (stated): the stream was too sparse.
- Trade-off: an ignored packet filter now leaks twice as fast. In a model run it takes damage at about 14, 24 and 36 s (was 24, 45 and 69 s), much faster than the 30 s clock games. A perfect player still needs about 30 s for both rounds.

**D55 - Smaller maze, and a wall hit bounces the dot back instead of sending it to the start.**
- 6x4 cells instead of 9x7; the route is about 11 cells and a perfect run takes about 11 s for both rounds (was about 38 s).
- Wall hit: the dot bounces 0.5 cells back along the path it came (the cell count and speed are coding-agent assumptions). It replaces "touching a wall restarts the maze" from the first maze description.
- Reason (stated): the maze was too big, and the wall hit should bounce the dot back rather than reset it.
- Trade-off: a wall hit is now a small setback, so most of the maze's pressure comes from the drift while the player is away.

**D56 - A resolved ticket shrinks to a half-height row in the ticket list, showing only its number, title and "Resolved".**
- Lost (blacked-out) tickets keep the full row with their damage and "Lost".
- Reason to be written by designer.
- Trade-off: open tickets stand out more and the list holds more tickets, but a resolved ticket no longer shows the damage it was fixed at.
- **Update:** blacked-out tickets get the same short row (D59).

**D57 - The packet filter runs once, its packets move slower, and every blocked bad packet takes 15% off the meter.**
- One round of 8 blocks (was 2 rounds); packets take 6 s to cross (was 4 s; the exact value is a coding-agent assumption).
- Reason to be written by designer.
- Trade-off: a perfect player clears it in about 17 s (was about 30 s) and good play actively lowers the pressure. Ignored, it still hurts fast: damage at about 16, 26 and 38 s, since the leak pressure was left as it was.

**D58 - The ticket list updates every time the player switches to a ticket.**
- When the player starts connecting to a ticket, every entry takes the current state of its ticket once, then stays stale again. Before this, a stale entry only updated while its ticket was connected or while the player had no session (D34). It can be switched off in Level1Config (`refreshListOnSwitch`).
- Reason to be written by designer.
- Trade-off: switching now doubles as a free check of all tickets, so the list is never older than the last switch; the hidden information (D10) is weaker.

**D59 - A blacked-out ticket also shrinks to the short row, with "Blackout" as its status (was "Lost").**
- Reason to be written by designer.
- Trade-off: closed tickets look alike apart from one word; the end screen and the "lost users" counter still show the losses.

---

## Open questions (from the build plan)
1. Hidden degradation cue (audio ping + flash): built (D28) and kept in the Milestone 2 text.
2. Countdown below cap, pause or reset: Pause, with simulation evidence in the D18 update.
3. Shop during level 1: not built yet (step 8).

## Assumptions still to confirm
Values the coding agent chose because the build plan left them open. All are in `Assets/ScriptableObjects/Level1Config.asset` unless noted.
- Ticket arrival times 0, 8, 20, 30, 45, 55 s; descriptions for tickets #3-#6 are placeholders.
- Minigame values (rounds, meter speed, wrong-click cost, ways the off-folder differs): every `[ASSUMPTION]` in `BUILD_PLAN_2.md` section 3, also marked in the Level1Config tooltips. Built so far: Find the off-folder (step 11), Match the colors (step 14, ticket #2), Packet filter (replaced the firewall, D53, tickets #3 and #6), Maze (step 16, ticket #4).
- Handbook text in `Assets/ScriptableObjects/Handbook.asset` is a draft by the coding agent (step 13), written to avoid exact numbers so it stays true when values are tuned. To be rewritten by the designer.
- The minigame panel now holds the session messages of the old PC screen ("No remote session", "Connecting to #n...", "Ticket resolved", "#n blacked out") in the middle of the board, with the connection progress bar (step 13).
- The virus-only assumptions (infected areas, dot size, spread clock) went with the dots (D37).

---

## Naming ideas
- "Lost Control" (working title): matches the experience of losing control.
- An actual burning firewall as a visual (Rule of Cool).
