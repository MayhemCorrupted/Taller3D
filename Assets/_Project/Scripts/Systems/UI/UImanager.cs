using System;
using UnityEngine;

public class UImanager : MonoBehaviour
{
    public static UImanager Instance { get; private set; }
    public enum UIPanelType { None, Inventory, Notes, Puzzle }
    [Header("Player Reference")]
    [SerializeField] GameObject player;
    Player_Movement movement;
    Player_Camera cam;
    #region Booleanos_UI
    public bool IsInventoryOpen { get; private set; }
    public bool IsNoteOpen { get; private set; }
    public bool IsPuzzleOpen { get; private set; }
    #endregion
    UIPanelType activePanel = UIPanelType.None;
    UIPanelType pendingPanel = UIPanelType.None;

    Action openInventoryCallback;
    Action openNoteCallback;
    Action openPuzzleCallback;
    public bool IsAnyPanelOpen() => activePanel != UIPanelType.None;
    void Start()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        if (player != null)
        {
            movement = player.GetComponent<Player_Movement>();
            cam = player.GetComponent<Player_Camera>();
        }
    }
    public void RegisterPanel(UIPanelType type, Action openTarget)
    {
        switch (type)
        {
            case UIPanelType.Inventory: openInventoryCallback = openTarget; break;
            case UIPanelType.Notes: openNoteCallback = openTarget; break;
            case UIPanelType.Puzzle: openPuzzleCallback = openTarget; break;
        }
    }
    public void ForceTransitionTo(UIPanelType newPanel)
    {
        if (activePanel == newPanel || activePanel == UIPanelType.None) return;

        UIPanelType previousPanel = activePanel;

        UpdatePanelState(previousPanel, false);

        pendingPanel = previousPanel;

        UpdatePanelState(newPanel, true);
        TriggerOpenCallback(newPanel);
    }
    public bool RequestOpen(UIPanelType type)
    {
        if (activePanel == type) return true;
        if (activePanel != UIPanelType.None)
        {
            pendingPanel = type;
            return false;
        }
        UpdatePanelState(type, true);   
        return true;
    }
    public void ReportClose(UIPanelType panelType)
    {
        if (activePanel != panelType) return;
        UpdatePanelState(panelType, false);

        if (pendingPanel != UIPanelType.None)
        {
            UIPanelType nextPanel = pendingPanel;
            pendingPanel = UIPanelType.None;
            UpdatePanelState(nextPanel, true);
            TriggerOpenCallback(nextPanel);
        }
    }
    void UpdatePanelState(UIPanelType type, bool state)
    {
        activePanel = state ? type : UIPanelType.None;

        switch (type)
        {
            case UIPanelType.Inventory: IsInventoryOpen = state; break;
            case UIPanelType.Notes: IsNoteOpen = state; break;
            case UIPanelType.Puzzle: IsPuzzleOpen = state; break;
        }

        bool blockInputs = IsAnyPanelOpen();
        if (cam != null) cam.LockCamera(blockInputs);
        if (movement != null) movement.CanMove(!blockInputs);

        Cursor.lockState = blockInputs ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = blockInputs;
    }
    void TriggerOpenCallback(UIPanelType type)
    {
        switch (type)
        {
            case UIPanelType.Inventory: openInventoryCallback?.Invoke(); break;
            case UIPanelType.Notes: openNoteCallback?.Invoke(); break;
            case UIPanelType.Puzzle: openPuzzleCallback?.Invoke(); break;
        }
    }
}
