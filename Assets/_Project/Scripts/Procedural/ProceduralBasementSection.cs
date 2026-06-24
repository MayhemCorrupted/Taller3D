using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class ProceduralBasementSection : MonoBehaviour
{
    [Serializable]
    public struct FusePuzzleVariant
    {
        public string variantName;        
        public GameObject puzzleContainer;
        public GameObject[] extraPropsToEnable;  
        public GameObject[] extraPropsToDisable;

        [Header("Procedural Sub-Variants")]
        [Tooltip("Si este puzzle tiene lógicas internas (Ej: empezar apagado), activa este script")]
        public SwitchPuzzle linkedSwitchLogic;
    }

    [Header("Available Fuse Puzzles")]
    [SerializeField] private FusePuzzleVariant[] puzzleVariants;

    void Start()
    {
        ExecuteFuseSpawning();
    }

    private void ExecuteFuseSpawning()
    {
        if (puzzleVariants == null || puzzleVariants.Length == 0) return;

        Random.InitState(ProceduralSeedGenerator.Instance.FuseSeed + gameObject.name.GetHashCode());

        int variantIndex = Random.Range(0, puzzleVariants.Length);
        FusePuzzleVariant activeVariant = puzzleVariants[variantIndex];

        Debug.Log($"[Procedural] Sótano/Fusibles (Semilla: {ProceduralSeedGenerator.Instance.FuseSeed}) | Variante: {activeVariant.variantName}");

        foreach (FusePuzzleVariant variant in puzzleVariants)
        {
            if (variant.puzzleContainer != null) variant.puzzleContainer.SetActive(false);
        }

        if (activeVariant.puzzleContainer != null) activeVariant.puzzleContainer.SetActive(true);

        foreach (GameObject prop in activeVariant.extraPropsToEnable)
        {
            if (prop != null) prop.SetActive(true);
        }
        foreach (GameObject prop in activeVariant.extraPropsToDisable)
        {
            if (prop != null) prop.SetActive(false);
        }

        if (activeVariant.linkedSwitchLogic != null)
        {
            int logicVariantSeed = Random.Range(0, 2);
            activeVariant.linkedSwitchLogic.InitializeProceduralState(logicVariantSeed);
        }
    }
}
