# Startup Quest XP Patcher

Startup Quest XP Patcher is a Skyrim Special Edition Synthesis patcher that
prevents selected startup and initialization quests from awarding quest XP when
using the **Experience** leveling mod.

## Supported mods

Currently supported:

- **Alternate Start - Live Another Life**

The following quests are patched:

- `ARTHLALChargenQuest` — **A Second Chance**
- `ARTHLALRumorsOfWarQuest` — **Live Another Life**

For each quest, the patcher changes:

```text
QUST -> DNAM -> Type
Main Quest -> None
```

The patcher resolves the current winning override and changes only the quest
type. Downstream changes to VMAD, scripts, aliases, stages, objectives,
objective targets, conditions, and other quest data are preserved.

If a quest is already set to `None`, no unnecessary override is created. If
Alternate Start is not installed, the patcher exits successfully without
creating quest overrides.

Additional startup mods, such as Frostfall, may be supported in the future.

## Installation and use

1. Add this patcher repository to Synthesis.
2. Run it after the relevant gameplay and compatibility mods.
3. During early testing, place it in its own Synthesis group so it generates a
   separate ESP.
4. Inspect the generated output in xEdit if desired.
