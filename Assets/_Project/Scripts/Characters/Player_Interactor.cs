using UnityEngine;

public class Player_Interaction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] float interactRange = 3f;
    [SerializeField] LayerMask interactLayer;
    [SerializeField] Transform interactPoint;
    void Update()
    {
        InteractDetector();
    }

    void InteractDetector()
    {
        Vector3 rayOrigin = interactPoint.position;
        Vector3 direction = interactPoint.forward;
        if (Physics.Raycast(rayOrigin, direction, out RaycastHit hit, interactRange, interactLayer))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
               Debug.Log("Interacted with " + hit.collider.name);
            }
        }
    }
}
