using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Order_Details_UI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Order_Manager orderManager;
    [SerializeField] private Order_UI_Controller uiController;

    [Header("UI")]
    [SerializeField] private GameObject detailsPanel;
    [SerializeField] private TMP_Text orderNameText;
    [SerializeField] private TMP_Text restaurantNameText;
    [SerializeField] private TMP_Text customerNameText;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button closeButton;

    private Order selectedOrder;

    public Order SelectedOrder => selectedOrder;

    private void Awake()
    {
        if (acceptButton != null)
            acceptButton.onClick.AddListener(AcceptOrder);

        if (closeButton != null)
            closeButton.onClick.AddListener(Close);

        Hide();
    }

    private void OnDestroy()
    {
        if (acceptButton != null)
            acceptButton.onClick.RemoveListener(AcceptOrder);

        if (closeButton != null)
            closeButton.onClick.RemoveListener(Close);
    }

    public void Show(Order order)
    {
        if (order == null)
            return;

        selectedOrder = order;

        UpdateUI();

        if (detailsPanel != null)
            detailsPanel.SetActive(true);
    }

    public void Hide()
    {
        selectedOrder = null;

        if (detailsPanel != null)
            detailsPanel.SetActive(false);
    }

    private void UpdateUI()
    {
        if (selectedOrder == null)
            return;

        if (orderNameText != null)
            orderNameText.text = selectedOrder.OrderName;

        if (restaurantNameText != null)
            restaurantNameText.text = selectedOrder.RestaurantName;

        if (customerNameText != null)
            customerNameText.text = selectedOrder.CustomerName;

        if (rewardText != null)
            rewardText.text = $"${selectedOrder.Reward}";

        if (acceptButton != null)
            acceptButton.interactable = CanAcceptOrder();
    }

    private void AcceptOrder()
    {
        if (!CanAcceptOrder())
            return;

        if (!orderManager.AcceptOrder(selectedOrder))
            return;

        Hide();

        if (uiController != null)
            uiController.ClosePanel();
    }

    private bool CanAcceptOrder()
    {
        if (selectedOrder == null)
            return false;

        if (orderManager == null)
            return false;

        if (orderManager.HasActiveOrder)
            return false;

        return selectedOrder.IsAvailable;
    }

    public void Close()
    {
        Hide();
    }
}