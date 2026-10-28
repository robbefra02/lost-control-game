# Lost Control - Build Plan 2: minigames and handbook

Second handoff document for the Unity coding project. It continues the original `BUILD_PLAN.md` (Google Drive, steps 0-10). Steps 0-7 of that plan are built (up to commit c0e4fe0). This plan replaces its steps 8-10.
Design owner: Robbe. The reasons behind every rule are in `DECISION_LOG.md` (D37-D46 for this plan).

**Rule for the coding agent (unchanged):** do not make design decisions. If a rule is missing or ambiguous, implement the default marked `[ASSUMPTION]`, expose it as a tunable value, and list it in the commit message so the designer can confirm or change it.

---

## 1. Why this plan exists

**Design claim (unchanged):** the player should feel torn between tickets, because the game hides the state of every ticket except the one they are connected to, forcing them to decide when to keep working and when to disconnect and check the others.

**First playtest (designer, commit 5d63510):**
- The virus dots were too easy and too hard at the same time. The starting dots go quickly, but once the virus doubles a few times it needs more than 15 clicks in a few seconds.
- The layout works. The PC screen panel has no clear purpose: there are no ideas for challenges inside an emulated OS.

**What changes:**
- The virus dots are cut. Four new minigames replace them.
- Each ticket needs several rounds of its minigame, so the player gets natural moments to check other tickets.
- Each ticket has one pressure meter that replaces the virus cap.
- A handbook replaces the PC screen.

**Suggested Milestone 3 question** (designer to confirm): *Do players leave a ticket between rounds to check the others?*
- Evidence: in the CSV log (step 12), compare disconnects and switches right after `round_done` with disconnects in the middle of a round.

No deadline is announced for Milestone 3 yet. Every step below ends in a playable build, and from step 12 on any build can be playtested and logged.

---

## 2. Terminology (use these names in code)

The old rule still holds: never use the word "freeze" in code or UI.

| Term | Meaning | Code name |
|---|---|---|
| **Minigame** | The task on one ticket's PC. One type per ticket. | `MinigameType`, `MinigameModel` |
| **Round** | One playthrough of a minigame. A ticket is resolved when its last round is done. | `RoundsDone`, `RoundsNeeded` |
| **Pressure meter** | Per ticket, 0-1. Filled by the minigame while a round is unsolved. Full = damage countdown runs. | `TicketModel.Pressure` |
| **Minigame seed** | The one seed every minigame draws its randomness from. | `LevelConfig.minigameSeed` |
| **Handbook** | Reference panel in the upper right area. | `HandbookView` |
| **Pause token** | `[ASSUMPTION]` working name for the shop item that replaces the virus container. | `PauseToken` |

Unchanged terms: Connected, Stale, Damage level (0 Fine, 1 Slightly broken, 2 More broken, 3 Blackout), Lost user.

---

## 3. Rules and parameters

All values go in `LevelConfig`. Each minigame gets its own `[Serializable]` settings class, shown in `LevelConfig` under its own header. Nothing is hardcoded.

### Layout (1920x1080, same panel sizes as now)
**Game data** | **Ticket list** | right column: empty area (top; the PC screen, then the handbook until D62) and **Minigame panel** (bottom, replaces the motherboard).

The minigame panel gets a header with the parts of the old PC screen:
- the connected ticket's title
- the connection progress bar
- the damage level
- the pressure meter
- the round count ("Round 2 / 3")

With no session it shows "No remote session" or "#n blacked out", as the PC screen does now.

