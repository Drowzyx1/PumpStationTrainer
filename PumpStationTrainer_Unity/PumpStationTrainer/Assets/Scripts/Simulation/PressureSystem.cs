using System;          // needed for Action
using UnityEngine;

public class PressureSystem : MonoBehaviour
{
    [SerializeField] private Valve supplyValve;
    [SerializeField] private float maxPsi = 3000f;
    [SerializeField] private float psiPerSecond = 800f;

    [ShowInDebug] public float Psi { get; private set; }
    public event Action<float> OnPressureChanged;

    void Awake()
    {
        Psi = maxPsi;
    }

    void Update()
    {
        float target = supplyValve.IsOpen ? maxPsi : 0f;

        float next = Mathf.MoveTowards(Psi, target, psiPerSecond * Time.deltaTime);

        if (Mathf.Approximately(next, Psi)) return;

        Psi = next;
        OnPressureChanged?.Invoke(Psi);
    }
}