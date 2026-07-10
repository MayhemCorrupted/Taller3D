using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TutorialPanel : MonoBehaviour
{
    [Header("Posiciones (pivot en 0, 0.5)")]
    [SerializeField] Vector2 hiddenPosition = new(-750, 0);
    [SerializeField] Vector2 visiblePosition = new(0, 0);

    [Header("Animación")]
    [SerializeField] float slideDuration = 0.5f;
    [SerializeField] AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Auto-ocultar")]
    [SerializeField] bool autoHide = true;
    [SerializeField] float visibleDuration = 3f;  // ← Segundos visible antes de ocultarse

    RectTransform rectTransform;
    bool isVisible = false;
    float timer;
    Coroutine autoHideCoroutine;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.pivot = new Vector2(0, 0.5f);
        rectTransform.anchoredPosition = hiddenPosition;
    }

    

    void Start()
    {
        Show();
    }

    public void Show()
    {
       
        if (autoHideCoroutine != null)
            StopCoroutine(autoHideCoroutine);

        isVisible = true;
        timer = 0f;
        enabled = true;

        
        if (autoHide)
            autoHideCoroutine = StartCoroutine(AutoHideRoutine());
    }

    public void Hide()
    {
        isVisible = false;
        timer = 0f;
        enabled = true;
    }



    IEnumerator AutoHideRoutine()
    {
        // Esperar la animación de entrada + tiempo visible
        yield return new WaitForSeconds(slideDuration + visibleDuration);

        Hide();
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / slideDuration);
        float easedT = easeCurve.Evaluate(t);

        Vector2 from = isVisible ? hiddenPosition : visiblePosition;
        Vector2 to = isVisible ? visiblePosition : hiddenPosition;

        rectTransform.anchoredPosition = Vector2.Lerp(from, to, easedT);

        if (t >= 1f) enabled = false;
    }
}
