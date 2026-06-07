using System.Collections.Generic;
using UnityEngine;

public class IndorProcedural : MonoBehaviour
{
    [Header("Rooms")]
    [SerializeField] private List<GameObject> roomPrefabs;

    [Header("Settings")]
    [Range(0f, 1f)][SerializeField] private float spawnChance = 1f;

    public void FillInternalSpace(GameObject currentBlock)
    {
        List<Transform> freeSpawns = FindSpawns(currentBlock);
        if (freeSpawns.Count == 0) return;

        Randomize(freeSpawns);
        int lastIndex = -1;

        foreach (Transform point in freeSpawns)
        {
            if (Random.value > spawnChance) continue;

            int prefabIndex = GetNextIndex(roomPrefabs.Count, lastIndex);
            GameObject chosenPrefab = roomPrefabs[prefabIndex];
            lastIndex = prefabIndex;

            Instantiate(chosenPrefab, point.position, point.rotation, currentBlock.transform);
        }
    }

    private List<Transform> FindSpawns(GameObject block)
    {
        List<Transform> foundSpawns = new List<Transform>();

        foreach (Transform child in block.GetComponentsInChildren<Transform>())
        {
            if (child == block.transform) continue;

            if (child.name.StartsWith("Spawn"))
            {
                foundSpawns.Add(child);
            }
        }
        return foundSpawns;
    }

    private int GetNextIndex(int total, int lastIndex)
    {
        if (total <= 1) return 0;
        int newIndex = lastIndex;

        int counter = 0;
        while (newIndex == lastIndex && counter < 10)
        {
            newIndex = Random.Range(0, total);
            counter++;
        }
        return newIndex;
    }

    private void Randomize (List<Transform> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Transform temp = list[i];
            int random = Random.Range(i, list.Count);
            list[i] = list[random];
            list[random] = temp;
        }
    }
}