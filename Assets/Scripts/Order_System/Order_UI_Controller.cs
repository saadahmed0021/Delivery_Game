using UnityEngine;

public class Order_UI_Controller : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject orderPanel;

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
        SetPanelState(!IsOpen);
    }

    public void OpenPanel()
    {
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
            Debug.LogWarning("Order UI Controller: Order Panel is not assigned.", this);
            return;
        }

        orderPanel.SetActive(state);
    }
}