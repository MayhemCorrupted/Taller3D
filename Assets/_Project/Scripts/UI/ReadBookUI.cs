using UnityEngine;
using UnityEngine.UI;

public class ReadBookUI : MonoBehaviour
{
    [SerializeField] GameObject bookPanel;
    [SerializeField] GameObject[] bookPages;
    [SerializeField] GameObject[] bookText;
    [SerializeField] ItemData bookItemData;
    [SerializeField] Button inspectBook;
    [SerializeField] Button nextPageButton;
    [SerializeField] Button previousPageButton;
    [SerializeField] Button closeButton;
    int index = 0;
    readonly UserInterfaceManager.PanelType bookPanelType = UserInterfaceManager.PanelType.Notes;
    void Start()
    {
        if (UserInterfaceManager.Instance != null)
        {
            UserInterfaceManager.Instance.RegisterPanel(bookPanelType, OpenBookPanel, CloseBookPanel);
        }

        if (nextPageButton != null) nextPageButton.onClick.AddListener(NextPage);
        if (previousPageButton != null) previousPageButton.onClick.AddListener(PreviousPage);
        if (closeButton != null) closeButton.onClick.AddListener(RequestClose);
        if (bookPanel != null) bookPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryOpenBook();
        }

    }
    void TryOpenBook()
    {
        if (UserInterfaceManager.Instance != null && UserInterfaceManager.Instance.IsAnyPanelOpen()) return;

        if (EquipmentManager.Instance != null && EquipmentManager.Instance.CurrentEquippedItem == bookItemData)
        {
            UserInterfaceManager.Instance.TryOpenPanel(bookPanelType);
        }
    }
    void OpenBookPanel()
    {
        index = 0; 
        if (bookPanel != null) bookPanel.SetActive(true);
        UpdatePageVisuals();
    }
    void RequestClose()
    {
        if (UserInterfaceManager.Instance != null)
        {
            UserInterfaceManager.Instance.ClosePanel(bookPanelType);
        }
    }
    void CloseBookPanel()
    {
        if (bookPanel != null) bookPanel.SetActive(false);
    }
    void NextPage()
    {
        if (index < bookPages.Length - 1)
        {
            index++;
            UpdatePageVisuals();
        }
    }
    void PreviousPage()
    {
        if (index > 0)
        {
            index--;
            UpdatePageVisuals();
        }
    }
    void UpdatePageVisuals()
    {
        for (int i = 0; i < bookPages.Length; i++)
        {
            if (bookPages[i] != null) bookPages[i].SetActive(i == index);
        }

        for (int i = 0; i < bookText.Length; i++)
        {
            if (bookText[i] != null) bookText[i].SetActive(i == index);
        }

        if (previousPageButton != null) previousPageButton.gameObject.SetActive(index > 0);
        if (nextPageButton != null) nextPageButton.gameObject.SetActive(index < bookPages.Length - 1);
    }
}
