using UnityEngine;
using System.Collections.Generic;

public class InteractableSpawner : MonoBehaviour
{
    [Header("Item Pools")]
    [SerializeField] GameObject[] keyPrefabs;
    [SerializeField] GameObject[] ballsPrefabs;
    [SerializeField] GameObject[] reflectorPrefabs;

    [Header("Spawn Procedural Settings")]
    [SerializeField] int minItemsPerPool = 1;
    [SerializeField] int maxItemsPerPool = 3;
    [SerializeField] float spawnRadius = 15f;
    [SerializeField] float minDistanceBetween = 3f;
    
    [Header("Ground Detection")]
    [SerializeField] float raycastHeight = 5f;
    [SerializeField] LayerMask groundLayer;

    List<Vector3> spawnedPositions = new List<Vector3>();

    void Start()
    {
        SpawnPool(keyPrefabs, "Llave");
        SpawnPool(ballsPrefabs, "Balls");
        SpawnPool(reflectorPrefabs, "Reflectores");

    }

    void SpawnPool(GameObject[] pool, string poolName)
    {
        if (pool == null || pool.Length == 0)
        {
            return;
        }

        int amount = Random.Range(minItemsPerPool, maxItemsPerPool + 1);

        for (int i = 0; i < amount; i++)
        {
            GameObject prefab = pool[Random.Range(0, pool.Length)];
            if (prefab == null) continue;

            if (!TryGetValidPosition(out Vector3 position)) continue;

            GameObject obj = Instantiate(prefab, position, Quaternion.identity);
            obj.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            obj.name = $"{poolName}_{i + 1}_spawned";
            spawnedPositions.Add(position);
        }
    }

    bool TryGetValidPosition(out Vector3 result)
    {
        int maxAttempts = 30;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 origin = transform.position + new Vector3(randomCircle.x, raycastHeight, randomCircle.y);

            if (!Physics.Raycast(origin, Vector3.down, out RaycastHit hit, raycastHeight * 2f, groundLayer))
                continue;

            if (!IsTooClose(hit.point))
            {
                result = hit.point;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }

    bool IsTooClose(Vector3 candidate)
    {
        foreach (Vector3 pos in spawnedPositions)
            if (Vector3.Distance(candidate, pos) < minDistanceBetween)
                return true;
        return false;
    }
}