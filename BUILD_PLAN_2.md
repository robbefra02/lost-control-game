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
**Game data** | **Ticket list** | right column: **Handbook** (top, replaces the PC screen) and **Minigame panel** (bottom, replaces the motherboard).

The minigame panel gets a header with the parts of the old PC screen:
- the connected ticket's title
- the connection progress bar
- the damage level
- the pressure meter
- the round count ("Round 2 / 3")

With no session it shows "No remote session" or "#n blacked out", as the PC screen does now.

### Tickets (unchanged unless listed)
- Six tickets, arriving at 0, 8, 20, 30, 45 and 55 s. Plus the existing rules for connecting, the stale view, the hidden damage cue, Disconnect, and win/lose/restart.
- `TicketDefinition.threat` and `infectedArea` are replaced by `minigame` (a `MinigameType`).
- `[ASSUMPTION]` Level 1 mix: #1 Off-folder, #2 Match the colors, #3 Firewall, #4 Maze, #5 Off-folder, #6 Firewall.
- `[ASSUMPTION]` The ticket descriptions are placeholders that hint at the minigame. The designer writes the final text.
- `[ASSUMPTION]` List entries add "Round n/N" next to the damage level. The pressure meter is not shown in the list.

### Pressure meter and damage
| Parameter | Value |
|---|---|
| Meter | 0-1 per ticket. Each minigame defines how it fills (see the minigames below). It fills while connected and while away. |
| Full meter | +1 damage level at once, then the meter drops back to half (`pressureAfterDamage` 0.5). It replaces the 20 s countdown at full (D47, 2026-10-26). |
| Round done | The meter empties to 0. |
| Blackout | Unchanged: the ticket closes, the user is lost, the player is kicked out. |

### Rounds
| Parameter | Value |
|---|---|
| Rounds per ticket | Set per minigame. Match the colors 1 (D50). `[ASSUMPTION]` Off-folder 3, Firewall 2, Maze 2. |
| Progress | Finished rounds stay finished when the player leaves. The round in progress keeps getting worse while they are away: the meter fills, fire grows, the maze dot drifts back. |
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

### Extinguish the firewall
- The firewall is on the right side of the panel and is on fire. Over the panel, the cursor becomes a hose that sprays a thin stream of water to the right.
- Fire:
  - `[ASSUMPTION]` A grid of fire cells on the wall (8 rows x 3 columns), each with an intensity of 0-1.
  - `[ASSUMPTION]` A round starts with a few burning cells, placed from the seed.
  - Fire grows while it is not being sprayed, including while the player is away. `[ASSUMPTION]` Unsprayed cells gain intensity at a fixed rate. A cell at full intensity sets a neighbouring cell alight after a delay.
- Spraying:
  - `[ASSUMPTION]` The hose sprays while the left mouse button is held.
  - `[ASSUMPTION]` The stream is thin (about a third of a cell high) and long enough to reach the wall from anywhere in the panel.
  - It lowers the intensity of the cells it touches.
- All cells at 0 = round done.
- The meter is the fire itself: total intensity / maximum intensity.
- `[ASSUMPTION]` Spray width, intensity growth, spread delay and spray strength are tunables.

### Maze
- A dot steered through a maze with the keyboard (WASD or the arrow keys). It moves slowly.
- Touching a wall puts the dot back at the start of the same maze.
- When the player is not actively playing (no movement key held, or not connected), the dot moves slowly backwards. `[ASSUMPTION]` It goes back along its own path towards the start.
- Reaching the exit = round done.
- Each round's maze is generated from the minigame seed. `[ASSUMPTION]` 9x7 cells; the size can be set per round.
- `[ASSUMPTION]` The meter fills on a clock: empty to full in 30 s.
- `[ASSUMPTION]` Dot speed 2 cells/s; drift speed 0.5 cells/s, starting right away.
- Keyboard input goes to the maze only while the player is connected to a maze ticket.

### Handbook (reference only in this plan)
- Always visible in the upper right panel. It has tabs:
  - Basics: tickets, connecting, the stale list, the meter, damage levels, lost users
  - one tab per minigame
  - Items & shop, added in step 17
- Reading it costs nothing and works at any time, including while connecting.
- `[ASSUMPTION]` The coding agent writes a first draft of every page from the rules above. The draft is marked as such, and the designer rewrites it.
- Puzzles that can only be solved with the handbook come in a later plan (D41), not this one.

### Economy (was step 8)
| Parameter | Value |
|---|---|
| Payout on resolve | Unchanged: 100 (Fine), 75 (Slightly), 50 (More), 0 (Blackout). |
| Pause token | Replaces the virus container. Pauses the connected ticket's pressure for 10 s: meter clock, fire growth, maze drift and the damage countdown. Keeps running after disconnect. The player starts with 2. |
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
  - The child views are `OffFolderView`, `MatchColorsView`, `FirewallView` and `MazeView`.
  - `HandbookView` is the upper right panel.
- **Input:** clicks go through uGUI, as now.
  - `FirewallView` reads the pointer position and left button with the Input System.
  - `MazeView` reads `Keyboard.current` (WASD and arrows), only while connected to its ticket.
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

**Step 13 - Handbook panel**
- Replace the PC screen with `HandbookView` and its tabs, filled with the draft text.
- Move the session info (title, progress bar, damage, meter, rounds) into the minigame panel header.
- Check: the layout still matches the wireframe at 1920x1080 and scales at 16:9; the handbook can be read while connected and while connecting.

**Step 14 - Match the colors**
- Ticket #2 uses it.
- Check: pairs match, mismatches flip back, matched pairs stay after a disconnect, the meter fills, restart gives the same layout.

**Step 15 - Extinguish the firewall**
- Tickets #3 and #6 use it.
- Check: fire grows when left alone (connected and away), the hose puts it out, the meter follows the fire, the cursor turns back to normal outside the panel.

**Step 16 - Maze**
- Ticket #4 uses it.
- Check: a wall hit returns the dot to the start; the dot drifts back with no key held and while away; reaching the exit finishes the round; keys do nothing on other tickets; restart gives the same mazes.

**Step 17 - Economy (was step 8)**
- Payout per resolve, pause tokens (start with 2) and their effect, the shop, and the "Items & shop" handbook tab.
- Check: payouts match the damage level; a token pauses all pressure on its ticket for 10 s, also after a disconnect; buying costs 100.

> After step 12 every build can go to playtesters. If Milestone 3 is announced before step 17, test with what exists, and remember that the log needs at least two minigame types to say anything about switching between different games.

---

## 6. Open questions for the designer

1. **Progressive generation:** should rounds get harder (a subtler off-folder, a bigger maze, more starting fire)? This plan builds the hooks but keeps the difficulty flat.
2. **Meter speeds:** 30 s from empty to full for the clock-based games is a placeholder. Should it differ per minigame?
3. **Firewall:** should the hose spray only while the button is held, or always?
4. **Maze:** should the dot drift back along its own path, or straight back to the start?
5. **Pause token:** the final name, and whether wrong clicks should still count while it runs.
6. **Ticket list:** should it show the minigame type, or only the description?
7. **Handbook:** the final text, and what the later "puzzles need the handbook" step looks like (D41: it must stay clearly different from Keep Talking and Nobody Explodes).
