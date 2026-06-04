using System;
using UnityEngine;

public class UserInterfaceManager : MonoBehaviour
{
    public static UserInterfaceManager Instance { get; private set; }

    public enum PanelType { None, Inventory, Notes, Puzzle }

    [Header("Player Reference")]
    [SerializeField] GameObject player;

    PlayerMovement movement;
    PlayerCamera cam;

    public PanelType ActivePanel { get; private set; } = PanelType.None;
    PanelType pendingPanel = PanelType.None;

    Action openInventoryCallback;
    Action openNoteCallback;
    Action openPuzzleCallback;

    #region Public State Checks

    public bool IsInventoryOpen => ActivePanel == PanelType.Inventory;
    public bool IsNoteOpen => ActivePanel == PanelType.Notes;
    public bool IsPuzzleOpen => ActivePanel == PanelType.Puzzle;
    public bool IsAnyPanelOpen() => ActivePanel != PanelType.None;

    #endregion

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (player != null)
        {
            movement = player.GetComponent<PlayerMovement>();
            cam = player.GetComponent<PlayerCamera>();
        }
    }

    public void RegisterPanel(PanelType type, Action openCallback)
    {
        switch (type)
        {
            case PanelType.Inventory: openInventoryCallback = openCallback; break;
            case PanelType.Notes: openNoteCallback = openCallback; break;
            case PanelType.Puzzle: openPuzzleCallback = openCallback; break;
        }
    }

    public bool RequestOpenPanel(PanelType type)
    {
        if (ActivePanel == type) return true;

        if (ActivePanel != PanelType.None)
        {
            pendingPanel = type;
            return false;
        }

        UpdatePanelState(type);
        return true;
    }

    public void ReportClosedPanel(PanelType panelType)
    {
        if (ActivePanel != panelType) return;

        if (pendingPanel != PanelType.None)
        {
            PanelType next = pendingPanel;
            pendingPanel = PanelType.None;
            UpdatePanelState(next);
            TriggerOpenCallback(next);
        }
        else
        {
            UpdatePanelState(PanelType.None);
        }
    }

    public void ForceTransitionTo(PanelType newPanel)
    {
        if (ActivePanel == newPanel || ActivePanel == PanelType.None) return;

        pendingPanel = ActivePanel;
        UpdatePanelState(newPanel);
        TriggerOpenCallback(newPanel);
    }

    void UpdatePanelState(PanelType newPanel)
    {
        ActivePanel = newPanel;
        bool blocking = IsAnyPanelOpen();

        if (cam != null) cam.LockCamera(!blocking == false); 
        if (movement != null) movement.CanMove(!blocking);

        Cursor.lockState = blocking ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = blocking;
    }

    void TriggerOpenCallback(PanelType type)
    {
        switch (type)
        {
            case PanelType.Inventory: openInventoryCallback?.Invoke(); break;
            case PanelType.Notes: openNoteCallback?.Invoke(); break;
            case PanelType.Puzzle: openPuzzleCallback?.Invoke(); break;
        }
    }
}