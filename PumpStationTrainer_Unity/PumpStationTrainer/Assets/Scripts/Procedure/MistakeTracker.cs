using System;
using System.Collections.Generic;
using UnityEngine;

public class MistakeTracker : MonoBehaviour
{
    [SerializeField] private PressureSystem pressure;
    [SerializeField] private Grabbable oldFilter;
    [SerializeField] private float safePsi = 1f;

    private readonly List<string> _mistakes = new();
    public IReadOnlyList<string> Mistakes => _mistakes;

    public event Action<string> OnMistake;

    void OnEnable() => oldFilter.OnPickedUp += CheckFilterRemoval;
    void OnDisable() => oldFilter.OnPickedUp -= CheckFilterRemoval;
    private void CheckFilterRemoval(Grabbable part)
    {
        if (pressure.Psi <= safePsi) return;

        string message = $"Removed {part.PartId} while the system was at {pressure.Psi:0} PSI";
        _mistakes.Add(message);
        OnMistake?.Invoke(message);
    }
}