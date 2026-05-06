using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData itemData;
    public void PickUp()
    {
        if (itemData == null)
        {
            Debug.LogError("ItemData is not assigned for " + gameObject.name);
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
        else if (itemData.itemType == ItemData.ItemType.Interactable)
        {
            pickedUp = InventoryManager.Instance.AddItem(itemData);
        }
        if (pickedUp) gameObject.SetActive(false);
    }
}
