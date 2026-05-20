using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(DoorController))]
public class KeyDoor : MonoBehaviour
{
    [Header("Key Settings")]
    [SerializeField] ItemData requiredKeyData;
    [SerializeField] bool noKeyOnUse = true;
    [SerializeField] string keyTextPrompt = "[E] Usar llave"; 
    [Header("Events")]
    [SerializeField] UnityEvent OnCorrectItem;
    [SerializeField] UnityEvent OnWrongItem;  
    DoorController door;
    public string KeyTextPrompt => keyTextPrompt;
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
            OnCorrectItem?.Invoke();
            if (noKeyOnUse)
            {
                InventoryManager.Instance.RemoveItem(heldItem);
                EquipmentManager.Instance.Unequip();
            }
        }
        else
        {
            door.Interact(playerPos);
            OnWrongItem?.Invoke();
        }
    }
    public bool HasCorrectKey()
    {
        ItemData heldItem = EquipmentManager.Instance.CurrentEquippedItem;
        return heldItem != null && heldItem == requiredKeyData;
    }
}
