using UnityEngine;

public class ProceduralSeedGenerator : MonoBehaviour
{
    public static ProceduralSeedGenerator Instance { get; private set; }

    [Header("Global Seed Configuration")]
    [Tooltip("Admite cualquier cadena de texto como semilla maestra. Si lo dejas vacío, se generará una semilla aleatoria.")]
    public string masterSeed = "";
    public int DoorSeed { get; private set; }
    public int FuseSeed { get; private set; }
    public int OfficeSeed { get; private set; }
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

        if (masterSeed.Length == 4 && int.TryParse(masterSeed, out _))
        {
            DoorSeed = int.Parse(masterSeed[0].ToString());
            FuseSeed = int.Parse(masterSeed[1].ToString());
            OfficeSeed = int.Parse(masterSeed[2].ToString());
            BookSeed = int.Parse(masterSeed[3].ToString());

            Debug.Log($"[Procedural] Semilla Numérica Estándar: {masterSeed}");
        }
        else
        {
            System.Random textHashRandom = new(masterSeed.GetHashCode());

            DoorSeed = textHashRandom.Next(0, 10);
            FuseSeed = textHashRandom.Next(0, 10);
            OfficeSeed = textHashRandom.Next(0, 10);
            BookSeed = textHashRandom.Next(0, 10);

            Debug.Log($"[Procedural] Semilla de Texto Detectada: '{masterSeed}' -> Convertida internamente.");
        }

        Debug.Log($"[Procedural] Distribución = Puertas: {DoorSeed} | Fusibles: {FuseSeed} | Oficina: {OfficeSeed} | Libro: {BookSeed}");
    }
    private void GenerateRandomMasterSeed()
    {
        masterSeed = "";
        for (int i = 0; i < 4; i++) 
        {
            masterSeed += Random.Range(0, 10).ToString();
        }
        Debug.LogWarning("[Procedural] Semilla vacía. Generando semilla aleatoria automáticamente.");
    }
    public int GetCustomLocalSeed(string uniqueID)
    {
        return masterSeed.GetHashCode() + uniqueID.GetHashCode();
    }
}
