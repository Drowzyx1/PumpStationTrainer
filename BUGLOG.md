# Bug Log

Real bugs I hit while building this project: what I saw, how I tracked it down, what caused it, and what I changed.

---

## Memory leak from a static event subscription (deliberate test)

**Date:** Nov 13, 2026 · **Area:** Unity (events, scene reloading)

**Symptom:** after reloading the scene a few times with R, changing the pressure filled the Console with `MissingReferenceException` errors, and the number of errors grew with every reload.

**How I found it:** read the stack trace, which pointed at `LeakyListener.Log`, and confirmed the pattern: one more batch of errors per reload. Then took Memory Profiler snapshots before and after 10 reloads and compared them: `LeakyListener` objects, each holding a 1 MB array, kept piling up instead of being freed.

**Cause:** `LeakyListener` subscribed to `PressureSystem.OnAnyPressureChanged` in `OnEnable` but never unsubscribed. Because the event is `static`, it survives scene reloads. Every reload added a new subscriber, while the destroyed listeners stayed referenced by the event. That meant (1) the garbage collector could never free them, so memory grew with every reload, and (2) the event kept calling methods on destroyed objects, which threw `MissingReferenceException`.

**Fix:** unsubscribe in `OnDisable()`, which runs when the object is destroyed during a reload:

```csharp
void OnDisable() => PressureSystem.OnAnyPressureChanged -= Log;
```

**Lesson:** every `+=` needs a matching `-=`, especially for static events or any event source that outlives its listeners.

---

## Listeners subscribing more than once

**Date:** Nov 7, 2026 · **Area:** C# console app (events)

**Symptom:** each valve toggle printed one, then two, then three sets of reactions as the program ran.

**How I found it:** noticed the count went up by exactly one on every pass of the parts loop.

**Cause:** the code that created `PressureSystem` and `ValveLogger` (which subscribe in their constructors) was inside a `foreach` loop, so every pass subscribed another pair of listeners to the same event.

**Fix:** moved the setup outside the loop so it runs once.

**Lesson:** each subscription runs its handler once per event; subscribing twice means reacting twice.

---

## Gauge needle jumping on the first valve change

**Date:** Nov 9, 2026 · **Area:** Unity (events, execution order)

**Symptom:** on the first valve change, the needle snapped from the 0 PSI position to near 3000, then animated normally.

**How I found it:** reasoned through when the gauge gets updated: only on `OnPressureChanged`, which only fires when the value changes.

**Cause:** the pressure starts at 3000 in `PressureSystem.Awake()`, but no event fires at startup, so the gauge kept showing its editor default (0 PSI) until the first change.

**Fix:** a one-time sync in `Gauge.Start()`, `UpdateNeedle(system.Psi)`. `Start` runs after every object's `Awake`, so the starting pressure is already set.

**Lesson:** event-driven displays need an initial sync; use `Start` when the value comes from another object's `Awake`.

---

## Scene changes missing from the repository

**Date:** Nov 10, 2026 · **Area:** Unity + Git

**Symptom:** the door and the first procedure step worked on my machine, but the pushed scene didn't contain them, and a step asset's instruction was blank.

**How I found it:** someone following my README noticed the pushed `SampleScene.unity` only had three steps.

**Cause:** the scene and the step asset had unsaved changes in the editor, so Git never saw them.

**Fix:** saved the scene (Ctrl+S) and the project (File → Save Project), checked `git status`, and recommitted.

**Lesson:** before every commit, save the scene and project, then check `git status`.

---

## New filter vanishing when picked up

**Date:** Nov 11, 2026 · **Area:** Unity (prefabs)

**Symptom:** the old filter could be picked up, but the new one disappeared when clicked.

**How I found it:** compared the two filters' Inspector fields: the new filter's Hold Point was empty.

**Cause:** the new filter was made from a prefab, and prefabs can't keep references to scene objects. The hold point is a child of the scene's camera, so the reference was dropped, and picking it up attached it to nothing (the world origin).

**Fix:** set the hold-point reference on the scene instance.

**Lesson:** scene-object references must be set on instances in the scene, or found at runtime, not stored in prefabs.

---

## Spark effects never returning to the pool

**Date:** Nov 11, 2026 · **Area:** Unity (object pooling, particles)

**Symptom:** sparks kept spawning forever after a mistake.

**How I found it:** the pooled object never switched off, so the coroutine waiting for it to finish never completed.

**Cause:** the particle system was looping with continuous emission, so it never reported itself as finished, and `WaitWhile(() => p.IsAlive(true))` waited forever.

**Fix:** a single non-looping burst (Looping off, Rate over Time 0, one burst of 30).

**Lesson:** pooling depends on knowing when an object is done; an effect that never ends never gets returned.
