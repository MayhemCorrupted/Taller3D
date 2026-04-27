using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public int id;
    public string itemName;
    [TextArea] public string description;
    public Sprite sprite;
    public ItemType itemType;
    public enum ItemType { Interactable, Notes }
}
