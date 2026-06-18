using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CaosDialogueManager : MonoBehaviour
{
    [System.Serializable] private struct DialoguesChaos

    {
        public string text;
        public TMP_FontAsset font;
        public Color color;
        public float minSize;
        public float maxSize;
        public FontStyles styleFont;
        public float lifeTime;
        public float waitBeforeNext;
    }
    [System.Serializable] private struct SpawnPoint
    {
        public RectTransform position;
        public bool used;
    }

    [SerializeField] GameObject textPrefab;
    [SerializeField] Transform canvasContainer;
    [SerializeField] int maxTexts = 10;

    [SerializeField] List<DialoguesChaos> dialogueList = new List<DialoguesChaos>();

    [SerializeField] float fadeInTime = 0.2f;
    [SerializeField] float fadeOutTime = 0.4f;

    [SerializeField] List<SpawnPoint> myPoints = new List<SpawnPoint>();

    private List<TMP_Text> textClones = new List<TMP_Text>();
    private bool isRunning = false;

    void Awake()
    {
        for (int i = 0; i < maxTexts; i++)
        {
            GameObject clone = Instantiate(textPrefab, canvasContainer);
            clone.SetActive(false);
            textClones.Add(clone.GetComponent<TMP_Text>());
        }
    }

    public void StartChaosDialogue()
    {
        if (isRunning == false)
        {
            StartCoroutine(WordsRoutine());
        }
    }

    IEnumerator WordsRoutine()
    {
        isRunning = true;

        foreach (DialoguesChaos currentDialogue in dialogueList)
        {
            int pointIndex = FindFreePoint();
            TMP_Text freeText = FindOffText();

            if (pointIndex != -1 && freeText != null)
            {
                TogglePoint(pointIndex, true);

                freeText.text = currentDialogue.text;
                freeText.font = currentDialogue.font;
                freeText.fontStyle = currentDialogue.styleFont;
                freeText.fontSize = Random.Range(currentDialogue.minSize, currentDialogue.maxSize);

                RectTransform rect = freeText.GetComponent<RectTransform>();
                rect.localPosition = myPoints[pointIndex].position.localPosition;
                rect.localRotation = Quaternion.Euler(0, 0, Random.Range(-5f, 5f));

                Color colorWithAlpha = currentDialogue.color;
                colorWithAlpha.a = 0f;
                freeText.color = colorWithAlpha;

                freeText.gameObject.SetActive(true);

                StartCoroutine(TextLife(pointIndex, freeText, colorWithAlpha, currentDialogue.lifeTime));
            }

            yield return new WaitForSeconds(currentDialogue.waitBeforeNext);
        }

        isRunning = false;
    }

    IEnumerator TextLife(int pointIndex, TMP_Text textComponent, Color originalColor, float visibleTime)
    {
        float timer = 0f;

        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            originalColor.a = Mathf.Lerp(0f, 1f, timer / fadeInTime);
            textComponent.color = originalColor;
            yield return null;
        }
        originalColor.a = 1f;
        textComponent.color = originalColor;

        yield return new WaitForSeconds(visibleTime);

        timer = 0f;
        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            originalColor.a = Mathf.Lerp(1f, 0f, timer / fadeOutTime);
            textComponent.color = originalColor;
            yield return null;
        }

        textComponent.gameObject.SetActive(false);
        TogglePoint(pointIndex, false);
    }

    int FindFreePoint()
    {
        List<int> freeOptions = new List<int>();
        for (int i = 0; i < myPoints.Count; i++)
        {
            if (myPoints[i].used == false)
            {
                freeOptions.Add(i);
            }
        }

        if (freeOptions.Count == 0) return -1;
        return freeOptions[Random.Range(0, freeOptions.Count)];
    }

    TMP_Text FindOffText()
    {
        foreach (TMP_Text textElement in textClones)
        {
            if (textElement.gameObject.activeSelf == false)
            {
                return textElement;
            }
        }
        return null;
    }

    void TogglePoint(int index, bool isUsed)
    {
        SpawnPoint point = myPoints[index];
        point.used = isUsed;
        myPoints[index] = point;
    }
}