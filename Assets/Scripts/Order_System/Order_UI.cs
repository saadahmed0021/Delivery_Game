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
    [SerializeField] private Order_Travel_Time travelTime;

    [Header("Card Positions")]
    [SerializeField] private Vector2 firstCardPosition = new Vector2(-11.9187f, 294f);
    [SerializeField] private Vector2 secondCardPosition = new Vector2(-11.9187f, 25f);
    [SerializeField] private Vector2 thirdCardPosition = new Vector2(-11.9187f, -244f);

    private readonly List<Order_Card_UI> orderCards = new();

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        ClearCards();

        if (travelTime == null)
        {
            travelTime = FindFirstObjectByType<Order_Travel_Time>();
        }

        if (orderManager == null)
        {
            return;
        }

        if (orderCardPrefab == null)
        {
            return;
        }

        if (orderCardContainer == null)
        {
            return;
        }

        if (detailsUI == null)
        {
            return;
        }

        IReadOnlyList<Order> orders = orderManager.AvailableOrders;

        for (int i = 0; i < orders.Count; i++)
        {
            CreateCard(orders[i], i);
        }
    }

    private void CreateCard(Order order, int index)
    {
        if (order == null)
            return;

        Order_Card_UI card = Instantiate(
            orderCardPrefab,
            orderCardContainer
        );

        if (card == null)
            return;

        RectTransform rect = card.GetComponent<RectTransform>();

        if (rect != null)
            rect.anchoredPosition = GetCardPosition(index);

        string pickupTime = string.Empty;
        Order_Location restaurant = order.RestaurantLocation;

        if (travelTime != null && restaurant != null &&
            travelTime.CalculatePickupTime(restaurant.Transform))
        {
            pickupTime = travelTime.PickupTimeDisplayFormatted;
        }

        card.Setup(order, detailsUI, pickupTime);

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

    private Vector2 GetCardPosition(int index)
    {
        switch (index)
        {
            case 0:
                return firstCardPosition;

            case 1:
                return secondCardPosition;

            case 2:
                return thirdCardPosition;

            default:
                return Vector2.zero;
        }
    }

    public void Close()
    {
        if (uiController != null)
            uiController.ClosePanel();
        else
            gameObject.SetActive(false);
    }
}