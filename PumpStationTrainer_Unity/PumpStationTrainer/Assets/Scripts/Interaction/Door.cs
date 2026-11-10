using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Door : InteractableBase
{
    [SerializeField] private float openAngle = 100f;
    [SerializeField] private float degreesPerSecond = 180f;
    private Rigidbody _rb;
    private Quaternion _openRotation;
    private Quaternion _closedRotation;

    [ShowInDebug] public bool IsOpen { get; private set; } = false;
    public override void Interact() { IsOpen = !IsOpen; }
    public override string Prompt => IsOpen ? "Close Panel" : "Open Panel";

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;

        _closedRotation = _rb.rotation;
        _openRotation = _closedRotation * Quaternion.Euler(0f, openAngle, 0f);
    }

    void FixedUpdate()
    {
        Quaternion target = IsOpen ? _openRotation : _closedRotation;
        float maxStep = degreesPerSecond * Time.fixedDeltaTime;

        _rb.MoveRotation(Quaternion.RotateTowards(_rb.rotation, target, maxStep));
    }
}
