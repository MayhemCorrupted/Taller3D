using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent (typeof(RectTransform))]
public class FuseDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] int fuseID;
    [SerializeField] float dragScaleMultiplier = 1.25f;  
    RectTransform rectTransform;
    CanvasGroup canvasGroup;
    Vector3 originalScale;

    Vector3 originalPosition;
    Transform originalParent;

    private Transform mainParent;
    private Vector3 mainLocalPosition;
    private Transform dragCanvasContainer;
    public int FuseID => fuseID;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalScale = transform.localScale;
    }
    void Start()
    {
        mainParent = transform.parent;
        mainLocalPosition = transform.localPosition;
        Canvas rootCanvas = GetComponentInParent<Canvas>();
        if (rootCanvas != null) dragCanvasContainer = rootCanvas.transform;
    }
    public void OnBeginDrag(PointerEventData eventData)
    { 
        originalPosition = transform.position;
        originalParent = transform.parent;
        
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.8f;

        transform.localScale = originalScale * dragScaleMultiplier;
        if (dragCanvasContainer != null) transform.SetParent(dragCanvasContainer, true);
        transform.SetAsLastSibling();
    }
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = Input.mousePosition;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        transform.localScale = originalScale;

        if (transform.parent == dragCanvasContainer)
        {
            transform.SetParent(originalParent);
            transform.position = originalPosition;
        }
    }
    public void ResetToInitialPosition()
    {
        transform.SetParent(mainParent);
        transform.localPosition = mainLocalPosition;
    }
}