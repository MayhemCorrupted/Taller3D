using UnityEngine;

public class InventoryManager : BaseManager<InventoryManager>
{
    private const int MAX_SLOTS = 3;

    readonly ItemData[] dataItem = new ItemData[MAX_SLOTS];
    private int itemCount = 0;

    public event System.Action OnInventoryChanged;

    public int MaxSlots => MAX_SLOTS;
    public int ItemCount => itemCount;

    public bool AddItem(ItemData item)
    {
        if (item.itemType != ItemData.ItemType.Interactable) return false;
        if (itemCount >= MAX_SLOTS) return false;

        dataItem[itemCount] = item;
        itemCount++;
        OnInventoryChanged?.Invoke();
        return true;
    }

    public void RemoveItem(ItemData item)
    {
        for (int i = 0; i < itemCount; i++)
        {
            if (dataItem[i] != item) continue;

            for (int j = i; j < itemCount - 1; j++)
                dataItem[j] = dataItem[j + 1];

            dataItem[itemCount - 1] = null;
            itemCount--;
            OnInventoryChanged?.Invoke();
            return;
        }
    }

    public ItemData GetItem(int index)
    {
        return (index >= 0 && index < itemCount) ? dataItem[index] : null;
    }

    public ItemData[] GetAllItems()
    {
        ItemData[] copy = new ItemData[itemCount];
        System.Array.Copy(dataItem, copy, itemCount);
        return copy;
    }

    public void ClearInventory()
    {
        System.Array.Clear(dataItem, 0, MAX_SLOTS);
        itemCount = 0;
        OnInventoryChanged?.Invoke();
    }

    public bool IsFull() => itemCount >= MAX_SLOTS;
    public bool IsEmpty() => itemCount == 0;
}