### Tickets (unchanged unless listed)
- Six tickets, arriving at 0, 8, 20, 30, 45 and 55 s. Plus the existing rules for connecting, the stale view, the hidden damage cue, Disconnect, and win/lose/restart.
- Resolving a ticket ends its session automatically (D51).
- A closed ticket's list entry shrinks to half height and shows only its number, title and "Resolved" or "Blackout" (D56, D59).
- Each time the player switches to a ticket (starts connecting), every list entry takes its ticket's current state once, then stays stale (D58, `refreshListOnSwitch`).
- `TicketDefinition.threat` and `infectedArea` are replaced by `minigame` (a `MinigameType`).
- Level 1 has five tickets (D69): #1 Off-folder, #2 Match the colors, #3 Packet filter, #4 Maze, #5 SSD trimming. The designer wrote the ticket descriptions.
- `[ASSUMPTION]` The ticket descriptions are placeholders that hint at the minigame. The designer writes the final text.
- List entries show the damage level and a small pressure bar (D60, replaces "Round n/N"); a stale entry shows the last-known pressure.

### Pressure meter and damage
| Parameter | Value |
|---|---|
| Meter | 0-1 per ticket. Each minigame defines how it fills (see the minigames below). It fills while connected and while away. |
| Full meter | +1 damage level at once, then the meter drops back to half (`pressureAfterDamage` 0.5). It replaces the 20 s countdown at full (D47, 2026-10-26). A minigame can override this rule (`MinigameModel.CheckDamage`); none does at the moment. |
| Round done | The meter empties to 0. |
| Blackout | Unchanged: the ticket closes, the user is lost, the player is kicked out. |

### Rounds
| Parameter | Value |
|---|---|
| Rounds per ticket | Set per minigame. Match the colors 1 (D50), Packet filter 1 (D57), Maze 1 (D68). `[ASSUMPTION]` Off-folder 3, SSD trimming 1. |
| Progress | Finished rounds stay finished when the player leaves. The round in progress keeps getting worse while they are away: the meter fills, bad packets leak in, the maze dot drifts back. |
| Next round | Starts immediately after a round is done, with new content from the minigame seed. |

### Minigame seed
- `minigameSeed` in `LevelConfig` replaces `randomSeed`. The dots were the only user of `randomSeed`.
- Every minigame gets its randomness from `MinigameSeed.For(ticketId, round)`. Same seed, same ticket and same round always give the same content, so a restart replays the same level.
- "Progressive generation": each generator also receives the round index, and difficulty values can be set per round (see each minigame).
  - `[ASSUMPTION]` All difficulty values are flat by default: no ramp until the designer sets one.

### Find the off-folder
- A 6x6 grid of greybox folder icons, each with a file glyph. Exactly one icon differs in one attribute. Click it to finish the round.
- The attribute that differs is picked from the seed. It is always about the icon's shape, never its colour (D49):
  - file glyph shape
  - tab side
  - small rotation
  - size
- `[ASSUMPTION]` How strong the difference is can be set per round (the rotation and size differences).
- A wrong click adds a chunk to the meter. `[ASSUMPTION]` +0.2, and the clicked folder flashes red.
- `[ASSUMPTION]` The meter fills on a clock: empty to full in 30 s.

### Match the colors
- A 4x4 grid of face-down cards: 8 color pairs, shuffled from the seed.
- Click a card and it shows its color. The second pick:
  - same color: both stay face-up (matched)
  - different color: both stay visible for a moment, then flip back. `[ASSUMPTION]` 0.75 s, and clicks are ignored during that time.
- All pairs matched = round done. The grid is solved once: one round per ticket (D50).
- Every matched pair lowers the meter by a quarter (`matchPressureRelief` 0.25, D50).
- `[ASSUMPTION]` A mismatch costs nothing extra.
- `[ASSUMPTION]` Matched pairs stay when the player leaves. A single revealed card flips back on disconnect.
- `[ASSUMPTION]` The meter fills on a clock: empty to full in 30 s.

### Packet filter (replaces Extinguish the firewall, D53)
Both firewall versions (step 15 and the D52 redesign) are removed.
- Packets travel along one cable from left to right into the PC. Each packet shows a port number.
- Every round, a rule above the cable names the blocked port, picked from the minigame seed.
  - `[ASSUMPTION]` Ports: 20, 21, 22, 23, 25, 53, 80, 110, 143, 443, 3389.
  - `[ASSUMPTION]` One blocked port per round; the count can be set per round.
