using UnityEngine;

public class PuzzleBoxInteractable : MonoBehaviour
{
    [SerializeField] PuzzleFuseBox puzzle;
    public void Interact() => puzzle?.OnPlayerInteract();
}