using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData itemData;
    public void PickUp()
    {
        gameObject.SetActive(false);
    }
}
