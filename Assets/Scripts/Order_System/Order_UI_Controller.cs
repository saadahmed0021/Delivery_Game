using UnityEngine;

public class Order_UI_Controller : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject orderPanel;
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
        if (orderPanel == null)
        {
            return;
        }

        orderPanel.SetActive(state);
    }
}