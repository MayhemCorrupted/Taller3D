using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.Events;
public enum DialogueType { UI, World_3D }
[Serializable] public struct DialogueLineData
{
    [TextArea(2, 4)]
    public string textLine;
    [Header("Configuración de Línea")]
    public DialogueType dialogueType;
    [Tooltip("Punto al que la cámara mirará automáticamente.")]
    public Transform targetToLookAt;
    [Tooltip("Opcional. Si se deja vacío, heredará el TextMesh de la línea anterior.")]
    public TMP_Text textMeshComponent;
    [Tooltip("Opcional. Si es 0, heredará la duración de la línea anterior.")]
    public float lineTypingDuration;
    [Header("Events")]
    public UnityEvent onLineStart;
}
[Serializable] public struct DialogueEntry
{
    [Tooltip("La llave exacta que usará el reader para encontrar este diálogo")]
    public string dialogueKey;

    [Header("Líneas de Diálogo")]
    public DialogueLineData[] dialogueSequence;
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
