using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class OpenObjects : MonoBehaviour, IInteractable
{
    public enum OpenMode { Drawer, Door }

    [Header("General Settings")]
    [Tooltip("Si usas un Empty como bisagra, arrástralo aquí. Si lo dejas vacío, animará este mismo objeto.")]
    [SerializeField] Transform targetToAnimate;
    [SerializeField] UnityEvent OnClosingObject;
    [SerializeField] UnityEvent OnOpeningObject;

    [Tooltip("¿Este objeto se desliza (Cajón) o gira (Puerta)?")]
    [SerializeField] OpenMode openMode = OpenMode.Drawer;
    [SerializeField] float animationSpeed = 1.5f;
    string interactPrompt = "[E] Open";

    [Header("Drawer Settings (Slide)")]
    [Tooltip("Dirección local hacia donde sale el cajón. Generalmente Z = (0, 0, 1) o X = (1, 0, 0)")] 
    [SerializeField] Vector3 slideAxis = Vector3.forward;
    [SerializeField] float slideDistance = 0.4f;

    [Header("Door Settings (Rotate)")]
    [Tooltip("Eje local sobre el cual gira la puerta. Generalmente Y = (0, 1, 0)")]
    [SerializeField] Vector3 rotationAxis = Vector3.up;
    [Tooltip("Grados de apertura. Usa valores negativos (ej. -90) para girar al lado contrario.")]
    [SerializeField] float rotationAngle = 90f;

    Vector3 closedPosition, openPosition;
    Quaternion closedRotation, openRotation;
    bool isOpen = false;
    bool isMoving = false;

    void Awake()
    {
        if (targetToAnimate == null) targetToAnimate = transform;

        closedPosition = targetToAnimate.localPosition;
        openPosition = closedPosition + (slideAxis.normalized * slideDistance);

        closedRotation = targetToAnimate.localRotation;
        openRotation = closedRotation * Quaternion.AngleAxis(rotationAngle, rotationAxis.normalized);
    }

    public string GetTextInteract() => interactPrompt;

    public void Interact(Transform interactorTransform)
    {
        UseObject();
    }

    public void UseObject()
    {
        if (isMoving) return;

        isOpen = !isOpen;
        interactPrompt = isOpen ? "[E] Close" : "[E] Open";

        if (isOpen) OnOpeningObject?.Invoke();
        else OnClosingObject?.Invoke();

        StopAllCoroutines();
        StartCoroutine(AnimateObject(isOpen));
    }

    IEnumerator AnimateObject(bool opening)
    {
        isMoving = true;
        float progress = 0f;

        Vector3 startPos = targetToAnimate.localPosition;
        Vector3 targetPos = opening ? openPosition : closedPosition;

        Quaternion startRot = targetToAnimate.localRotation;
        Quaternion targetRot = opening ? openRotation : closedRotation;

        while (progress < 1f)
        {
            progress += Time.deltaTime * animationSpeed;
            float smoothT = Mathf.SmoothStep(0f, 1f, progress);

            if (openMode == OpenMode.Drawer) targetToAnimate.localPosition = Vector3.Lerp(startPos, targetPos, smoothT);
            else targetToAnimate.localRotation = Quaternion.Slerp(startRot, targetRot, smoothT);

            yield return null;
        }

        if (openMode == OpenMode.Drawer) targetToAnimate.localPosition = targetPos;
        else targetToAnimate.localRotation = targetRot;

        isMoving = false;
    }
}