using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float reach = 3f;
    [SerializeField] private LayerMask interactableMask;   // set to the Interactable layer

    void Update()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, reach, interactableMask)) return;

        // InParent: the collider may be on a child (e.g. the door's panel) while the script is on its parent
        IInteractable target = hit.collider.GetComponentInParent<IInteractable>();
        if (target != null && target.CanInteract)
            target.Interact();
    }
}