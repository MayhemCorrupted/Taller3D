using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FuseDraggable : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Identificación")]
    [SerializeField] string fuseName = "F1";

    [Header("Tamaño del fusil")]
    [SerializeField] float fuseWidth = 100f;
    [SerializeField] float fuseHeight = 100f;
    [SerializeField] float fuseScale = 2f;

    [Header("Tamaño dentro del slot")]
    [Tooltip("Ancho cuando está colocado en un slot")]
    [SerializeField] float slotWidth = 80f;
    [Tooltip("Alto cuando está colocado en un slot")]
    [SerializeField] float slotHeight = 80f;
    [Tooltip("Scale cuando está en el slot")]
    [SerializeField] float slotScale = 1f;

    [Header("Colores")]
    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color draggingColor = new Color(1f, 1f, 1f, 0.65f);

    PuzzleFuseBox puzzle;
    Image img;
    Canvas rootCanvas;
    CanvasGroup canvasGroup;
    RectTransform rt;

    Transform originalParent;
    Vector2 originalAnchoredPos;

    bool isInSlot = false;
    int currentSlotIndex = -1;

    public int CurrentSlotIndex => currentSlotIndex;
    public string FuseName => fuseName;

    public void Init(PuzzleFuseBox manager)
    {
        puzzle = manager;
        img = GetComponent<Image>();
        rt = GetComponent<RectTransform>();
        rootCanvas = GetComponentInParent<Canvas>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        originalParent = transform.parent;
        originalAnchoredPos = rt.anchoredPosition;

        ApplyPoolSize();
        img.color = normalColor;
    }

    void LateUpdate()
    {
        if (isInSlot) ApplySlotSize();
        else ApplyPoolSize();
    }

    void ApplyPoolSize()
    {
        transform.localScale = Vector3.one * fuseScale;
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(fuseWidth, fuseHeight);
    }

    void ApplySlotSize()
    {
        transform.localScale = Vector3.one * slotScale;
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(slotWidth, slotHeight);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentSlotIndex >= 0)
        {
            puzzle.GetSlot(currentSlotIndex)?.ClearSlot();
            currentSlotIndex = -1;
        }

        isInSlot = false;
        transform.SetParent(rootCanvas.transform, true);
        ApplyPoolSize();

        canvasGroup.blocksRaycasts = false;
        img.color = draggingColor;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rt.anchoredPosition += eventData.delta / rootCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        img.color = normalColor;

        if (currentSlotIndex < 0)
            ReturnToPool();
    }

    public void SnapToSlot(FuseSlot slot)
    {
        currentSlotIndex = slot.SlotIndex;
        isInSlot = true;

        transform.SetParent(slot.transform, false);
        ApplySlotSize();
        rt.anchoredPosition = Vector2.zero;
        img.color = normalColor;
    }

    public void ReturnToPool()
    {
        currentSlotIndex = -1;
        isInSlot = false;

        transform.SetParent(originalParent, false);
        ApplyPoolSize();
        rt.anchoredPosition = originalAnchoredPos;
        img.color = normalColor;
    }
}