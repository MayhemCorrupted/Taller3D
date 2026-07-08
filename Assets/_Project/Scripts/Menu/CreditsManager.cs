using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour
{

    [SerializeField] private GameObject[] memberPanels;
    [SerializeField] private List<Button> memberButtons;
    [SerializeField] private int member;

    void Start()
    {

        foreach (Button button in memberButtons)
        {
            if (button != null)
            {
                
                button.onClick.AddListener(() => DetectIndex(button));
            }
        }
    }


    public void DetectIndex(Button pressedButton)
    {
        
        member = memberButtons.IndexOf(pressedButton);

        for (int i = 0; i < memberPanels.Length; i++)
        {
            if (i == member)
            {
                memberPanels[i].SetActive(true);
            }
        }
    }

    public void DeactivateCurrentPanel()
    {
        DeactivateAllPanels();
    }

    public void DeactivateAllPanels()
    {
        foreach (GameObject panel in memberPanels)
        {
            if (panel != null)
            {

                panel.SetActive(false);
            }
        }
    }

}
