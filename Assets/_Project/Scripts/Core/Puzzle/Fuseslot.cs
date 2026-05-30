using UnityEngine;
using UnityEngine.EventSystems;

public class FuseSlot : MonoBehaviour, IDropHandler
{
    DraggablePuzzle puzzleManager;

    void Awake()
    {
        puzzleManager = FindFirstObjectByType<DraggablePuzzle>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
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