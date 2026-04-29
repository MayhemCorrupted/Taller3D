using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    [SerializeField] int MaxSlots = 5;
    List<ItemData> items = new();
    public event System.Action OnInventoryChanged;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public bool AddItem(ItemData item)
    {
        if (items.Count >= MaxSlots) return false;
        items.Add(item);
        OnInventoryChanged?.Invoke();
        return true;
    }
    public void RemoveItem(ItemData item)
    {
        if (items.Remove(item))
        {
            OnInventoryChanged?.Invoke();
        }
    }
    public List<ItemData> GetItems() => items;
}
