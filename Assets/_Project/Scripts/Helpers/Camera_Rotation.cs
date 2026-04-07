using UnityEngine;

public class Camera_Rotation : MonoBehaviour
{
    private Transform cameraPosition;
    private void Awake()
    {
        cameraPosition = GameObject.Find("PlayerCam").transform;
    }
    void Update()
    {
        transform.position = cameraPosition.position;
    }
}
