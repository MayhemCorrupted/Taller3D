using UnityEngine;
using UnityEngine.Events;
public class Item : MonoBehaviour
{
    [Header("Data")]
    public ItemData itemData;
    [SerializeField] bool isPickable = true;
    [SerializeField] UnityEvent OnPicked;
    [SerializeField] UnityEvent OnUnpickable;
    [SerializeField] string itemPrompt;
    public string ItemPrompt => itemPrompt;
    public bool IsPickable { set { isPickable = value; } }
    public void PickUp()
    {
        if (!isPickable)
        {
            OnUnpickable?.Invoke();
            return;
        }
        if (itemData == null)
        {
            Debug.LogError($"[Item] ItemData no asignado en '{gameObject.name}'.");
            return;
        }
        bool pickedUp = false;
        if (itemData.itemType == ItemData.ItemType.Notes)
        {
            if (itemData is NoteData note)
            {
                NotesManager.Instance.AddNote(note);
                pickedUp = true;
            }
        }
        else if (itemData.itemType == ItemData.ItemType.Interactable) pickedUp = InventoryManager.Instance.AddItem(itemData);

        if (pickedUp)
        {
            OnPicked?.Invoke();
            gameObject.SetActive(false);
        }

    }
}