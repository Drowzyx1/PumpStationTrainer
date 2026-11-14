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

| Milestone                                                                         | Status                                                                                       |
| --------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------- |
| 0 · Setup (Unity 6.3 LTS, .NET, Git)                                              | ✅ Done                                                                                      |
| 1 · Pure C# model (console app)                                                   | ✅ Done                                                                                      |
| 2 · Unity interaction: clickable valve, gauge driven by events, prefabs           | ✅ Done                                                                                      |
| 3 · Procedure system: steps as ScriptableObjects, UI, mistake detection           | ✅ Done                                                                                      |
| 4 · Physics and polish: kinematic door, grabbable filters, socket, object pooling | ✅ Done                                                                                      |
| 5 · Profiling, debugging and documentation                                        | 🟡 In progress: Profiler pass and memory-leak investigation done; event-driven refactor next |

## Milestone 5: profiling and debugging (in progress)

**Profiler pass:** I checked every script's per-frame cost in the Unity Profiler (CPU module → Hierarchy, sorted by GC Alloc). All of them allocate **0 B per frame**. To practice finding garbage, I added a string concatenation inside `PressureSystem.Update()`: the Profiler showed **58 B allocated every frame**, which I traced by expanding the Hierarchy down to the responsible method. I then removed it and verified 0 B again.

| Before                              | Introduced garbage                    | After                             |
| ----------------------------------- | ------------------------------------- | --------------------------------- |
| ![Before](docs/profiler-before.png) | ![Garbage](docs/profiler-garbage.png) | ![After](docs/profiler-after.png) |

**Memory-leak investigation:** I deliberately created a listener that subscribes to a **static** event without unsubscribing, then reloaded the scene repeatedly. Each reload added a subscriber that could never be freed (confirmed with Memory Profiler snapshots), and the event kept calling destroyed objects, throwing `MissingReferenceException`. The fix was unsubscribing in `OnDisable()`. Full write-up in [BUGLOG.md](BUGLOG.md).

## Milestones 3–4: the full procedure

The complete six-step procedure now runs end to end: open the access panel, close the supply valve, wait for the pressure to bleed off, remove the clogged filter, install the new one, reopen the valve. Each step ticks off a checklist, removing the filter while the system is still pressurized triggers a warning, and the run ends with a summary of total time, mistakes and the slowest step.

- **Steps as data (ScriptableObjects):** `TrainingStep` is an abstract `ScriptableObject` with an `instruction` and `abstract bool IsComplete(ProcedureContext ctx)`. `DoorOpenStep`, `ValveStateStep`, `PressureBelowStep` and `PartInSocketStep` each override it with their own rule, and each step is an asset created from the editor menu, so a new procedure can be built without writing code. One class can serve several steps with different data: `PartInSocketStep` with an empty id means "socket must be empty" (remove the filter), and with `new-filter` means "new filter installed."
- **Grabbable filters (`Grabbable`):** non-kinematic while loose, so physics drops them; kinematic while held (parented to a hold point on the camera) or installed. Each method sets the state it creates (`PickUp`, `Drop`, `LockInto`), with a shared `Attach` helper. Picking one up raises `OnPickedUp`.
- **Filter socket (`FilterSocket`):** a trigger collider that installs a dropped part through guard clauses (socket empty, is a `Grabbable`, not held, correct part id). It holds the installed part by reference (composition, not inheritance), subscribes to its `OnPickedUp` on insert and unsubscribes on removal, and exposes a computed `CurrentPartId`.
- **Mistake detection (`MistakeTracker`):** a second, independent listener on the old filter's `OnPickedUp`. If pressure is above a safe level, it records the mistake and raises `OnMistake`, which the UI shows as a warning.
- **Pooled spark effects (`SparkEffects`):** a burst of sparks at the socket on every mistake, reused from Unity's `ObjectPool<T>` instead of `Instantiate`/`Destroy` each time, which avoids garbage and GC spikes. A coroutine returns each effect to the pool once it finishes playing.
- **Kinematic access panel (`Door`):** a kinematic `Rigidbody` that code moves, not physics. `Interact()` only flips its target state; `FixedUpdate()` rotates it toward that target with `MoveRotation` and `Quaternion.RotateTowards`, so it swings smoothly and stays in sync with the physics system. The hinge is an empty parent at the panel's edge, so it pivots like a real door.
- **Assets vs scene data:** step assets live in the project, not the scene, so they can't reference scene objects directly. A `ProcedureContext` in the scene gives them access: it builds a `Dictionary<string, Valve>` of every valve keyed by its `Id` (LINQ `ToDictionary`), and `GetValve` logs a clear error for an unknown id instead of failing later with a null reference.
- **`ProcedureManager`:** runs the steps in order, calling `IsComplete` without knowing the step's concrete type (polymorphism). It times each step and raises `OnStepStarted` / `OnProcedureComplete`. Its state is exposed read-only (`IReadOnlyList`, get-only properties), and the first event fires in `Start()`, after every listener has subscribed in `OnEnable()`.
- **`ProcedureUI`:** fully event-driven, so nothing updates per frame. It shows the current instruction, rebuilds the checklist with a `StringBuilder`, and computes the end summary (total time, slowest step) with LINQ `Sum`, `Max` and `IndexOf`. Layout uses a Canvas Scaler and anchored TextMeshPro text with background panels.

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

