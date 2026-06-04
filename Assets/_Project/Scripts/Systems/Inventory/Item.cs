using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Data")]
    public ItemData itemData;
    [SerializeField] bool isPickable = true;
    
    public void PickUp()
    {
        if (!isPickable)
        {
            return;
        }
        if (itemData == null)
        {
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