using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Order_Result_UI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Order_Manager orderManager;
    [SerializeField] private Order_UI_Controller uiController;

    [Header("UI")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text orderNameText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(Close);

        Hide();
    }

    private void OnDestroy()
    {
        if (closeButton != null)
            closeButton.onClick.RemoveListener(Close);
    }

    public void Show(Order order)
    {
        if (order == null)
            return;

        if (!order.IsDelivered)
            return;

        if (orderNameText != null)
            orderNameText.text = order.OrderName;

        if (resultText != null)
            resultText.text = "Order Delivered!";

        if (rewardText != null)
            rewardText.text = $"Reward: ${order.Reward}";

        if (resultPanel != null)
            resultPanel.SetActive(true);

        orderManager?.CompleteOrder();
    }

    public void Hide()
    {
        if (resultPanel != null)
            resultPanel.SetActive(false);
    }

    public void Close()
    {
        Hide();

        if (uiController != null)
            uiController.ClosePanel();
    }
}