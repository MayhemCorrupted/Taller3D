using Unity.VisualScripting;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    [SerializeField] private GameObject panel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("ShowPanel", 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ShowPanel()
    {
        panel.SetActive(true);
    }
}
