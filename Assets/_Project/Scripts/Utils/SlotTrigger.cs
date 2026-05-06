using UnityEngine;
using UnityEngine.EventSystems;

public class SlotTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] int slotIndex;
    InventoryUI uiController;
    float lastTimeClicked;
    const float doubleClickThreshold = 0.3f;
    void Awake() => uiController = GetComponentInParent<InventoryUI>();
    public void OnPointerClick(PointerEventData eventData)
    {
        uiController.ShowItemDetails(slotIndex);
        uiController.ShowEquipPrompt(slotIndex, eventData.position);

        float timeLastClicked = Time.time - lastTimeClicked;
        if (timeLastClicked <= doubleClickThreshold) uiController.EquipFromSlot(slotIndex);
        else lastTimeClicked = Time.time;
    }
    public void OnPointerEnter(PointerEventData eventData) => uiController.ShowItemDetails(slotIndex);
    public void OnPointerExit(PointerEventData eventData) => uiController.ClearItemInfo();
}
