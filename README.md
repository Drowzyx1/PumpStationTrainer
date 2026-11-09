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

| Milestone                                                                         | Status  |
| --------------------------------------------------------------------------------- | ------- |
| 0 · Setup (Unity 6.3 LTS, .NET, Git)                                              | ✅ Done |
| 1 · Pure C# model (console app)                                                   | ✅ Done |
| 2 · Unity interaction: clickable valve, gauge driven by events, prefabs           | ✅ Done |
| 3 · Procedure system: steps as ScriptableObjects, UI, mistake detection           | ⬜      |
| 4 · Physics and polish: kinematic door, grabbable filters, socket, object pooling | ⬜      |
| 5 · Profiling, debugging and documentation                                        | ⬜      |

## Milestone 2: Unity interaction (`PumpStationTrainer_Unity/`)

A gray-box pump station scene in Unity 6.3 LTS. You can walk around, close the supply valve, and watch the pressure gauge bleed down to 0, then rise again when you reopen it.

**What's in it so far**

- **Interface + abstract MonoBehaviour:** `IInteractable` defines what anything clickable must have (`Prompt`, `CanInteract`, `Interact()`). `InteractableBase` is an abstract `MonoBehaviour` that implements it, with `virtual` defaults for `Prompt` and `CanInteract` and an `abstract Interact()` every subclass must write.
- **`Valve`:** inherits `InteractableBase`, overrides `Prompt` ("Open/Close Valve") and `Interact()`, rotates its handle, and raises `OnToggled(Valve, bool)` so other systems can react without the valve knowing about them.
- **`Interactor`:** raycasts from the camera on click, filtered by an `Interactable` layer mask, and uses `GetComponentInParent<IInteractable>()`, so it works with any interactable, whatever its concrete type, even when the collider sits on a child object.
- **`SimpleCameraController`:** right-mouse look and WASD movement using Unity's new Input System, frame-rate independent via `Time.deltaTime`.
- **`PressureSystem`:** moves pressure toward 3000 PSI or 0 depending on the valve, using `Mathf.MoveTowards` and `Time.deltaTime`. It raises `OnPressureChanged` **only when the value actually changes** (checked with `Mathf.Approximately`), so listeners do no work while pressure is steady.
- **`Gauge`:** subscribes in `OnEnable`, unsubscribes in `OnDisable` (no leaked listeners), and converts PSI to a needle angle with `Mathf.Clamp01` and `Mathf.Lerp`. A one-time sync in `Start` sets the needle to the starting pressure.
- **Prefabs:** the valve and gauge are prefabs. A second valve instance overrides its `id` to `drain`, showing prefab data vs per-instance overrides.
- **Inspector-driven setup:** fields are `[SerializeField] private`, editable in the Inspector but hidden from other code. Materials give each part a readable color.

**Run it:** open `PumpStationTrainer_Unity` in Unity Hub (Unity 6.3 LTS), load the scene, press Play. Hold right mouse to look, WASD to move, left-click the supply valve and watch the gauge.

## Milestone 1: the C# model (`PumpSimConsole/`)

Before touching Unity, I built the simulation logic as a plain C# console app to practice the language and object-oriented design on their own.

- **Abstract base class + inheritance:** `Component` holds shared state (`Name`, `Hours`) and declares `abstract bool IsFaulty()`. `Pump`, `Valve` and `Filter` each define their own fault rule and override `virtual Describe()`.
- **Encapsulation:** properties use `protected set`, so outside code can't change them; values come in through constructors chained with `: base(name, hours)`.
- **Interfaces:** `IInspectable` and `IRepairable` describe capabilities. `Filter` implements both, `Pump` is only inspectable, `Valve` only repairable, and the code checks each object with `is` rather than its concrete type.
- **Events:** `Valve` raises `OnToggled`. `PressureSystem` and `ValveLogger` subscribe and react independently; the valve has no reference to either (observer pattern).
- **Static class:** `Units` provides PSI ↔ bar conversions.
- **LINQ:** `Reports.PrintSummary` lists faulty parts (`Where` + `Select`), total hours (`Sum`) and parts sorted by hours (`OrderByDescending`).
- **Reflection + custom attribute:** `DebugReport` prints any object's properties tagged with `[ShowInDebug]`, discovered at runtime, so one generic method works for every class.
- **Interactive menu:** inspect parts, toggle the valve, print the LINQ report, or print the debug report.

**Run it** (requires the .NET SDK):

```
cd PumpSimConsole
dotnet run
```

## Bugs found and fixed

- **Listeners subscribing more than once:** each toggle printed one, then two, then three sets of reactions. The listener setup was inside a `foreach` loop, so every pass subscribed new listeners to the same event. Moved it outside the loop so it runs once.
- **Project file missing from the repo:** the Unity `.gitignore` template ignores all `*.csproj` files, so the console app's project file was never committed. Added an exception for it.
- **Build output committed by accident:** `bin/` and `obj/` inside the console project were tracked because the root ignore rules only covered the top level. Untracked them and added `**/bin/` and `**/obj/`.
- **Nested Git repository:** Unity Hub created its own `.git` inside the Unity project, which would have made the main repo store only a pointer instead of the files. Removed the inner repo so the project lives in one repository.
- **Missing Input System package:** scripts using `UnityEngine.InputSystem` failed to compile until the Input System package was installed and the new input backend enabled.
- **Gauge needle jumping on the first valve change:** the gauge only updates on `OnPressureChanged`, which fires only on change, so at startup it still showed 0 PSI while the system was at 3000. Added an initial sync in `Start()`, which runs after every `Awake()`, so the pressure system has already set its starting value.

## Project structure

```
PumpSimConsole/
├── Program.cs
├── Components/     Component, Pump, Valve, Filter
├── Interfaces/     IInspectable, IRepairable
├── Systems/        PressureSystem, ValveLogger
└── Utilities/      Units, Reports, DebugReport, ShowInDebugAttribute

PumpStationTrainer_Unity/.../Assets/Scripts/
├── Core/           ShowInDebugAttribute, IInteractable, InteractableBase
├── Interaction/    Valve, Interactor
├── Player/         SimpleCameraController
└── Simulation/     PressureSystem, Gauge
```

## Next up

Milestone 3: the procedure system. Each training step becomes a `ScriptableObject` asset built on an abstract `TrainingStep` class, a `ProcedureManager` runs them in order, and an on-screen UI shows the current instruction and a checklist.

**Ideas for later:** make the drain valve speed up pressure bleed-off.
