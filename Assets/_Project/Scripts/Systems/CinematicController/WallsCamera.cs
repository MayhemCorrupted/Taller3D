using Unity.Cinemachine;
using UnityEngine;

public class WallsCamera : MonoBehaviour
{
    [System.Serializable]
    public class CameraView
    {
        public GameObject interactObject;
        public CinemachineCamera targetCamera;
    }

    [Header("Player Camera")]
    [SerializeField] CinemachineCamera playerCamera;

    [Header("Views")]
    [SerializeField] CameraView[] virtualCameras;

    [Header("Prompt")]
    public string interactPrompt = "[E] Ver";

    CinemachineCamera activeCamera;

    void Awake ()
    {
        playerCamera.Priority = 10;

        foreach (CameraView view in virtualCameras)
        {
            view.targetCamera.Priority = 0;
        }
    }

    public void Interact(GameObject interactObj)
    {
        foreach (CameraView view in virtualCameras)
        {
            if (view.interactObject == interactObj)
            {
                if (activeCamera != null)
                {
                    activeCamera.Priority = 0;
                }

                playerCamera.Priority = 0;

                view.targetCamera.Priority = 100;

                activeCamera = view.targetCamera;

                break;
            }
        }
    }

    void Update()
    {
        if (activeCamera == null) return;

        if (Input.GetKeyDown(KeyCode.E) ||
            Input.GetKeyDown(KeyCode.Escape))
        {
            activeCamera.Priority = 0;

            playerCamera.Priority = 10;

            activeCamera = null;
        }
    }
}