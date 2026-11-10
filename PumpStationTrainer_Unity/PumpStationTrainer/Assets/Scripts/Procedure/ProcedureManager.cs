using System.Runtime.Serialization;
using UnityEngine;
using System.Collections.Generic;
using System;

public class ProcedureManager : MonoBehaviour
{
    [SerializeField] private ProcedureContext context;
    [SerializeField] private List<TrainingStep> steps = new();

    private int _index;
    private float _stepStart;
    private readonly List<float> _stepTimes = new();

    public IReadOnlyList<TrainingStep> Steps => steps;
    public IReadOnlyList<float> StepTimes => _stepTimes;
    public int CurrentIndex => _index;
    public TrainingStep Current => _index < steps.Count ? steps[_index] : null;

    public event Action<TrainingStep> OnStepStarted;
    public event Action OnProcedureComplete;
    void Start()
    {
        _stepStart = Time.time;
        OnStepStarted?.Invoke(Current);
    }

    void Update()
    {
        if (Current == null || !Current.IsComplete(context)) return;

        _stepTimes.Add(Time.time - _stepStart);

        _stepStart = Time.time;

        _index++;

        if (Current == null) OnProcedureComplete?.Invoke();
        else OnStepStarted?.Invoke(Current);
    }
}
