using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{


    [SerializeField] private Animator anim;
    [SerializeField] private CreditsManager crman;
    [SerializeField] private GameObject[] goBackList;
    [SerializeField] private GameObject membersList;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private bool isOnMenu;
    [SerializeField] private bool isOnPlay;
    [SerializeField] private bool isOnOptions;
    [SerializeField] private bool isOnCredits;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        isOnMenu = false;
        anim = GetComponentInParent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameTittleAnim()
    {
        anim.SetTrigger("GoToMenu");
        isOnMenu = true;
        mainMenu.SetActive(true);
        
    }

    public void PressPlay()
    {

        if (isOnMenu)
        {
            anim.SetTrigger("ZoomToPlay");
            isOnMenu= false;
            isOnPlay = true;
            activeGoBack(0);
        }
        else SceneManager.LoadScene("CinematicBookIntro"); 
    }

    public void PressOptions()
    {
        if (isOnMenu)
        {
            anim.SetTrigger("ZoomToOptions");
            isOnMenu = false;
            isOnOptions = true;
            activeGoBack(1);

        }


    }

    public void PressCredits()
    {
        if (isOnMenu)
        {
            anim.SetTrigger("ZoomToCredits");
            isOnMenu = false;
            isOnCredits = true;
            membersList.SetActive(true);
            activeGoBack(2);
        }
    }

    public void ReturnToMenu()
    {
        if (isOnPlay)
        {
            anim.SetTrigger("PlayToMenu");
            isOnPlay= false;
        }
        else if (isOnOptions)
        {
            anim.SetTrigger("OptionsToMenu");
            isOnOptions= false;
            
        }
        else if (isOnCredits)
        {
            anim.SetTrigger("CreditsToMenu");
            isOnCredits= false;
            membersList.SetActive(false);
            crman.DeactivateAllPanels();
        }
        else return;

        isOnMenu = true;
        DeactivateGoBacks();
        
    }

    private void activeGoBack( int option)
    {

        for (int i = 0; i < goBackList.Length; i++)
        {
            if(i == option)
            {
                goBackList[i].SetActive(true);
            }
        }

    }

    private void DeactivateGoBacks()
    {
        foreach (GameObject gobackbutton in goBackList)
        {
            if (gobackbutton != null)
            {

                gobackbutton.SetActive(false);
            }
        }
    }
}
