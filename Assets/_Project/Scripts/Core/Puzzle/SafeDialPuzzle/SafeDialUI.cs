using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class SafeDialUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Dial Configuration")]
    [Tooltip("Cantidad total de números en el dial.")]
    [Range(2, 360)]
    public int maxDialNumbers = 100;

    [Header("Events")]
    public UnityEvent<int> OnNumberChanged;
    public UnityEvent<int> OnDialReleased;

    RectTransform dialRectTransform;
    float lastMouseAngle;
    bool isInteractable = true;
    int currentNumber = 0;
    float dragAngleSum = 0f;
    void Awake()
    {
        dialRectTransform = GetComponent<RectTransform>();
    }
    public void SetInteractable(bool state) => isInteractable = state;
    float GetMouseAngle(PointerEventData eventData)
    {
        RectTransform parentRect = dialRectTransform.parent as RectTransform;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, eventData.pressEventCamera, out Vector2 localMousePos))
        {
            Vector2 dir = localMousePos - (Vector2)dialRectTransform.localPosition;
            return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }
        return 0f;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isInteractable) return;
        lastMouseAngle = GetMouseAngle(eventData);
        dragAngleSum = 0f;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!isInteractable) return;

        float currentMouseAngle = GetMouseAngle(eventData);
        float angleDelta = Mathf.DeltaAngle(lastMouseAngle, currentMouseAngle);
        dragAngleSum += angleDelta;
        
        Vector3 currentRot = dialRectTransform.localEulerAngles;
        currentRot.z += angleDelta;
        dialRectTransform.localEulerAngles = currentRot;

        lastMouseAngle = currentMouseAngle;

        CalculateNumberFromAngle(currentRot.z);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isInteractable) return;
        if (Mathf.Abs(dragAngleSum) < 1f) return;

        OnDialReleased?.Invoke(currentNumber);
    }
    public int GetNetDragDirection()
    {
        return dragAngleSum <= 0 ? -1 : 1;
    }
    void CalculateNumberFromAngle(float angle)
    {
        float normalizedAngle = Mathf.Repeat(angle, 360f);
        int mappedNumber = Mathf.RoundToInt((normalizedAngle / 360f) * maxDialNumbers) % maxDialNumbers;

        if (mappedNumber != currentNumber)
        {
            currentNumber = mappedNumber;
            OnNumberChanged?.Invoke(currentNumber);
        }
    }
    public void ResetVisualDial()
    {
        dialRectTransform.localEulerAngles = Vector3.zero;
        lastMouseAngle = 0f;
        currentNumber = 0;
    }
}
