using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Scrollbar))]
public class FuseButtonTrigger : MonoBehaviour, IPointerClickHandler, IDragHandler
{
    [SerializeField] Puzzle_Switch manager;
    [Tooltip("Índice del fusible a activar, de 0 a 3 para el array")]
    [SerializeField] [Range(0,3)] int fuseIndex;
    
    Scrollbar scrollbar;
    void Awake() => scrollbar = GetComponent<Scrollbar>();
    public void OnPointerClick(PointerEventData eventData)
    {
        if (scrollbar == null) return;
        scrollbar.value = (scrollbar.value > 0.5f) ? 0f : 1f;
        if (manager != null) manager.OnFuseClicked(fuseIndex);
    }
    public void OnIniatilizePotentialDrag(PointerEventData eventData) 
    {
        eventData.useDragThreshold = false;
    }
    public void OnDrag(PointerEventData eventData) {}
}