- Clicking a packet blocks it (removes it).
  - A bad packet (blocked port) counts towards the round.
  - A good packet adds pressure. `[ASSUMPTION]` +0.2.
- A blocked bad packet also takes 15% off the meter (D57).
- A bad packet that reaches the PC adds pressure (`[ASSUMPTION]` +0.15) and the PC flashes red. Good packets pass harmlessly.
- Only leaks and wrong blocks fill the meter, with no clock. The stream keeps flowing while the player is away, so bad packets leak in. Damage uses the normal rule (D47).
- The round is done after N bad packets are blocked; leaked ones don't count, so a ticket never fixes itself.
  - `[ASSUMPTION]` 8 per round (D54: denser stream, more blocks). One round only (D57).
- `[ASSUMPTION]` Stream: a packet enters every 0.75 s (never faster), 35% are bad (D54), and crossing takes 6 s (slower, D57).
- `[ASSUMPTION]` A new round starts with an empty cable and the first packet entering at once. The board shows "Blocked n / N".

### Maze
- A dot steered through a maze with an on-screen joystick, so it is played with the mouse (D64; it replaces the d-pad of D61 and the keyboard of D42). The player grabs the joystick and drags it in any direction.
  - `[ASSUMPTION]` The joystick sits right of the maze.
  - `[ASSUMPTION]` The dot's speed follows how far the stick is pushed.
  - `[ASSUMPTION]` 15% of the radius is a dead zone.
- Touching a wall bounces the dot backwards (D55). `[ASSUMPTION]` 0.5 cells back along its own path at 4 cells/s; keys do nothing during the bounce.
- When the player is not actively playing (no movement key held, or not connected), the dot moves slowly backwards. `[ASSUMPTION]` It goes back along its own path towards the start.
- Reaching the exit = round done. `[ASSUMPTION]` Start in the bottom-left cell, exit in the top-right cell.
- One maze per ticket (D68), generated from the minigame seed. `[ASSUMPTION]` 8x4 cells (the designer asked for a little longer than 6x4).
- Pellets lie on the way to the exit or on a short detour (D68). When the dot touches one, it disappears and takes pressure off the meter.
  - The designer asked for at most 3. `[ASSUMPTION]` There are always 3 when the maze has room. They never sit in the start or exit cell or next to each other.
  - `[ASSUMPTION]` "A short detour" is at most 2 cells off the way to the exit. Each pellet takes 0.15 off the meter. Pellet radius 0.15 cells, yellow.
- `[ASSUMPTION]` Generated by depth-first carving: every cell is reachable and there is exactly one route to the exit.
- `[ASSUMPTION]` The meter fills on a clock: empty to full in 30 s.
- `[ASSUMPTION]` Dot speed 2 cells/s at a full push; drift speed 0.5 cells/s.
- The drift starts after 2 s without steering (D64). "Without steering" means the joystick is released, or the player is not connected.

### SSD trimming (D65)
- A stack of wide, low blocks sticks out on the left and the right past a marked target width. The marks show the tolerance.
- The player holds the mouse button and draws a cutting line (D67). On release, every block whose middle the line crosses is cut off there, on the side of the centre the crossing is on. A click without a line cuts nothing.
  - The designer asked for a maximum length of just over twice the thickness of one block, then tuned it to 1.22 thicknesses in the Inspector. With the gap between blocks, one line now reaches one block, so trimming takes a line per end.
  - The line follows the mouse while it is held; the blocks it would cut light up.
- Every end grows back slowly, also while the player is away. An end cut past the tolerance has to grow back into the mark.
- The round is done once every end is within the tolerance.
- The meter fills on a clock in 47 s (the designer asked for forty-something, then 5 s more, D67).
- `[ASSUMPTION]` Layout and round: 5 blocks; target width 0.4 of the view; tolerance ±0.03; one round.
- `[ASSUMPTION]` Cutting: starting overhang 0.08-0.22 per end; regrowth 0.0015/s; the line is drawn in red.
- `[ASSUMPTION]` Ticket #5 plays it (it was the second off-folder ticket); a block turns green once both its ends are trimmed.

