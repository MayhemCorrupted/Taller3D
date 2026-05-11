using UnityEngine;

[CreateAssetMenu(fileName = "NoteData", menuName = "Scriptable Objects/NoteData")]
public class NoteData : ItemData
{
    [TextArea(5, 20)] public string NoteDescription;
    public Sprite image;

    [Header("Puzzle Note")]
    public bool isPuzzleNote;
    [HideInInspector] public string generatedCode;
}
