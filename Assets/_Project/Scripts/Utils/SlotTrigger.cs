using UnityEngine;
using UnityEngine.EventSystems;

public class SlotTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] InventoryUI uiController;
    [SerializeField] int slotIndex;
    float lastTimeClicked;
    const float doubleClickThreshold = 0.3f;
    public void OnPointerClick(PointerEventData eventData)
    {
        uiController.ShowItemDetails(slotIndex);
        uiController.ShowEquipPrompt(slotIndex, eventData.position);

        float timeLastClicked = Time.time - lastTimeClicked;
        if (timeLastClicked <= doubleClickThreshold && !uiController.IsSlotEquipped(slotIndex)) 
            uiController.EquipFromSlot(slotIndex);
        else lastTimeClicked = Time.time;
    }
    public void OnPointerEnter(PointerEventData eventData) => uiController.ShowItemDetails(slotIndex);
    public void OnPointerExit(PointerEventData eventData) => uiController.ClearItemInfo();
}
