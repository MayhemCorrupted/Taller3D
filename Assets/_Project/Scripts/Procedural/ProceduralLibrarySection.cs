using System;
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

        if (!string.IsNullOrEmpty(activeVariant.spawnTag))
        {
            GameObject[] availablePoints = GameObject.FindGameObjectsWithTag(activeVariant.spawnTag);

            if (availablePoints.Length > 0)
            {
                int spawnIndex = Random.Range(0, availablePoints.Length);
                Transform targetPoint = availablePoints[spawnIndex].transform;

                GameObject spawnedItem = Instantiate(activeVariant.itemPrefab, targetPoint.position, targetPoint.rotation);
                spawnedItem.transform.SetParent(targetPoint, true);

                Debug.Log($"[Procedural] Librería | Variante: {activeVariant.variantName} | Spawn en: {targetPoint.name} (Tag: {activeVariant.spawnTag})");
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
}
