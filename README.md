# Pump Station Maintenance Trainer

> 🚧 **Work in progress.** A small virtual maintenance trainer I'm building to learn C# and Unity, started November 6, 2026. This README is updated as each milestone lands.

## What it is

A trainee follows a guided procedure to replace a clogged hydraulic filter on a pump station:

1. Open the access panel
2. Close the supply valve
3. Wait for system pressure to bleed off
4. Remove the clogged filter
5. Install the new filter
6. Reopen the valve and confirm pressure is restored

The finished Unity version will show each instruction on screen, track a step checklist, warn the trainee about unsafe actions (like removing the filter while the system is pressurized), and end with a summary of time taken and mistakes made.

## Progress

| Milestone                                                                         | Status           |
| --------------------------------------------------------------------------------- | ---------------- |
| 0 · Setup (Unity 6.3 LTS, .NET, Git)                                              | ✅ Done          |
| 1 · Pure C# model (console app)                                                   | ✅ Done          |
| 2 · Unity interaction: clickable valve, gauge driven by events, prefabs           | ⬜ Starting soon |
| 3 · Procedure system: steps as ScriptableObjects, UI, mistake detection           | ⬜               |
| 4 · Physics and polish: kinematic door, grabbable filters, socket, object pooling | ⬜               |
| 5 · Profiling, debugging and documentation                                        | ⬜               |

## Milestone 1: the C# model (`PumpSimConsole/`)

Before touching Unity, I built the simulation logic as a plain C# console app to practice the language and object-oriented design on their own.

**What's in it so far**

- **Abstract base class + inheritance:** `Component` holds shared state (`Name`, `Hours`) and declares `abstract bool IsFaulty()`. `Pump`, `Valve` and `Filter` each define their own fault rule and override `virtual Describe()`.
- **Encapsulation:** properties use `protected set`, so outside code can't change them; values come in through constructors chained with `: base(name, hours)`.
- **Interfaces:** `IInspectable` and `IRepairable` describe capabilities. `Filter` implements both, `Pump` is only inspectable, `Valve` only repairable, and the main loop checks each object with `is` rather than its concrete type.
- **Events:** `Valve` raises `OnToggled`. `PressureSystem` and `ValveLogger` subscribe and react independently; the valve has no reference to either (observer pattern).
- **Static class:** `Units` provides PSI ↔ bar conversions.
- **LINQ:** `Reports.PrintSummary` lists faulty parts (`Where` + `Select`), total hours (`Sum`) and parts sorted by hours (`OrderByDescending`).
- **Reflection + custom attribute:** `DebugReport` prints any object's properties tagged with `[ShowInDebug]`, discovered at runtime, so one generic method works for every class.

**Run it** (requires the .NET SDK):

```
cd PumpSimConsole
dotnet run
```

### Bugs found and fixed

- **Listeners subscribing more than once:** each toggle printed one, then two, then three sets of reactions. The listener setup was inside a `foreach` loop, so every pass subscribed new listeners to the same event. Moved it outside the loop so it runs once.
- **Project file missing from the repo:** the Unity `.gitignore` template ignores all `*.csproj` files, so the console app's project file was never committed. Added an exception for it.

## Project structure

```
PumpSimConsole/
├── Program.cs
├── Components/     Component, Pump, Valve, Filter
├── Interfaces/     IInspectable, IRepairable
├── Systems/        PressureSystem, ValveLogger
└── Utilities/      Units, Reports, DebugReport, ShowInDebugAttribute
```

## Next up

Milestone 2: the Unity scene. A clickable valve built on an `IInteractable` interface and an abstract `InteractableBase : MonoBehaviour`, a pressure system and gauge connected by events, and prefabs.
