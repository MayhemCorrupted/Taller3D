using UnityEngine;
using UnityEngine.EventSystems;

public class EquipButtonTrigger : MonoBehaviour, IPointerExitHandler
{
    InventoryUI inventoryUI;

    void Awake() => inventoryUI = Object.FindFirstObjectByType<InventoryUI>();

    public void OnPointerExit(PointerEventData eventData)
    {
        if (inventoryUI != null) inventoryUI.HideEquipPrompt();
    }
}