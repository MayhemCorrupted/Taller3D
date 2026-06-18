using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{


    [SerializeField] private Animator anim;
    [SerializeField] private GameObject goBackList;
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
            activeGoBack();
        }
        else SceneManager.LoadScene("WalkThoughtScene"); 
    }

    public void PressOptions()
    {
        if (isOnMenu)
        {
            anim.SetTrigger("ZoomToOptions");
            isOnMenu = false;
            isOnOptions = true;
            activeGoBack();

        }
        
    }

    public void PressCredits()
    {
        if (isOnMenu)
        {
            anim.SetTrigger("ZoomToCredits");
            isOnMenu = false;
            isOnCredits = true;
            activeGoBack();
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
        }
        else return;

        isOnMenu = true;
        goBackList.SetActive(false);
    }

    private void activeGoBack()
    {
        goBackList.SetActive(true);
    }
}
