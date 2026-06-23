using System;
using System.Collections.Generic;
using UnityEngine;

public class UserInterfaceManager : MonoBehaviour
{
    public static UserInterfaceManager Instance { get; private set; }

    public enum PanelType { None, Inventory, Notes, Switch, Draggable, Dial, Panel, Sequence, Pause }

    [Header("Player Reference")]
    [SerializeField] private GameObject player;
    private PlayerMovement movement;
    private PlayerCamera cam;
    public PanelType ActivePanel { get; private set; } = PanelType.None;

    private readonly Dictionary<PanelType, Action> openCallbacks = new();
    private readonly Dictionary<PanelType, Action> closeCallbacks = new();
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
    public void RegisterPanel(PanelType type, Action openTarget, Action closeTarget)
    {
        if (!openCallbacks.TryAdd(type, openTarget)) openCallbacks[type] = openTarget;
        if (!closeCallbacks.TryAdd(type, closeTarget)) closeCallbacks[type] = closeTarget;
    }
    public void TogglePanel(PanelType type)
    {
        if (ActivePanel == type)
            ClosePanel(type);
        else
            TryOpenPanel(type);
    }
    public void TryOpenPanel(PanelType type)
    {
        if (ActivePanel != PanelType.None)
        {
            if (type == PanelType.Pause)
            {
                TriggerForceClose(ActivePanel);
                UpdatePanelState(type, true);
                TriggerOpenCallback(type);
            }
            return;
        } 

        UpdatePanelState(type, type == PanelType.Pause);
        TriggerOpenCallback(type);
    }
    public void ClosePanel(PanelType type)
    {
        if (ActivePanel != type) return;

        TriggerForceClose(type);
        UpdatePanelState(PanelType.None, false);
    }
    public void ForceTransitionTo(PanelType newPanel)
    {
        if (ActivePanel == newPanel) return;

        if (ActivePanel != PanelType.None) TriggerForceClose(ActivePanel);

        if (newPanel == PanelType.Pause) Time.timeScale = 0f;
        else Time.timeScale = 1f;

        UpdatePanelState(newPanel, newPanel == PanelType.Pause);
        TriggerOpenCallback(newPanel);
    }
    private void UpdatePanelState(PanelType newPanel, bool isPause)
    {
        ActivePanel = newPanel;

        Time.timeScale = isPause ? 0f : 1f;

        bool blockInputs = IsAnyPanelOpen();

        if (cam != null) cam.LockCamera(blockInputs);
        if (movement != null) movement.CanMove(!blockInputs);

        Cursor.lockState = blockInputs ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = blockInputs;
    }
    private void TriggerOpenCallback(PanelType type)
    {
        if (openCallbacks.TryGetValue(type, out Action callback)) callback?.Invoke();
    }
    private void TriggerForceClose(PanelType type)
    {
        if (closeCallbacks.TryGetValue(type, out Action callback)) callback?.Invoke();
    }
}