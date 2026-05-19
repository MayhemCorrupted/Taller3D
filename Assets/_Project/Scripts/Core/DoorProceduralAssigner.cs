using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class DoorProceduralAssigner : MonoBehaviour
{

    [SerializeField][Range(0f, 1f)] private float lockedChance = 0.3f;
    [SerializeField] private float minDistance = 3f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AssignProcedural();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AssignProcedural()
    {
        DoorController[] doors = FindObjectsByType<DoorController>(FindObjectsSortMode.None);
        List<Vector3> lockedPositions = new List<Vector3>();

        foreach(DoorController door in doors)
        {
            bool nearLocked = false;        

            foreach (Vector3 pos in lockedPositions)
            {
                if(Vector3.Distance(pos, door.transform.position) < minDistance)
                {
                    nearLocked = true;
                    break;
                }
            }

            bool isLocked = !nearLocked && Random.value < lockedChance;
            if(isLocked) lockedPositions.Add(door.transform.position);
            if (isLocked) door.Setup(DoorController.DoorType.locked);
            else door.Setup(DoorController.DoorType.interactive);

            Renderer rend = door.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                if (isLocked)
                {
                    rend.material.color = Color.red;
                }
                else
                {
                    rend.material.color = Color.white;
                }
            }
        }
    }
}
