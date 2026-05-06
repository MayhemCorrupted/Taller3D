using UnityEngine;
using TMPro;

public class Item : MonoBehaviour
{
    [Header("Data")]
    public ItemData itemData;

    [Header("Feedback (opcional)")]
    [Tooltip("Texto TMP en el mundo o en el HUD para mostrar '¡X recogido!'")]
    [SerializeField] TextMeshProUGUI pickupFeedbackText;
    [SerializeField] float feedbackDuration = 1.5f;

    public void PickUp()
    {
        if (itemData == null)
        {
            Debug.LogError($"[Item] ItemData no asignado en '{gameObject.name}'.");
            return;
        }

        bool pickedUp = TryAddToSystem();

        if (pickedUp)
        {
            ShowFeedback($"{itemData.itemName} recogido");
            gameObject.SetActive(false);
        }
        else
        {
            ShowFeedback("Inventario lleno");
            Debug.Log($"[Item] No se pudo recoger '{itemData.itemName}'.");
        }
    }

    bool TryAddToSystem()
    {
        switch (itemData.itemType)
        {
            case ItemData.ItemType.Notes:
                return TryAddNote();

            case ItemData.ItemType.Interactable:
                return TryAddToInventory();

            default:
                Debug.LogWarning($"[Item] Tipo desconocido para '{itemData.itemName}'.");
                return false;
        }
    }

    bool TryAddNote()
    {
        if (itemData is NoteData note)
        {
            if (NotesManager.Instance == null)
            {
                Debug.LogError("[Item] NotesManager no encontrado en la escena.");
                return false;
            }
            NotesManager.Instance.AddNote(note);
            return true;
        }

        Debug.LogError($"[Item] '{itemData.itemName}' tiene tipo Notes pero no es NoteData.");
        return false;
    }

    bool TryAddToInventory()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("[Item] InventoryManager no encontrado en la escena.");
            return false;
        }

        if (InventoryManager.Instance.IsFull())
        {
            return false;
        }

        return InventoryManager.Instance.AddItem(itemData);
    }

    void ShowFeedback(string message)
    {
        if (pickupFeedbackText == null) return;

        pickupFeedbackText.text = message;
        pickupFeedbackText.gameObject.SetActive(true);
        CancelInvoke(nameof(HideFeedback));
        Invoke(nameof(HideFeedback), feedbackDuration);
    }

    void HideFeedback()
    {
        if (pickupFeedbackText != null)
            pickupFeedbackText.gameObject.SetActive(false);
    }
}