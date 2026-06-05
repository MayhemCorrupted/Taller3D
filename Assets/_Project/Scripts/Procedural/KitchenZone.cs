using UnityEngine;

public class KitchenZone : MonoBehaviour
{
    //script temporal para generar objetos hasta que se haga el sistema robusto de esto para los demás sitios xd
    [SerializeField] GameObject keyItemPrefab, 
    noteItemPrefab, keyDoorObject, panelDoorObject;
    [SerializeField] ItemData requiredData;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] int itemSeed;
    [SerializeField] PanelCodePuzzle panelToUnlock;
    int spawnIndex = 0;

    void Start()
    {
        InitializeSeededGeneration();
    }

    private void InitializeSeededGeneration()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[KitchenZone] No hay puntos de spawn asignados en el Inspector.");
            return;
        }

        if (itemSeed == 0)
        {
            itemSeed = Random.Range(1, 100000);
        }

        Random.InitState(itemSeed);

        ExecuteSpawning();
    }

    private void ExecuteSpawning()
    {
        int itemRoll = Random.Range(1, 101);
        bool isOdd = (itemRoll % 2 != 0);

        GameObject prefabToSpawn = isOdd ? keyItemPrefab : noteItemPrefab;

        spawnIndex = Random.Range(0, spawnPoints.Length);
        Transform targetPoint = spawnPoints[spawnIndex];

        if (prefabToSpawn != null && targetPoint != null)
        {
            Instantiate(prefabToSpawn, targetPoint.position, targetPoint.rotation, targetPoint);

            if (isOdd)
            {
                if (panelToUnlock != null) panelToUnlock.CanUsePanel = false;
                else Debug.LogWarning("[KitchenZone] Se generó la llave pero 'panelToUnlock' no está asignado en el Inspector.");
                if (keyDoorObject != null) keyDoorObject.SetActive(true);
                if (panelDoorObject != null) panelDoorObject.SetActive(false);

                Debug.Log("[KitchenZone] Estado: Ruta Llave Activa. Panel inhabilitado.");
            }
            else
            {
                if (panelToUnlock != null) panelToUnlock.CanUsePanel = true;

                if (keyDoorObject != null) keyDoorObject.SetActive(false);
                if (panelDoorObject != null) panelDoorObject.SetActive(true);

                Debug.Log("[KitchenZone] Estado: Ruta Panel Activa. Panel habilitado.");
            }

            Debug.Log($"[Procedural] Semilla Activa: {itemSeed} | Número: {itemRoll} ({(isOdd ? "Impar" : "Par")}) | Posición Índice: {spawnIndex} | Padre: {targetPoint.name}");
        }
    }
}
