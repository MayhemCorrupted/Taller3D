using UnityEngine;

public class ProceduralHouse : MonoBehaviour
{
    [Header("Blocks")]
    [SerializeField] private GameObject center;
    [SerializeField] private GameObject left;
    [SerializeField] private GameObject right;

    private IndorProcedural spawner;

    private void Start()
    {
        spawner = GetComponent<IndorProcedural>();
        GenerateBase();
    }

    private void GenerateBase()
    {
        GameObject centerBlock = Instantiate(center, transform.position, transform.rotation);
        Transform exitLeft = centerBlock.transform.Find("ExitLeft");
        Transform exitRight = centerBlock.transform.Find("ExitRight");
        GameObject leftBlock = Instantiate(left, Vector3.zero, Quaternion.identity);
        Transform entranceLeft = leftBlock.transform.Find("Entrance");
        AlignBlock(leftBlock.transform, entranceLeft, exitLeft);
        spawner.FillInternalSpace(leftBlock);
        GameObject rightBlock = Instantiate(right, Vector3.zero, Quaternion.identity);
        Transform entranceRight = rightBlock.transform.Find("Entrance");
        AlignBlock(rightBlock.transform, entranceRight, exitRight);
        spawner.FillInternalSpace(rightBlock); spawner.FillInternalSpace(centerBlock);
    }

    private void AlignBlock(Transform block, Transform entrance, Transform previousExit)
    {
        float angle = Vector3.SignedAngle(entrance.forward, - previousExit.forward, Vector3.up);
        block.Rotate(Vector3.up, angle);
        Vector3 offset = block.position - entrance.position;
        block.position = previousExit.position + offset;
    }
}