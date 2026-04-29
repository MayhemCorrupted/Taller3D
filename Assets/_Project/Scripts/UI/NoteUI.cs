using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NoteUI : MonoBehaviour
{
    public static NoteUI Instance { get; private set; }

    [SerializeField] GameObject notePanel;
    [SerializeField] TextMeshProUGUI noteTitle;
    [SerializeField] TextMeshProUGUI noteText;
    [SerializeField] Image noteImage;
    [SerializeField] Button closeButton;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        notePanel.SetActive(false);
        closeButton.onClick.AddListener(CloseNote);
    }

    public void OpenNote(NoteData note)
    {
        notePanel.SetActive(true);
        noteTitle.text = note.name;
        noteText.text = note.NoteDescription;

        if (note.images != null && note.images.Length > 0)
        {
            noteImage.gameObject.SetActive(true);
            noteImage.sprite = note.images[0];
        }
        else noteImage.gameObject.SetActive(false);
    }

    public void CloseNote()
    {
        notePanel.SetActive(false);
    }
}
