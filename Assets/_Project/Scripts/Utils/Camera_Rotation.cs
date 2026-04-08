using UnityEngine;

public class Camera_Rotation : MonoBehaviour
{
    [SerializeField] Transform cameraPosition;
    void Update()
    {
        transform.position = cameraPosition.position;
    }
}