### Explanations (replace the handbook, D62, D63)
The handbook built in step 13 is removed. Its upper right area stays empty.
- **Introduction:** before the level starts, an overlay (styled like the end screen) shows the basics: tickets, connecting, the stale list, pressure and damage. The level clock and all tickets wait until the player clicks "Let's go!".
  - It shows again after a restart.
  - Under a divider the player can type the minigame seed, or tick "Random", which greys the box out (D66). "Random" starts ticked. A random seed is written into the box. `[ASSUMPTION]` The box and the tick are remembered across restarts in one run, so a random level can be replayed by unticking.
  - It can be switched off with `showIntroduction` in Level1Config.
- **Info box:** the remote PC panel shows an info box on the right of the board that explains the connected ticket's minigame. The minigame gets the rest of the board.
  - `[ASSUMPTION]` The box takes 34% of the board.
  - The 'i' icon at the right of the panel header folds the box away or back for every ticket. It is open by default.
  - The icon is grey while the box is closed and bright while hovered.
  - `[ASSUMPTION]` The setting resets to open after a restart.
- All texts live in `Assets/ScriptableObjects/InfoTexts.asset`. `[ASSUMPTION]` They are drafts by the coding agent; the designer rewrites them.

### Economy (was step 8)
| Parameter | Value |
|---|---|
| Payout on resolve | Unchanged: 100 (Fine), 75 (Slightly), 50 (More), 0 (Blackout). |
| Pause token | Replaces the virus container. Pauses the connected ticket's pressure for 10 s: meter clock, packet leaks, maze drift and the damage countdown. Keeps running after disconnect. The player starts with 2. |
| Using it | `[ASSUMPTION]` Only while connected (not while connecting), from a button in the game data panel. |
| Wrong clicks while paused | `[ASSUMPTION]` Still add to the meter: the penalty is separate from time pressure. |
| Shop | Unchanged: `[ASSUMPTION]` available during the level, one token for 100. |

### Removed
- `VirusModel`, `DotView`, `MotherboardView`, `PcScreenView` and `ThreatType`.
- `LevelConfig` virus fields: `startDotsMin`, `startDotsMax`, `spreadInterval`, `spreadDistance`, `dotCap` and `dotDiameter`.
- The designer's tuned virus values (start dots 3-6, spread every 5 s, spread distance 0.2) stay in git history (commit c0e4fe0).

---

## 4. Architecture

The original guidelines still hold: simulation separate from the view, one config asset, normalized coordinates, uGUI, seeded RNG. The changes:

```
GameSimulation
  TicketModel (one per ticket)
    Minigame : MinigameModel        pure C#, one subclass per MinigameType
    Pressure  (reads Minigame.Pressure, 0-1)
    RoundsDone / RoundsNeeded
    Damage    (+1 each time Pressure reaches 1; the meter then drops to pressureAfterDamage)
  ConnectionModel                   unchanged
MinigameModel (abstract)
  StartRound(SeededRandom random, int round)
  Tick(float deltaTime, bool connected, bool paused)
  Pressure, ResetPressure()
  event RoundCompleted
  + input methods per subclass: ClickFolder(i), PickCard(i), SetHose(position, spraying), SetMove(direction)
MinigameSeed
  SeededRandom For(int ticketId, int round)
```

- **Views:**
  - `MinigamePanelView` owns the header and one child view per `MinigameType`. It shows the view of the connected ticket's minigame only.
  - The child views are `OffFolderView`, `MatchColorsView`, `PacketFilterView` and `MazeView`.
  - `IntroView` holds the level until "Let's go!"; the info box and its `InfoToggleView` live in the minigame panel.
