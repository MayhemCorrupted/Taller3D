using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Data")]
    public ItemData itemData;
    [SerializeField] bool isPickable = true;
    public bool IsPickable { set { isPickable = value; } }
    public void PickUp()
    {
        if (!isPickable)
        {
            Debug.LogWarning($"[Item] El item '{gameObject.name}' no es recogible.");
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
        if (pickedUp) gameObject.SetActive(false);
    }
}