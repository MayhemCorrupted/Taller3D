using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PuzzleDoorInteractable : MonoBehaviour
{
    [SerializeField] PuzzleKitchenDoor puzzle;

    public void Interact() => puzzle?.OnPlayerInteract();
}
