using UnityEngine;
using System;

public class FilterSocket : MonoBehaviour
{

    [SerializeField] private Transform slot;
    [SerializeField] private string acceptsPartId = "new-filter";
    [SerializeField] private Grabbable startingPart;

    private Grabbable _current = null;
    public string CurrentPartId => _current != null ? _current.PartId : "";
    public event Action<string> OnContentsChanged;

    void Start()
    {
        if (startingPart != null)
            Insert(startingPart);
    }

    private void Insert(Grabbable part)
    {
        _current = part;
        part.LockInto(slot);
        part.OnPickedUp += HandleRemoved;
        OnContentsChanged?.Invoke(CurrentPartId);
    }

    void OnTriggerEnter(Collider other)
    {
        if (_current) return;
        if (!other.TryGetComponent(out Grabbable part)) return;
        if (part.IsHeld) return;
        if (part.PartId != acceptsPartId) return;

        Insert(part);
    }

    private void HandleRemoved(Grabbable part)
    {
        part.OnPickedUp -= HandleRemoved;
        _current = null;
        OnContentsChanged?.Invoke(CurrentPartId);
    }
}
