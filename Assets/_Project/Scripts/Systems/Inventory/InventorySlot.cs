using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InventorySlot : MonoBehaviour
{
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] Button useItemButton;

    ItemData currentItem;

    public void Setup(ItemData item)
    {
        currentItem = item;
        itemIcon.sprite = item.sprite;
        itemName.text = item.itemName;
        useItemButton.onClick.AddListener(OnSlotClicked);
    }

    void OnSlotClicked()
    {
        switch(currentItem.itemType)
        {
            case ItemData.ItemType.Notes:
                ReadNote();
                break;
            case ItemData.ItemType.Interactable:
                UseInteractable();
                break;
        }


        InventoryManager.Instance.RemoveItem(currentItem);
    }
    void ReadNote()
    {
        NoteData note = currentItem as NoteData;
        if (note != null)
            NoteUI.Instance.OpenNote(note);
    }
    void UseInteractable()
    {
        InventoryManager.Instance.RemoveItem(currentItem);
    }

}
