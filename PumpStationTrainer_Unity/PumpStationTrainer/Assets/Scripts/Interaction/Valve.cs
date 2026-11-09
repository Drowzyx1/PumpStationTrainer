using UnityEngine;
using System;

public class Valve : InteractableBase
{
    [SerializeField] private string id = "supply";
    [SerializeField] private Transform handle;
    [SerializeField] private float closedAngle = 90f;
    public string Id { get => id; }

    [ShowInDebug] public bool IsOpen { get; private set; } = true;
    public event Action<Valve, bool> OnToggled;

    public override string Prompt => IsOpen ? "Close Valve" : "Open Valve";
    public override void Interact()
    {
        IsOpen = !IsOpen;
        handle.localRotation = Quaternion.Euler(0f, 0f, IsOpen ? 0f : closedAngle);
        OnToggled?.Invoke(this, IsOpen);
    }
}
