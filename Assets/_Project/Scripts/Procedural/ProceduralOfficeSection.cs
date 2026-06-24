using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProceduralOfficeSection : MonoBehaviour
{
    [Serializable]
    public struct OfficePuzzleVariant
    {
        public string variantName;
        public GameObject officeContainer;

        [Header("Props Base")]
        public GameObject[] propsToEnable;
        public GameObject[] propsToDisable;

        [Header("Random Extra Spawns")]
        [Tooltip("Arreglo de objetos (documentos, tazas, etc). El sistema elegirá UN objeto al azar de esta lista para activarlo, ignorando el resto.")]
        public GameObject[] randomBonusObjects;
    }

    [Header("Available Office Configurations")]
    [SerializeField] private OfficePuzzleVariant[] officeVariants;

    void Start()
    {
        ExecuteOfficeSpawning();
    }

    private void ExecuteOfficeSpawning()
    {
        if (officeVariants == null || officeVariants.Length == 0) return;

        Random.InitState(ProceduralSeedGenerator.Instance.OfficeSeed + gameObject.name.GetHashCode());

        int variantIndex = Random.Range(0, officeVariants.Length);
        OfficePuzzleVariant activeVariant = officeVariants[variantIndex];

        foreach (OfficePuzzleVariant variant in officeVariants)
        {
            if (variant.officeContainer != null) variant.officeContainer.SetActive(false);
        }

        if (activeVariant.officeContainer != null) activeVariant.officeContainer.SetActive(true);

        foreach (GameObject prop in activeVariant.propsToEnable)
        {
            if (prop != null) prop.SetActive(true);
        }
        foreach (GameObject prop in activeVariant.propsToDisable)
        {
            if (prop != null) prop.SetActive(false);
        }

        if (activeVariant.randomBonusObjects != null && activeVariant.randomBonusObjects.Length > 0)
        {
            foreach (GameObject bonus in activeVariant.randomBonusObjects)
            {
                if (bonus != null) bonus.SetActive(false);
            }

            int randomBonusIndex = Random.Range(0, activeVariant.randomBonusObjects.Length);
            GameObject selectedBonus = activeVariant.randomBonusObjects[randomBonusIndex];

            if (selectedBonus != null) selectedBonus.SetActive(true);

            Debug.Log($"[Procedural] Oficina | Variante: {activeVariant.variantName} | Extra Spawneado: {selectedBonus.name}");
        }
        else
        {
            Debug.Log($"[Procedural] Oficina | Variante: {activeVariant.variantName} | Sin objetos extra.");
        }
    }
}
