using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProceduralKitchenSection : MonoBehaviour
{
    [Serializable]
    public struct SpawnLocation
    {
        public string locationName;
        public string locationTag;
    }

    [Serializable]
    public struct PuzzleVariant
    {
        public string variantName;
        public GameObject itemPrefab;

        [Tooltip("Agrega todos los tags donde este objeto tiene permitido aparecer")]
        public SpawnLocation[] allowedLocations;

        [Header("Door Control")]
        [Tooltip("La puerta que se activará para esta ruta")]
        public GameObject doorToEnable;

        [Tooltip("Todas las demás puertas que deben apagarse")]
        public GameObject[] doorsToDisable;

        [Header("Panel Control")]
        public PanelCodePuzzle linkedPanel;
        public bool isPanelActive;
    }

    [Header("Procedural Configuration")]
    [SerializeField] private PuzzleVariant[] puzzleVariants;

    void Start()
    {
        ExecuteProceduralSpawning();
    }

    private void ExecuteProceduralSpawning()
    {
        if (puzzleVariants == null || puzzleVariants.Length == 0) return;

        Random.InitState(ProceduralSeedGenerator.Instance.DoorSeed + gameObject.name.GetHashCode());

        int variantIndex = Random.Range(0, puzzleVariants.Length);
        PuzzleVariant activeVariant = puzzleVariants[variantIndex];

        int locationIndex = Random.Range(0, activeVariant.allowedLocations.Length);
        string selectedTag = activeVariant.allowedLocations[locationIndex].locationTag;

        GameObject[] availablePoints = GameObject.FindGameObjectsWithTag(selectedTag);

        if (availablePoints.Length > 0)
        {
            Transform targetPoint = availablePoints[Random.Range(0, availablePoints.Length)].transform;

            GameObject spawnedItem = Instantiate(activeVariant.itemPrefab, targetPoint.position, targetPoint.rotation);
            spawnedItem.transform.SetParent(targetPoint, true);

            Debug.Log($"[Procedural] Ruta: {activeVariant.variantName} | Objeto en: {selectedTag}");
        }
        else Debug.LogWarning($"[Procedural] No se encontraron objetos con el tag: {selectedTag}");

        if (activeVariant.doorToEnable != null) activeVariant.doorToEnable.SetActive(true);

        foreach (GameObject door in activeVariant.doorsToDisable)
        {
            if (door != null) door.SetActive(false);
        }

        if (activeVariant.linkedPanel != null) activeVariant.linkedPanel.CanUsePanel = activeVariant.isPanelActive;
    }
}
