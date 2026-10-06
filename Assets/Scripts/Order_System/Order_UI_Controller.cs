using UnityEngine;

public class Order_UI_Controller : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject orderPanel;
    [SerializeField] private GameObject backgroundPanel;
    [SerializeField] private Order_Manager orderManager;

    [Header("Input")]
    [SerializeField] private KeyCode toggleKey = KeyCode.M;

    public bool IsOpen => orderPanel != null && orderPanel.activeSelf;

    private void Awake()
    {
        SetPanelState(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            TogglePanel();
    }

    public void TogglePanel()
    {
        if (orderManager != null && orderManager.HasActiveOrder)
            return;

        SetPanelState(!IsOpen);
    }

    public void OpenPanel()
    {
        if (orderManager != null && orderManager.HasActiveOrder)
            return;

        SetPanelState(true);
    }

    public void ClosePanel()
    {
        SetPanelState(false);
    }

    private void SetPanelState(bool state)
    {
        if (orderPanel == null && backgroundPanel == null)
            return;

        bool orderPanelMatches = orderPanel == null || orderPanel.activeSelf == state;
        bool backgroundPanelMatches = backgroundPanel == null || backgroundPanel.activeSelf == state;

        if (orderPanelMatches && backgroundPanelMatches)
            return;

        Cursor_Controller cursorController = Cursor_Controller.Instance;

        if (cursorController != null)
        {
            if (state)
            {
                cursorController.OpenUI(orderPanel);
                cursorController.OpenUI(backgroundPanel);
            }
            else
            {
                cursorController.CloseUI(orderPanel);
                cursorController.CloseUI(backgroundPanel);
            }
        }

        if (orderPanel != null)
            orderPanel.SetActive(state);

        if (backgroundPanel != null)
            backgroundPanel.SetActive(state);
    }
}