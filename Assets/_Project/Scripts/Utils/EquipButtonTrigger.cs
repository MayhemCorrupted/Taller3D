using UnityEngine;
using UnityEngine.EventSystems;

public class EquipButtonTrigger : MonoBehaviour, IPointerExitHandler
{
    private InventoryUI uiInventory;
    void Awake() => uiInventory = Object.FindFirstObjectByType<InventoryUI>();
    public void OnPointerExit(PointerEventData eventData)
    {
        if (uiInventory != null) uiInventory.HideEquipPrompt();
    }
}
