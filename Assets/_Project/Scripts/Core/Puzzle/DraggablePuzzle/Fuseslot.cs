using UnityEngine;
using UnityEngine.EventSystems;

public class FuseSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] DraggablePuzzle puzzleManager;

    void Start()
    {
        puzzleManager = puzzleManager.gameObject.GetComponent<DraggablePuzzle>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        FuseDraggable existingFuse = GetComponentInChildren<FuseDraggable>();

        if (existingFuse == null)
        {
            GameObject droppedFuse = eventData.pointerDrag;

            if (droppedFuse.TryGetComponent(out FuseDraggable fuseLogic) && fuseLogic != null)
            {
                droppedFuse.transform.SetParent(transform);
                droppedFuse.transform.localPosition = Vector3.zero;

                if (puzzleManager != null) puzzleManager.CheckWinCondition();
            }
        }
    }
}