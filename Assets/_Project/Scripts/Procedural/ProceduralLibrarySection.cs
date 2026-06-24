using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class ProceduralLibrarySection : MonoBehaviour
{
    [Serializable]
    public struct LibraryPuzzleVariant
    {
        public string variantName;          
        public GameObject itemPrefab;      

        [Header("Spawn Configuration")]
        [Tooltip("El Tag exacto en Unity de los puntos donde este objeto tiene permitido aparecer")]
        public string spawnTag;
        [Header("Secondary Items Configuration")]
        [Tooltip("Prefabs adicionales (falsos/decorativos) que llenarán los puntos restantes")]
        public GameObject[] secondaryPrefabs;

        [Header("Environment Props (Optional)")]
        [Tooltip("Libros falsos o pistas visuales que aparecen SOLO en esta ruta")]
        public GameObject[] propsToEnable;

        [Tooltip("Cosas que estorban o deben desaparecer en esta ruta")]
        public GameObject[] propsToDisable;
    }

    [Header("Library Configurations")]
    [SerializeField] private LibraryPuzzleVariant[] libraryVariants;
    
    void Start()
    {
        ExecuteLibrarySpawning();
    }

    private void ExecuteLibrarySpawning()
    {
        if (libraryVariants == null || libraryVariants.Length == 0) return;

        Random.InitState(ProceduralSeedGenerator.Instance.BookSeed + gameObject.name.GetHashCode());

        int variantIndex = Random.Range(0, libraryVariants.Length);
        LibraryPuzzleVariant activeVariant = libraryVariants[variantIndex];

        Debug.Log($"[Procedural] Librería (Semilla: {ProceduralSeedGenerator.Instance.BookSeed}) | Variante: {activeVariant.variantName}");

        if (!string.IsNullOrEmpty(activeVariant.spawnTag))
        {
            List<GameObject> availablePoints = new(GameObject.FindGameObjectsWithTag(activeVariant.spawnTag));

            if (availablePoints.Count > 0)
            {
                int mainSpawnIndex = Random.Range(0, availablePoints.Count);
                Transform mainTargetPoint = availablePoints[mainSpawnIndex].transform;

                SpawnPrefabAtTransform(activeVariant.itemPrefab, mainTargetPoint);
                Debug.Log($"[Procedural] Librería | Objeto Principal en: {mainTargetPoint.name}");

                availablePoints.RemoveAt(mainSpawnIndex);

                if (activeVariant.secondaryPrefabs != null && activeVariant.secondaryPrefabs.Length > 0)
                {
                    for (int i = 0; i < availablePoints.Count; i++)
                    {
                        Transform secTargetPoint = availablePoints[i].transform;

                        int randomPrefabIndex = Random.Range(0, activeVariant.secondaryPrefabs.Length);
                        GameObject selectedPrefab = activeVariant.secondaryPrefabs[randomPrefabIndex];

                        SpawnPrefabAtTransform(selectedPrefab, secTargetPoint);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"[Procedural] No se encontraron puntos de spawn con el tag: {activeVariant.spawnTag}. Asegúrate de que los GameObjects en la escena tengan el Tag correcto.");
            }
        }
        else
        {
            Debug.LogError($"[Procedural] La variante {activeVariant.variantName} no tiene un Tag asignado en el Inspector.");
        }
        foreach (GameObject prop in activeVariant.propsToEnable)
        {
            if (prop != null) prop.SetActive(true);
        }

        foreach (GameObject prop in activeVariant.propsToDisable)
        {
            if (prop != null) prop.SetActive(false);
        }
    }
    void SpawnPrefabAtTransform(GameObject prefab, Transform target)
    {
        if (prefab == null || target == null) return;

        GameObject spawnedItem = Instantiate(prefab, target.position, target.rotation);
        spawnedItem.transform.SetParent(target, true);
    }
}
