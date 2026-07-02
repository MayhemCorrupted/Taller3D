using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class EnvironmentFeedback : MonoBehaviour
{
    [Header("Objeto a sacudir")]
    [SerializeField] Transform shakeTarget;

    [Header("Shake Settings")]
    [SerializeField] float shakeDuration = 0.6f;
    [SerializeField] float shakeMagnitude = 0.04f;
    [SerializeField] int shakeLoops = 3;
    [SerializeField] float pauseBetweenLoops = 0.2f;
    [SerializeField] bool returnToOrigin = true;

    [Header("Eventos")]
    public UnityEvent OnShakeStart;
    public UnityEvent OnShakeEnd;

    Vector3 originLocalPos;
    bool isShaking = false;

    void Awake()
    {
        if (shakeTarget == null) shakeTarget = transform;
        originLocalPos = shakeTarget.localPosition;
    }

    public void Trigger()
    {
        if (isShaking) return;
        StartCoroutine(ShakeRoutine());
    }

    IEnumerator ShakeRoutine()
    {
        isShaking = true;
        OnShakeStart?.Invoke();

        for (int loop = 0; loop < shakeLoops; loop++)
        {
            float elapsed = 0f;

            while (elapsed < shakeDuration)
            {
                Vector3 offset = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-0.3f, 0.3f),
                    Random.Range(-0.5f, 0.5f)
                ) * shakeMagnitude;

                shakeTarget.localPosition = originLocalPos + offset;

                elapsed += Time.deltaTime;
                yield return null;
            }

            if (returnToOrigin) shakeTarget.localPosition = originLocalPos;
            if (loop < shakeLoops - 1)
                yield return new WaitForSeconds(pauseBetweenLoops);
        }

        if (returnToOrigin) shakeTarget.localPosition = originLocalPos;
        isShaking = false;
        OnShakeEnd?.Invoke();
    }
}