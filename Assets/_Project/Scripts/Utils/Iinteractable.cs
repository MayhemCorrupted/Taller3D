using UnityEngine;
public interface IInteractable 
{
    string GetTextInteract();
    void Interact(Transform interactorTransform);
}
