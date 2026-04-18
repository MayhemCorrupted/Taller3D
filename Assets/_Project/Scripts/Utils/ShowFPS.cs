using TMPro;
using UnityEngine;

public class ShowFPS : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    float time;
    void Start()
    {
        time = 0;
    }

    void Update()
    {
        FPS();
    }
    void FPS()
    {
        if (time >= 0.5f)
        {
            int fps = (int)(1 / Time.unscaledDeltaTime);
            text.text = fps.ToString();   
            time = 0;
        }
        time += Time.unscaledDeltaTime;
    }
}
