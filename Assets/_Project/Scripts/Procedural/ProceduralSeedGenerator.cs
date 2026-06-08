using UnityEngine;

public class ProceduralSeedGenerator : MonoBehaviour
{
    public static ProceduralSeedGenerator Instance { get; private set; }

    [Header("Global Seed Configuration")]
    [Tooltip("Formato estricto de 11 dígitos: 3(Puertas) + 3(Fusibles) + 3(Cajas) + 2(Libro). Si lo dejas vacío, se generará uno aleatorio.")]
    public string masterSeed = "";
    public int DoorSeed { get; private set; }
    public int FuseSeed { get; private set; }
    public int SafeSeed { get; private set; }
    public int BookSeed { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializeMasterSeed();
    }

    private void InitializeMasterSeed()
    {
        if (string.IsNullOrEmpty(masterSeed) || masterSeed.Length != 11)
        {
            GenerateRandomMasterSeed();
        }

        try
        {
            DoorSeed = int.Parse(masterSeed.Substring(0, 3));
            FuseSeed = int.Parse(masterSeed.Substring(3, 3));
            SafeSeed = int.Parse(masterSeed.Substring(6, 3));
            BookSeed = int.Parse(masterSeed.Substring(9, 2));
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[ProceduralManager] Error al fragmentar la semilla. Generando semilla de emergencia. Error: {e.Message}");
            GenerateRandomMasterSeed();
            InitializeMasterSeed();
            return;
        }

        Debug.Log($"[ProceduralManager] Semilla Maestra Iniciada: {masterSeed}\nPuertas: {DoorSeed} | Fusibles: {FuseSeed} | Cajas: {SafeSeed} | Libro: {BookSeed}");
    }
    private void GenerateRandomMasterSeed()
    {
        masterSeed = "";
        for (int i = 0; i < 11; i++)
        {
            masterSeed += Random.Range(0, 10).ToString();
        }
        Debug.LogWarning("[ProceduralManager] Semilla aleatoria generada automáticamente.");
    }
    public int GetCustomLocalSeed(string uniqueID)
    {
        return masterSeed.GetHashCode() + uniqueID.GetHashCode();
    }
}
