using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class Grabbable : InteractableBase
{
    [SerializeField] private string partId = "new-filter";
    [SerializeField] private Transform holdPoint;

    private Rigidbody _rb;
    public string PartId => partId;
    [ShowInDebug] public bool IsHeld { get; private set; }
    public event Action<Grabbable> OnPickedUp;

    public override string Prompt => IsHeld ? "Drop" : "Pick Up";

    public override void Interact()
    {
        if (IsHeld)
            Drop();
        else
            PickUp();
    }

    private void PickUp()
    {
        IsHeld = true;
        _rb.isKinematic = true;
        Attach(holdPoint);
        OnPickedUp?.Invoke(this);
    }

    private void Drop()
    {
        IsHeld = false;
        transform.SetParent(null, true);
        _rb.isKinematic = false;
    }

    public void LockInto(Transform slot)
    {
        IsHeld = false;
        _rb.isKinematic = true;
        Attach(slot);
    }

    private void Attach(Transform parent)
    {
        transform.SetParent(parent, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
}
