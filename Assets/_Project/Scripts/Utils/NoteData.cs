using UnityEngine;

[CreateAssetMenu(fileName = "NoteData", menuName = "Scriptable Objects/NoteData")]
public class NoteData : ItemData
{
    [TextArea(5, 20)]public string NoteDescription;
    public Sprite[] images;
}
