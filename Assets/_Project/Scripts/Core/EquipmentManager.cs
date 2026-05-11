using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance { get; private set; }
    [SerializeField] Transform handPoint;
    GameObject currentEquipedModel;
    ItemData currentData;
    public ItemData CurrentEquippedItem => currentData;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void EquipItem(ItemData item)
    {
        if (item.itemModelPrefab == null) return;
        Unequip();
        currentData = item;
        currentEquipedModel = Instantiate(item.itemModelPrefab, handPoint);
        currentEquipedModel.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
    public void Unequip()
    {
        if (currentEquipedModel != null)
        {
            Destroy(currentEquipedModel);
            currentEquipedModel = null;
            currentData = null;
        }
    }
}
