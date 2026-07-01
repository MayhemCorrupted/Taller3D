using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
[System.Serializable]
public struct BookPage
{
    public Sprite bookPageSprite;
    [TextArea(2,3)] public string pageText;
    public UnityEvent onPassingPage;
}
public class ReadBookUI : MonoBehaviour
{
    [Header("Item Reference")]
    [SerializeField] ItemData bookItemData;
    [Header("GameObject References")]
    [SerializeField] GameObject bookPanel;
    [SerializeField] GameObject inspectContainer;
    [Header("Text and Image Components")]
    [SerializeField] TMP_Text pageTextComponent;
    [SerializeField] Image pageImageComponent;
    [Header("Button References")]
    [SerializeField] Button inspectBook;
    [SerializeField] Button nextPageButton;
    [SerializeField] Button previousPageButton;
    [SerializeField] Button closeButton;
    [Header("Events")]
    [SerializeField] UnityEvent onOpenPanel;
    [SerializeField] UnityEvent onClosePanel;
    [Header("Book Pages")]
    [SerializeField] BookPage[] bookPages;

    int index = 0;
    void Start()
    {
        if (UserInterfaceManager.Instance != null)
        {
            UserInterfaceManager.Instance.RegisterPanel(UserInterfaceManager.PanelType.Book, OpenBookPanel, CloseBookPanel);
        }

        if (nextPageButton != null) nextPageButton.onClick.AddListener(NextPage);
        if (previousPageButton != null) previousPageButton.onClick.AddListener(PreviousPage);
        if (closeButton != null) closeButton.onClick.AddListener(RequestClose);
        if (inspectBook != null) inspectBook.onClick.AddListener(ToggleInspect);
        if (inspectContainer != null) inspectContainer.SetActive(false);
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
            UserInterfaceManager.Instance.TryOpenPanel(UserInterfaceManager.PanelType.Book);
        }
    }
    void OpenBookPanel()
    {
        index = 0; 
        if (bookPanel != null) bookPanel.SetActive(true);
        onOpenPanel?.Invoke();
        UpdatePageVisuals();
    }
    void RequestClose()
    {
        if (UserInterfaceManager.Instance != null)
        {
            UserInterfaceManager.Instance.ClosePanel(UserInterfaceManager.PanelType.Book);
        }
    }
    void CloseBookPanel()
    {
        if (bookPanel != null) bookPanel.SetActive(false);
        onClosePanel?.Invoke();
    }
    void NextPage()
    {
        if (index < bookPages.Length - 1)
        {
            index++;
            UpdatePageVisuals();
        }
    }
    void ToggleInspect()
    {
        if (inspectContainer != null)
        {
            bool isActive = inspectContainer.activeSelf;
            inspectContainer.SetActive(!isActive);
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
        if (bookPages == null || bookPages.Length == 0) return;

        BookPage currentPage = bookPages[index];

        if (pageImageComponent != null)
        {
            pageImageComponent.sprite = currentPage.bookPageSprite;
            pageImageComponent.enabled = currentPage.bookPageSprite != null;
        }

        if (pageTextComponent != null) pageTextComponent.text = currentPage.pageText;

        if (previousPageButton != null) previousPageButton.gameObject.SetActive(index > 0);
        if (nextPageButton != null) nextPageButton.gameObject.SetActive(index < bookPages.Length - 1);

        currentPage.onPassingPage?.Invoke();
    }
}
