using UnityEngine;
using UnityEngine.Events;
public class Item : MonoBehaviour, IInteractable
{
    [Header("Data")]
    public ItemData itemData;
    [SerializeField] bool isPickable = true;
    [SerializeField] UnityEvent OnPicked;
    [SerializeField] UnityEvent OnUnpickable;
    [SerializeField] string itemPrompt = "[{key}] grab {item}";
    [SerializeField] string unpickablePrompt = "Cannot grab {item}";
    public string ItemPrompt => itemPrompt;
    public bool IsPickable { set { isPickable = value; } }
    #region referencias de la interfaz (el IInteractable)
    public string GetTextInteract()
    {
        string itemName = itemData != null ? itemData.itemName : gameObject.name;
        string selectedPrompt = isPickable ? itemPrompt : unpickablePrompt;
        return selectedPrompt.Replace("{item}", itemName);
    }
    public void Interact(Transform interactorTransform)
    {
        PickUp();
    }
    #endregion
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