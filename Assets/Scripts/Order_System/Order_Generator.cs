using System.Collections.Generic;
using UnityEngine;

public class Order_Generator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Order_Manager orderManager;
    [SerializeField] private Order orderPrefab;
    [SerializeField] private Transform orderContainer;

    [Header("Order Pool")]
    [SerializeField, Min(1)] private int orderCount = 3;
    [SerializeField] private List<Order_Data> orderDataList = new();

    [Header("Card Positions")]
    [SerializeField] private Vector2 firstCardPosition = new Vector2(-11.9187f, 294f);
    [SerializeField] private Vector2 secondCardPosition = new Vector2(-11.9187f, 25f);
    [SerializeField] private Vector2 thirdCardPosition = new Vector2(11.9187f, -244f);

    public int OrderCount => orderCount;

    private void Start()
    {
        GenerateAndSetOrders();
    }

    public void GenerateAndSetOrders()
    {
        if (!ValidateSetup())
            return;

        if (orderManager.HasActiveOrder)
            return;

        orderManager.ClearAvailableOrders();
        ClearGeneratedCards();

        List<Order> generatedOrders = GenerateOrders();

        orderManager.SetAvailableOrders(generatedOrders);
    }

    public List<Order> GenerateOrders()
    {
        List<Order> generatedOrders = new();

        if (!ValidateGeneratorSetup())
            return generatedOrders;

        int count = Mathf.Min(orderCount, orderDataList.Count);

        List<Order_Data> availableData = new(orderDataList);

        Shuffle(availableData);

        for (int i = 0; i < count; i++)
        {
            Order order = CreateOrder(availableData[i], i);

            if (order != null)
                generatedOrders.Add(order);
        }

        return generatedOrders;
    }

    public Order GenerateReplacementOrder(int slotIndex)
    {
        if (!ValidateGeneratorSetup())
            return null;

        if (orderManager == null)
        {
            Debug.LogError(
                "Order Generator: Order Manager is not assigned.",
                this
            );

            return null;
        }

        if (orderDataList == null || orderDataList.Count == 0)
            return null;

        int randomIndex = Random.Range(0, orderDataList.Count);
        Order_Data randomData = orderDataList[randomIndex];
        Order replacementOrder = CreateOrder(randomData, slotIndex);

        if (replacementOrder == null)
            return null;

        orderManager.InsertAvailableOrder(replacementOrder, slotIndex);

        return replacementOrder;
    }

    private Order CreateOrder(Order_Data data, int index)
    {
        if (data == null)
            return null;

        Transform parent = orderContainer != null
            ? orderContainer
            : transform;

        Order order = Instantiate(orderPrefab, parent, false);

        if (order == null)
            return null;

        order.Initialize(data);
        order.name = $"Order_{data.OrderName}";

        RectTransform rect = order.GetComponent<RectTransform>();

        if (rect != null)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;

            rect.anchoredPosition = GetCardPosition(index);
        }

        return order;
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

    private void ClearGeneratedCards()
    {
        Transform parent = orderContainer != null
            ? orderContainer
            : transform;

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }

    private bool ValidateSetup()
    {
        if (orderManager == null)
        {
            Debug.LogError(
                "Order Generator: Order Manager is not assigned.",
                this
            );

            return false;
        }

        return ValidateGeneratorSetup();
    }

    private bool ValidateGeneratorSetup()
    {
        if (orderPrefab == null)
        {
            Debug.LogError(
                "Order Generator: Order Prefab is not assigned.",
                this
            );

            return false;
        }

        if (orderDataList == null || orderDataList.Count == 0)
        {
            Debug.LogError(
                "Order Generator: No Order Data assets assigned.",
                this
            );

            return false;
        }

        return true;
    }

    private void Shuffle(List<Order_Data> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            (list[i], list[randomIndex]) =
                (list[randomIndex], list[i]);
        }
    }
}