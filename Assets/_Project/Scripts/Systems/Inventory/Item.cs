using UnityEngine;
using TMPro;

public class Item : MonoBehaviour
{
    [Header("Data")]
    public ItemData itemData;
    public void PickUp()
    {
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