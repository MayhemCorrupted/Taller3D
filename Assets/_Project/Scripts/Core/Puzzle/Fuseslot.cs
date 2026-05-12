using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FuseSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Visuals")]
    [SerializeField] Image backgroundImage;
    [SerializeField] Color normalColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    [SerializeField] Color hoverColor = new Color(0.4f, 0.6f, 1f, 1f);
    [SerializeField] Color occupiedColor = new Color(0.15f, 0.15f, 0.15f, 1f);

    PuzzleFuseBox puzzle;
    FuseDraggable occupant;
    int slotIndex;

    public bool IsOccupied => occupant != null;
    public FuseDraggable Occupant => occupant;
    public int SlotIndex => slotIndex;

    public void Init(PuzzleFuseBox manager)
    {
        puzzle = manager;
        slotIndex = transform.GetSiblingIndex();
        SetColor(normalColor);
    }

    public void OnDrop(PointerEventData eventData)
    {
        FuseDraggable dragged = eventData.pointerDrag?.GetComponent<FuseDraggable>();
        if (dragged == null) return;

        if (IsOccupied && occupant != dragged)
        {
            occupant.ReturnToPool();
        }

        PlaceFuse(dragged);
    }

    public void PlaceFuse(FuseDraggable fuse)
    {
        if (fuse.CurrentSlotIndex >= 0)
        {
            FuseSlot previous = puzzle.GetSlot(fuse.CurrentSlotIndex);
            if (previous != null && previous != this)
                previous.ClearSlot();
        }

        occupant = fuse;
        fuse.SnapToSlot(this);
        SetColor(occupiedColor);
        puzzle.CheckSolution();
    }

    public void ClearSlot()
    {
        occupant = null;
        SetColor(normalColor);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsOccupied) SetColor(hoverColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetColor(IsOccupied ? occupiedColor : normalColor);
    }

    void SetColor(Color c)
    {
        if (backgroundImage != null) backgroundImage.color = c;
    }
}