Detailed write-ups (symptom, how I found it, cause, fix, lesson) are in [BUGLOG.md](BUGLOG.md).

- **Memory leak from a static event:** listeners that never unsubscribed from a static event survived scene reloads, leaking memory and throwing `MissingReferenceException`. Fixed by unsubscribing in `OnDisable()`.
- **Listeners subscribing more than once:** each toggle printed one, then two, then three sets of reactions. The listener setup was inside a `foreach` loop, so every pass subscribed new listeners to the same event. Moved it outside the loop so it runs once.
- **Project file missing from the repo:** the Unity `.gitignore` template ignores all `*.csproj` files, so the console app's project file was never committed. Added an exception for it.
- **Build output committed by accident:** `bin/` and `obj/` inside the console project were tracked because the root ignore rules only covered the top level. Untracked them and added `**/bin/` and `**/obj/`.
- **Nested Git repository:** Unity Hub created its own `.git` inside the Unity project, which would have made the main repo store only a pointer instead of the files. Removed the inner repo so the project lives in one repository.
- **Missing Input System package:** scripts using `UnityEngine.InputSystem` failed to compile until the Input System package was installed and the new input backend enabled.
- **Scene changes missing from the repo:** the door and the first step worked locally but weren't in the pushed scene, because the scene and a step asset had unsaved changes. Now I save the scene and the project before every commit.
- **New filter vanishing when picked up:** prefabs can't keep references to scene objects, so the filter prefab lost its hold-point reference, and the copy made from it attached to nothing. Set the reference on the scene instance.
- **Sparks never returning to the pool:** the particle system was looping with continuous emission, so it never reported itself finished, and the coroutine waited forever. Switched it to a single non-looping burst.
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
├── Interaction/    Valve, Interactor, Door, Grabbable
├── Player/         SimpleCameraController
├── Simulation/     PressureSystem, Gauge, FilterSocket
├── Procedure/      ProcedureContext, TrainingStep, ValveStateStep, PressureBelowStep,
│                   DoorOpenStep, PartInSocketStep, ProcedureManager, MistakeTracker
├── Effects/        SparkEffects
└── Debugging/      LeakyListener, SceneReloader (leak investigation)
└── UI/             ProcedureUI
```

## Next up

Finish Milestone 5: refactor `ProcedureManager` from polling every frame to reacting to events (valve, pressure, door and socket changes), so it does no work when nothing changes, then a demo GIF.

**Ideas for later:** make the drain valve speed up pressure bleed-off.
