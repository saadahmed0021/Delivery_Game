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
        if (orderManager == null)
            orderManager = FindFirstObjectByType<Order_Manager>();

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

        SetPanelVisible(true);

        if (orderManager == null || !orderManager.CompleteOrder())
        {
            Hide();
        }
    }

    public void ShowFailure(Order order)
    {
        if (order == null)
            return;

        if (orderNameText != null)
            orderNameText.text = order.OrderName;

        if (resultText != null)
            resultText.text = "Order Failed!";

        if (rewardText != null)
            rewardText.text = "No reward";

        SetPanelVisible(true);
    }

    public void Hide()
    {
        SetPanelVisible(false);
    }

    private void SetPanelVisible(bool visible)
    {
        if (resultPanel == null)
            return;

        Cursor_Controller cursorController = Cursor_Controller.Instance;

        if (cursorController != null)
        {
            if (visible)
                cursorController.OpenUI(resultPanel);
            else
                cursorController.CloseUI(resultPanel);
        }

        resultPanel.SetActive(visible);
    }

    public void Close()
    {
        Hide();

        if (uiController != null)
            uiController.ClosePanel();
    }
}