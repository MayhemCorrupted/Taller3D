using UnityEngine;

public class KitchenZone : BaseZone
{
    [Header("Prefabs")]
    [SerializeField] GameObject keyItemPrefab;
    [SerializeField] GameObject noteItemPrefab;

    [Header("Door Objects")]
    [SerializeField] GameObject keyDoorObject;
    [SerializeField] GameObject panelDoorObject;

    [Header("Puzzle Reference")]
    [SerializeField] PanelCodePuzzle panelToUnlock;

    protected override void ExecuteSpawning()
    {
        int itemRoll = Random.Range(1, 101);
        bool spawnKey = (itemRoll % 2 != 0);

        GameObject prefabToSpawn = spawnKey ? keyItemPrefab : noteItemPrefab;
        spawnIndex = Random.Range(0, spawnPoints.Length);
        Transform targetPoint = spawnPoints[spawnIndex];

        if (prefabToSpawn == null || targetPoint == null) return;

        Instantiate(prefabToSpawn, targetPoint.position, targetPoint.rotation, targetPoint);

        if (spawnKey)
        {
            ActivateKeyRoute();
        }
        else
        {
            ActivatePanelRoute();
        }
       
    }

    void ActivateKeyRoute()
    {
        if (panelToUnlock != null) panelToUnlock.CanUsePanel = false;

        if (keyDoorObject != null) keyDoorObject.SetActive(true);
        if (panelDoorObject != null) panelDoorObject.SetActive(false);
    }

    void ActivatePanelRoute()
    {
        if (panelToUnlock != null) panelToUnlock.CanUsePanel = true;

        if (keyDoorObject != null) keyDoorObject.SetActive(false);
        if (panelDoorObject != null) panelDoorObject.SetActive(true);
    }
}