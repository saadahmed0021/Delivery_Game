using System.Collections.Generic;
using UnityEngine;

public class Cursor_Controller : MonoBehaviour
{
    public static Cursor_Controller Instance;

    [SerializeField] private bool uiOpen;

    private readonly HashSet<GameObject> openUIPanels = new();
    private int anonymousUIRequests;

    public bool IsUIOpen => uiOpen;

    private void Awake()
    {
        Instance = this;
        ApplyCursorState();
    }

    public void SetGameplayCursor()
    {
        anonymousUIRequests = 0;
        ApplyCursorState();
    }

    public void SetUICursor()
    {
        anonymousUIRequests = Mathf.Max(1, anonymousUIRequests);
        ApplyCursorState();
    }

    public void OpenUI()
    {
        anonymousUIRequests++;
        ApplyCursorState();
    }

    public void CloseUI()
    {
        if (anonymousUIRequests > 0)
            anonymousUIRequests--;

        ApplyCursorState();
    }

    public void OpenUI(GameObject panel)
    {
        if (panel == null)
            return;

        openUIPanels.Add(panel);
        ApplyCursorState();
    }

    public void CloseUI(GameObject panel)
    {
        if (panel == null)
            return;

        openUIPanels.Remove(panel);
        ApplyCursorState();
    }

    private void ApplyCursorState()
    {
        uiOpen = anonymousUIRequests > 0 || openUIPanels.Count > 0;
        Cursor.visible = uiOpen;
        Cursor.lockState = uiOpen
            ? CursorLockMode.None
            : CursorLockMode.Locked;
    }
}