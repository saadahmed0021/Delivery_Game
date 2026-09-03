using System.Collections.Generic;
using UnityEngine;

public class Order_UI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Order_Manager orderManager;
    [SerializeField] private Order_UI_Controller uiController;
    [SerializeField] private Order_Card_UI orderCardPrefab;
    [SerializeField] private Transform orderCardContainer;
    [SerializeField] private Order_Details_UI detailsUI;

    private readonly List<Order_Card_UI> orderCards = new();

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        ClearCards();

        if (orderManager == null)
        {
            Debug.LogWarning("Order UI: Order Manager is not assigned.", this);
            return;
        }

        if (orderCardPrefab == null)
        {
            Debug.LogWarning("Order UI: Order Card Prefab is not assigned.", this);
            return;
        }

        if (orderCardContainer == null)
        {
            Debug.LogWarning("Order UI: Order Card Container is not assigned.", this);
            return;
        }

        if (detailsUI == null)
        {
            Debug.LogWarning("Order UI: Order Details UI is not assigned.", this);
            return;
        }

        IReadOnlyList<Order> orders = orderManager.AvailableOrders;

        for (int i = 0; i < orders.Count; i++)
        {
            CreateCard(orders[i]);
        }
    }

    private void CreateCard(Order order)
    {
        if (order == null)
            return;

        Order_Card_UI card = Instantiate(
            orderCardPrefab,
            orderCardContainer
        );

        if (card == null)
            return;

        card.Setup(order, detailsUI);

        orderCards.Add(card);
    }

    private void ClearCards()
    {
        for (int i = orderCards.Count - 1; i >= 0; i--)
        {
            if (orderCards[i] != null)
                Destroy(orderCards[i].gameObject);
        }

        orderCards.Clear();
    }

    public void Close()
    {
        if (uiController != null)
            uiController.ClosePanel();
        else
            gameObject.SetActive(false);
    }
}