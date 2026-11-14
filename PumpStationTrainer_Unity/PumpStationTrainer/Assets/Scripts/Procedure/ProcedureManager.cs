using System;
using System.Collections.Generic;
using UnityEngine;

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
        foreach (Valve v in context.Valves) v.OnToggled += HandleValve;
        context.Pressure.OnPressureChanged += HandlePressure;
        context.AccessPanel.OnToggled += HandleDoor;
        context.FilterSocket.OnContentsChanged += HandleSocket;

        _stepStart = Time.time;
        OnStepStarted?.Invoke(Current);
        CheckCurrentStep();
    }

    void OnDestroy()
    {
        if (context == null) return;
        foreach (Valve v in context.Valves) v.OnToggled -= HandleValve;
        context.Pressure.OnPressureChanged -= HandlePressure;
        context.AccessPanel.OnToggled -= HandleDoor;
        context.FilterSocket.OnContentsChanged -= HandleSocket;
    }

    private void HandleValve(Valve valve, bool isOpen) => CheckCurrentStep();
    private void HandlePressure(float psi) => CheckCurrentStep();
    private void HandleDoor(bool isOpen) => CheckCurrentStep();
    private void HandleSocket(string partId) => CheckCurrentStep();

    private void CheckCurrentStep()
    {
        while (Current != null && Current.IsComplete(context))
        {
            _stepTimes.Add(Time.time - _stepStart);
            _stepStart = Time.time;
            _index++;

            if (Current == null) OnProcedureComplete?.Invoke();
            else OnStepStarted?.Invoke(Current);
        }
    }
}