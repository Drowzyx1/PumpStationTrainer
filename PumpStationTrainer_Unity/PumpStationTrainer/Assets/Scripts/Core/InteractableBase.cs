using UnityEngine;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [SerializeField] private string displayName = "Part";
    public virtual string Prompt => $"Use {displayName}";
    public virtual bool CanInteract => true;
    public abstract void Interact();
}
