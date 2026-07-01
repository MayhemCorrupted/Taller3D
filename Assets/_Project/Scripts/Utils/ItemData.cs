using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Items/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemType { Interactable, Notes }
    public int id;
    public string itemName;
    [Space(10)]
    [Header("Item Section")]
    [TextArea] public string description;
    public Sprite sprite;
    public ItemType itemType;
    public GameObject itemModelPrefab;
}
