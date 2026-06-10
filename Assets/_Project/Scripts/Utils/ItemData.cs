using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Items/ItemData")]
public class ItemData : ScriptableObject
{
    public int id;
    public string itemName;
    [TextArea] public string description;
    public Sprite sprite;
    public ItemType itemType;
    public GameObject itemModelPrefab;
    public enum ItemType { Interactable, Notes }
}
