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
            FuseDraggable fuseLogic = droppedFuse.GetComponent<FuseDraggable>();

            if (fuseLogic != null)
            {
                droppedFuse.transform.SetParent(transform);

                droppedFuse.transform.localPosition = Vector3.zero;

                if (puzzleManager != null)
                {
                    puzzleManager.CheckWinCondition();
                }
            }
        }
    }
}