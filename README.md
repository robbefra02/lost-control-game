# Lost Control (working title)

Prototype for KdG MCT2, 2.1 Principles of Game Design.

You are a servicedesk agent handling tickets from many PCs at once; each PC has a minigame to fix it. You can only see the ticket you are connected to; every other ticket goes stale. The prototype tests whether that hidden information makes players feel torn between tickets.

## Unity version

**Unity 6.3 LTS - 6000.3.7f1** (pinned). Open the project with exactly this version through Unity Hub.

## Project layout

```
Assets/
  Scripts/
    Model/      Pure C# simulation (tickets, minigames, connection), no MonoBehaviours
    View/       uGUI views that only read model state and pass input to it
    Config/     ScriptableObject types (LevelConfig, TicketDefinition, minigame settings)
  ScriptableObjects/  Config assets with all tunable values (Level1Config)
  Prefabs/      UI pieces created at runtime (ticket list entry, folder icon)
  Audio/        Sound files per cue (none yet: missing cues play a placeholder beep)
  Scenes/       The single game scene
```

All tunable values live in `Assets/ScriptableObjects/Level1Config.asset`; the handbook pages live in `Assets/ScriptableObjects/Handbook.asset`. Select it to edit them in the Inspector; fields marked `[ASSUMPTION]` in their tooltip are defaults still to be confirmed by the designer.

## Status

Building level 1 step by step. Milestone 2 deadline: 2026-10-21.

- [x] Step 0 - Project setup
- [x] Step 1 - Config data
- [x] Step 2 - Greybox layout
- [x] Step 3 - One virus, one motherboard
- [x] Step 4 - Damage
- [x] Step 5 - Tickets and list
- [x] Step 6 - Connecting and stale view
- [x] Step 7 - Win / lose / restart

After the first playtest the virus was replaced by minigames ([BUILD_PLAN_2.md](BUILD_PLAN_2.md)):

- [x] Step 11 - Minigame framework + Find the off-folder
- [ ] Step 12 - Playtest logging (postponed)
- [x] Step 13 - Handbook panel
- [x] Step 14 - Match the colors
- [x] Step 15 - Extinguish the firewall
- [ ] Step 16 - Maze
- [ ] Step 17 - Economy

## Build

No build is linked here: Build Plan 2 dropped the old step 10 (Windows build).
