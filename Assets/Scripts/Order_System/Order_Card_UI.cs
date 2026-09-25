using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Order_Card_UI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text orderNameText;
    [SerializeField] private TMP_Text restaurantNameText;
    [SerializeField] private TMP_Text customerNameText;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private TMP_Text travelTimeText;
    [SerializeField] private Button selectButton;

    private Order_Details_UI detailsUI;
    private Order order;

    public Order Order => order;

    private void Awake()
    {
        if (selectButton != null)
            selectButton.onClick.AddListener(SelectOrder);
    }

    private void OnDestroy()
    {
        if (selectButton != null)
            selectButton.onClick.RemoveListener(SelectOrder);
    }

    public void Setup(Order targetOrder, Order_Details_UI targetDetailsUI)
    {
        Setup(targetOrder, targetDetailsUI, string.Empty);
    }

    public void Setup(
        Order targetOrder,
        Order_Details_UI targetDetailsUI,
        string pickupTimeFormatted)
    {
        order = targetOrder;
        detailsUI = targetDetailsUI;

        if (order == null)
        {
            Clear();
            return;
        }

        UpdateUI(pickupTimeFormatted);
    }

    private void UpdateUI(string pickupTimeFormatted)
    {
        if (orderNameText != null)
            orderNameText.text = order.OrderName;

        if (restaurantNameText != null)
            restaurantNameText.text = order.RestaurantName;

        if (customerNameText != null)
            customerNameText.text = order.CustomerName;

        if (rewardText != null)
            rewardText.text = $"${order.Reward}";

        if (travelTimeText != null)
            travelTimeText.text = pickupTimeFormatted;
    }

    private void SelectOrder()
    {
        if (order == null)
            return;

        if (detailsUI == null)
        {
            return;
        }

        detailsUI.Show(order);
    }

    private void Clear()
    {
        if (orderNameText != null)
            orderNameText.text = string.Empty;

        if (restaurantNameText != null)
            restaurantNameText.text = string.Empty;

        if (customerNameText != null)
            customerNameText.text = string.Empty;

        if (rewardText != null)
            rewardText.text = string.Empty;

        if (travelTimeText != null)
            travelTimeText.text = string.Empty;

        if (selectButton != null)
            selectButton.interactable = false;
    }
}