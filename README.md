# Startup Quest XP Patcher

Startup Quest XP Patcher is a Skyrim Special Edition Synthesis patcher that
prevents selected startup and initialization quests from awarding quest XP when
using the **Experience** leveling mod.

## Supported mods

Currently supported:

- **Alternate Start - Live Another Life**
- **Frostfall**
- **SunHelm**

The following quests are patched:

- `ARTHLALChargenQuest` — **A Second Chance**
- `ARTHLALRumorsOfWarQuest` — **Live Another Life**
- `_Frost_TrackingQuest` — **Frostfall**
- `_SHStartPrompt` — **SunHelm**

For each quest, the patcher changes:

```text
QUST -> DNAM -> Type
Main Quest / Side Quest -> None
```

The patcher resolves the current winning override and changes only the quest
type. Downstream changes to VMAD, scripts, aliases, stages, objectives,
objective targets, conditions, and other quest data are preserved.

If a quest is already set to `None`, no unnecessary override is created. If
a supported mod is not installed, that mod is skipped without creating quest
overrides. Each supported mod is detected and patched independently.

## Installation and use

1. Add this patcher repository to Synthesis.
2. Run it after the relevant gameplay and compatibility mods.
3. During early testing, place it in its own Synthesis group so it generates a
   separate ESP.
4. Inspect the generated output in xEdit if desired.
