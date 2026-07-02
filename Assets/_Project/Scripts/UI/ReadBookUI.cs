using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
[System.Serializable]
public struct BookPage
{
    public Sprite rightPageSprite;
    public Sprite leftPageSprite;
    [Space(3)]
    [TextArea(2,3)] public string leftPageText;
    [TextArea(2,3)] public string rightPageText;
    public UnityEvent onPassingPage;
}
public class ReadBookUI : MonoBehaviour
{
    [Header("Main References")]
    [SerializeField] ItemData bookItemData;
    [SerializeField] GameObject bookPanel;
    [Header("Left Side References")]
    [SerializeField] GameObject rightInspectContainer;
    [SerializeField] GameObject leftInspectContainer;
    [Space(3)]
    [SerializeField] TMP_Text rightTextComponent;
    [SerializeField] TMP_Text leftTextComponent;
    [Space(3)]
    [SerializeField] Image rightImageComponent;
    [SerializeField] Image leftImageComponent;
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
        if (leftInspectContainer != null) leftInspectContainer.SetActive(false);
        if (rightInspectContainer != null) rightInspectContainer.SetActive(false);
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
        if (leftInspectContainer != null)
        {
            bool isActive = leftInspectContainer.activeSelf && rightInspectContainer.activeSelf;
            leftInspectContainer.SetActive(!isActive);
            rightInspectContainer.SetActive(!isActive);
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

        if (leftImageComponent != null)
        {
            leftImageComponent.sprite = currentPage.leftPageSprite;
            leftImageComponent.enabled = currentPage.leftPageSprite != null;
        }
        if (rightImageComponent != null)
        {
            rightImageComponent.sprite = currentPage.rightPageSprite;
            rightImageComponent.enabled = currentPage.rightPageSprite != null;
        }

        if (leftTextComponent != null) leftTextComponent.text = currentPage.leftPageText;
        if (rightTextComponent != null) rightTextComponent.text= currentPage.rightPageText;

        if (previousPageButton != null) previousPageButton.gameObject.SetActive(index > 0);
        if (nextPageButton != null) nextPageButton.gameObject.SetActive(index < bookPages.Length - 1);

        currentPage.onPassingPage?.Invoke();
    }
}
