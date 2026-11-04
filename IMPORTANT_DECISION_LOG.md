# Lost Control - Important Decision Log

The four decisions that shaped the game most. They come from `DECISION_LOG.md`, which stays as the full history (D01-D70). Production updates, small tweaks and decisions with no player-facing effect are left out.

**Design claim (D02):** the player is a servicedesk agent with more broken PCs than they can handle at once. They have to keep deciding which ticket to work on and which to leave.

So far, the evidence comes from the designer's own playtests and from model runs; the build logs no playtest data (D70). Under "Player behavior", **observed** means the designer saw it while playing; **expected** means a hypothesis still to test.

Text in *[brackets]* is for the designer to fill in, because the log has no recorded reason there.

---

## 1. The other tickets go stale while you are connected (D10)

**Decision:** While the player is connected to one PC, the other tickets in the list stop updating.
- They refresh once each time the player switches (D58) and show live while the player is disconnected (D34).
- When a PC the player can't see takes damage, a beep plays and its row flashes (D28).

**Alternative:** A live overview of every ticket at all times.

**Reason:** With live information the best order is always visible, so the choice of ticket becomes nominal. Hiding it turns checking into a decision with a cost: getting back to work means reconnecting (D10). The softer rules came from playing. The designer found the strict version stressful, unfair and odd (context of D60), so the list now refreshes on every switch and shows a pressure bar per row (D60).

**Trade-off:** A player can lose a ticket they never saw getting worse. Each softening (the beep, the refresh on switch, the live list when disconnected) leaks some of the hidden information back.

**Player behavior:**
- Observed (designer): the strict version felt stressful and unfair.
- Expected: players disconnect or switch partly to look, not only to fix. They react to the beep by checking the ticket that flashed.

**Counterfactual:**
- Removed: the fullest pressure bar is always visible. The game turns into "go to the highest bar", and connection time is only a delay.
- Doubled (no refresh on switch, no live list when disconnected, no beep): losses come without warning and feel random. Players would likely fall back to a fixed rotation.
- Both versions can be shown in the build with `refreshListOnSwitch`, `liveListWhenDisconnected` and `hiddenDamageCue` in Level1Config.

## 2. Connecting to a PC takes time (D09)

**Decision:** Switching to a ticket costs a connection time, during which nothing can be fixed. It is 2.1 s in the final build, and a connection that has started always finishes (D30). Disconnecting is instant and shows every ticket live (D34). Looking is free; getting back to work costs the connection time.

**Alternative:** Switching instantly, with no wait.

**Reason:** In a 2D interface, it keeps the opportunity cost that walking between problems had in the original concept (D09). *[Why 2.1 s: the designer lowered it from 3 s (D33) in the final tuning; reason not recorded.]*

**Trade-off:** Every check costs a reconnect, and a misclick costs a full connection. Too long, and it becomes friction instead of a choice (D09).

**Player behavior:** Expected (the Milestone 2 balance hypothesis, D33): players stop checking other tickets when connecting takes more than about 3 s. At 2.1 s they still check, but stay on a ticket long enough to make the trip worth it.

**Counterfactual:**
- Removed: leaving to look costs nothing, so a player can check constantly. The stale view (decision 1) stops hiding anything.
- Doubled (4.2 s): this is above the 3 s hypothesis. Players would switch rarely and work through the tickets first-in-first-out, the triage failure flagged in D24.

## 3. One pressure meter per ticket; a full meter costs a damage level (D39, D47)

**Decision:** Every ticket has one pressure meter that its minigame fills in its own way: a clock, leaked packets or wrong clicks.
- A full meter costs one damage level at once and drops back to half (D47). The levels are Fine, Slightly broken, More broken and Blackout.
- Good play lowers the meter: each matched pair, blocked packet or maze pellet takes a chunk off (D50, D57, D68).
- Two Blackouts lose the level.

**Alternative:** A plain damage timer per ticket, running from its arrival.

**Reason:** One rule works across every minigame, and it can be shown as one bar per row in the list (D60). Damage at once, instead of after a 20 s countdown, came from playing: "taking damage should be faster" (D47). In a model run at the time, an idle player lost at 1:08 instead of 1:50. *[Why a meter instead of a timer (D39): reason not recorded.]*

**Trade-off:** There is no grace time at a full meter, and each damage level after the first comes twice as fast. One shared meter also hides how differently the minigames build pressure (a clock, leaks, wrong clicks).

**Player behavior:** Expected: players use the list's pressure bars to pick the next ticket. Before leaving a ticket, they push its bar down with relief (matches, blocks, pellets). Pellets make players take a short detour in the maze (D68).

**Counterfactual:**
- Removed: each minigame would need its own damage rule, as the firewall had (D52). The list could no longer compare tickets, so choosing becomes guesswork.
- Doubled (meters fill in about 20-27 s instead of 40-53 s): more PCs fill while the player is busy elsewhere. That brings back the stressful, unfair feeling behind D60, and pushes players to give up a ticket on purpose (one Blackout is survivable).

## 4. Five different minigames replace the virus dots (D37, D38)

**Decision:** The single virus-clicking task is cut. Each ticket now has its own minigame: find the off-folder, match the colors, packet filter, maze or SSD trimming. In level 1 there is one ticket per minigame (D69).

**Alternative:** Rework the virus spread: add a fixed number of dots per interval instead of doubling them, which removes the point where clearing becomes impossible (D37).

**Reason:** Evidence from the designer's playtest of the first build (5d63510): the starting dots were quick to clear, but after a few doublings it took more than 15 clicks in a few seconds. The game was too easy and too hard at once (D37). And with one task everywhere, tickets differed only in arrival time, the risk flagged in D24. Stated: the game needs more and different types of minigames (D38). *[Why cut instead of rework: reason not recorded.]*

**Trade-off:** A much bigger build before testing with other players. Every minigame had to be balanced from zero, the link to the virus theme is gone, and each minigame needs explaining (the info box, D62).

**Player behavior:** Expected: switching tickets also means switching skill (visual search, memory, sorting, steering, precise cutting). Players learn which minigames survive being left and which don't:
- matched pairs stay
- the maze dot drifts back after 2 s
- packets leak
- SSD blocks grow back

They use that to choose what to leave.

**Counterfactual:**
- Removed (one minigame for every ticket): tickets differ only in arrival time, and players handle them first-in-first-out (D24).
- Doubled (ten minigames, two per ticket): more to learn than a short level can teach. The game drifts back toward the "five different games" of the first concept (D01 feedback), which pulls attention away from the choice between tickets.