- **Input:** clicks go through uGUI, as now. The maze's `JoystickView` uses pointer down, drag and up; `SsdTrimView` draws the SSD cutting line the same way.
  - Views pass input to the model. They never change state themselves.
- **Logging:** a `PlaytestLog` listens to simulation events. Views do not write to it, except handbook page opens.

---

## 5. Step-by-step build order

Commit and push after every step, on `main`. Do not start the next step before the current one works in Play mode.

**Step 11 - Minigame framework + Find the off-folder**
- Build `MinigameModel`, `MinigameSeed`, the pressure meter, rounds and `MinigamePanelView`.
- Port damage from the virus cap to the full meter.
- Remove the virus code and fields (section 3, "Removed").
- First minigame: Find the off-folder. All six tickets use it until their own minigame exists.
- Check: all tickets play; the meter fills while away; a round empties it; damage starts at a full meter; a wrong click raises the meter; a restart gives the same grids.

**Step 12 - Playtest logging (was step 9)** - postponed by the designer on 2026-10-26; build it later, before the Milestone 3 playtest.
- Write one CSV per session to `Application.persistentDataPath`, with columns: time, event, ticket, minigame, round, value.
- Events: `session_start` (seed), `ticket_arrived`, `connect_start`, `connect_done`, `disconnect`, `round_start`, `round_done`, `wrong_click`, `mismatch`, `wall_hit`, `meter_full`, `damage_level_up`, `token_used`, `token_bought`, `handbook_page`, `resolved`, `blackout` and `game_over`.
- `[ASSUMPTION]` The end screen shows the log file's path in small text, so playtesters can find it.
- Check: one session gives a readable CSV; a switch right after a round is visible in it.

**Step 13 - Handbook panel** (built, then replaced by the introduction and info box, D62, D63)
- Replace the PC screen with `HandbookView` and its tabs, filled with the draft text.
- Move the session info (title, progress bar, damage, meter, rounds) into the minigame panel header.
- Check: the layout still matches the wireframe at 1920x1080 and scales at 16:9; the handbook can be read while connected and while connecting.

**Step 14 - Match the colors**
- Ticket #2 uses it.
- Check: pairs match, mismatches flip back, matched pairs stay after a disconnect, the meter fills, restart gives the same layout.

**Step 15 - Packet filter** (first built as Extinguish the firewall, replaced by D53)
- Tickets #3 and #6 use it.
- Check: bad packets leak pressure when left alone (connected and away); blocking a bad packet counts, blocking a good one costs pressure; the round needs N blocks.

**Step 16 - Maze**
- Ticket #4 uses it.
- Check: a wall hit returns the dot to the start; the dot drifts back with no key held and while away; reaching the exit finishes the round; keys do nothing on other tickets; restart gives the same mazes.

**Step 17 - Economy (was step 8)**
- Payout per resolve, pause tokens (start with 2) and their effect, the shop, and the "Items & shop" handbook tab.
- Check: payouts match the damage level; a token pauses all pressure on its ticket for 10 s, also after a disconnect; buying costs 100.

> After step 12 every build can go to playtesters. If Milestone 3 is announced before step 17, test with what exists, and remember that the log needs at least two minigame types to say anything about switching between different games.

---

## 6. Open questions for the designer

1. **Progressive generation:** should rounds get harder (a subtler off-folder, a bigger maze, more blocked ports)? This plan builds the hooks but keeps the difficulty flat.
2. **Meter speeds:** 30 s from empty to full for the clock-based games is a placeholder. Should it differ per minigame?
3. **Packet filter:** should later rounds block more than one port, or use other packet traits?
4. **Maze:** should the dot drift back along its own path, or straight back to the start?
5. **Pause token:** the final name, and whether wrong clicks should still count while it runs.
6. **Ticket list:** should it show the minigame type, or only the description?
7. **Handbook:** the final text, and what the later "puzzles need the handbook" step looks like (D41: it must stay clearly different from Keep Talking and Nobody Explodes).
