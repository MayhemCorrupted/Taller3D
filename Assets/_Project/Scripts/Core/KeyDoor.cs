using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(DoorController))]
public class KeyDoor : MonoBehaviour
{
    [Header("Key Settings")]
    [SerializeField] ItemData requiredKeyData;
    [SerializeField] bool noKeyOnUse = true;
    [Header("Events")]
    [SerializeField] UnityEvent OnCorrectKey;
    [SerializeField] UnityEvent OnWrongKey;  
    DoorController door;
    void Awake()
    {
        door = GetComponent<DoorController>();
    }
    public void TryUnlock(ItemData heldItem, Vector3 playerPos)
    {
        if (!door.IsLocked)
        {
            door.Interact(playerPos);
            return;
        }
        if (heldItem != null && heldItem == requiredKeyData)
        {
            OnCorrectKey?.Invoke();
            if (noKeyOnUse)
            {
                InventoryManager.Instance.RemoveItem(heldItem);
                EquipmentManager.Instance.Unequip();
            }
        }
        else OnWrongKey?.Invoke();
    }
    public bool HasCorrectKey()
    {
        ItemData heldItem = EquipmentManager.Instance.CurrentEquippedItem;
        return heldItem != null && heldItem == requiredKeyData;
    }
}
