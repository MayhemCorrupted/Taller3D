using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public struct DialogueEntry
{
    [Tooltip("La llave exacta que usará el reader para encontrar este diálogo")]
    public string dialogueKey;
    [TextArea(3, 5)]
    public string[] dialogueLines;
    public TMP_Text uiTextComponent;
    public Transform targetToLookAt;
}
public class Dialogues : MonoBehaviour
{
    public static Dialogues Instance { get; private set; }
    [SerializeField] List<DialogueEntry> dialogueEntries = new();
    readonly Dictionary<string, DialogueEntry> library = new();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (var entry in dialogueEntries)
        {
            if (!library.ContainsKey(entry.dialogueKey)) library.Add(entry.dialogueKey, entry);
        }
    }
    public DialogueEntry? GetDialogue(string key)
    {
        if (library.TryGetValue(key, out var entry)) return entry;
        Debug.LogWarning($"[DialogueLibrary] No se encontró el diálogo con la llave: {key}");
        return null;
    }
}
