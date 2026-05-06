using UnityEngine;
using UnityEngine.EventSystems;

public class SlotTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] int slotIndex;
    InventoryUI uiController;
    void Awake() => uiController = GetComponentInParent<InventoryUI>();
    public void OnPointerEnter(PointerEventData eventData) => uiController.ShowItemDetails(slotIndex);
    public void OnPointerExit(PointerEventData eventData) => uiController.ClearInfo();
}
