using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
public class TutorialDialogueManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CinemachineTargetGroup targetGroup;
    [SerializeField] GameObject[] stepObjects;

    [Header("Settings")]
    [SerializeField] float focusWeight = 1.5f;
    [SerializeField] float transitionSpeed = 2.5f;
    [SerializeField] float displayDuration = 1.2f;
    bool isBusy = false;

    public void TriggerStep(int index)
    {
        if (isBusy || index < 0 || index >= stepObjects.Length) return;
        if (stepObjects[index] == null) return;

        StartCoroutine(ExecuteStep(index));
    }
    IEnumerator ExecuteStep(int index)
    {
        isBusy = true;
        GameObject stepObj = stepObjects[index];
        Transform target = stepObj.transform;

        stepObj.SetActive(true);
        targetGroup.AddMember(target, 0, 1);

        int memberIndex = targetGroup.FindMember(target);
        float weight = 0;
        while (weight < focusWeight)
        {
            weight += Time.deltaTime * transitionSpeed;
            targetGroup.Targets[memberIndex].Weight = weight;
            yield return null;
        }

        yield return new WaitForSeconds(displayDuration);

        while (weight > 0f)
        {
            weight -= Time.deltaTime * transitionSpeed;
            targetGroup.Targets[memberIndex].Weight = weight;
            yield return null;
        }

        targetGroup.RemoveMember(target);
        stepObj.SetActive(false);
        isBusy = false;
    }
}
