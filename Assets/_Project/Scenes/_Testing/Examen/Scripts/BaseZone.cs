using UnityEngine;

public abstract class BaseZone : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] protected Transform[] spawnPoints;
    [SerializeField] protected int itemSeed;

    protected int spawnIndex = 0;

    protected virtual void Start()
    {
        InitializeSeededGeneration();
    }

    protected void InitializeSeededGeneration()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return;
        }

        if (itemSeed == 0)
            itemSeed = Random.Range(1, 100000);

        Random.InitState(itemSeed);

        ExecuteSpawning();
    }

    protected abstract void ExecuteSpawning();
}