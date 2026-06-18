using System;
using UnityEngine;

public class UserInterfaceManager : MonoBehaviour
{
    public static UserInterfaceManager Instance { get; private set; }

    public enum PanelType { None, Inventory, Notes, Puzzle, Pause, Misc }

    [Header("Player Reference")]
    [SerializeField] GameObject player;
    PlayerMovement movement;
    PlayerCamera cam;

    #region Booleanos_UI
    public bool IsInventoryOpen => ActivePanel == PanelType.Inventory;
    public bool IsNoteOpen => ActivePanel == PanelType.Notes;
    public bool IsPuzzleOpen => ActivePanel == PanelType.Puzzle;
    public bool IsPauseOpen => ActivePanel == PanelType.Pause;
    public bool IsMiscOpen => ActivePanel == PanelType.Misc;
    #endregion

    public PanelType ActivePanel { get; private set; } = PanelType.None;
    PanelType pendingPanel = PanelType.None;

    Action openInventoryCallback;
    Action openNoteCallback;
    Action openPuzzleCallback;
    Action openPauseCallback;
    Action openMiscCallback;

    Action closeInventoryCallback;
    Action closeNoteCallback;
    Action closePuzzleCallback;
    Action closeMiscCallback;

    public bool IsAnyPanelOpen() => ActivePanel != PanelType.None;

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

    public void RegisterPanel(PanelType type, Action openTarget)
    {
        switch (type)
        {
            case PanelType.Inventory: openInventoryCallback = openTarget; break;
            case PanelType.Notes: openNoteCallback = openTarget; break;
            case PanelType.Puzzle: openPuzzleCallback = openTarget; break;
            case PanelType.Pause: openPauseCallback = openTarget; break;
            case PanelType.Misc: openMiscCallback = openTarget; break;
        }
    }

    public void RegisterForceClose(PanelType type, Action closeTarget)
    {
        switch (type)
        {
            case PanelType.Inventory: closeInventoryCallback = closeTarget; break;
            case PanelType.Notes: closeNoteCallback = closeTarget; break;
            case PanelType.Puzzle: closePuzzleCallback = closeTarget; break;
            case PanelType.Misc: closeMiscCallback = closeTarget; break;
        }
    }

    public bool RequestOpenPanel(PanelType type)
    {
        if (ActivePanel == type) return true;

        if (type == PanelType.Pause)
        {
            if (ActivePanel != PanelType.None)
            {
                TriggerForceClose(ActivePanel);
                pendingPanel = ActivePanel;
            }
            UpdatePanelState(type);
            return true;
        }

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
        else UpdatePanelState(PanelType.None);
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
        bool blockInputs = IsAnyPanelOpen();

        if (newPanel != PanelType.Pause)
        {
            if (cam != null) cam.LockCamera(blockInputs);
            if (movement != null) movement.CanMove(!blockInputs);
        }

        Cursor.lockState = blockInputs ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = blockInputs;
    }

    void TriggerOpenCallback(PanelType type)
    {
        switch (type)
        {
            case PanelType.Inventory: openInventoryCallback?.Invoke(); break;
            case PanelType.Notes: openNoteCallback?.Invoke(); break;
            case PanelType.Puzzle: openPuzzleCallback?.Invoke(); break;
            case PanelType.Pause: openPauseCallback?.Invoke(); break;
            case PanelType.Misc: openMiscCallback?.Invoke(); break;
        }
    }

    void TriggerForceClose(PanelType type)
    {
        switch (type)
        {
            case PanelType.Inventory: closeInventoryCallback?.Invoke(); break;
            case PanelType.Notes: closeNoteCallback?.Invoke(); break;
            case PanelType.Puzzle: closePuzzleCallback?.Invoke(); break;
            case PanelType.Misc: closeMiscCallback?.Invoke(); break;
        }
    }
}