using System.Collections.Generic;
using UnityEngine;

public class Order_Generator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Order orderPrefab;
    [SerializeField] private Transform orderContainer;

    [Header("Order Pool")]
    [SerializeField, Min(1)] private int orderCount = 4;
    [SerializeField] private List<Order_Data> orderDataList = new();

    public int OrderCount => orderCount;

    public List<Order> GenerateOrders()
    {
        List<Order> generatedOrders = new();

        if (!ValidateSetup())
            return generatedOrders;

        int count = Mathf.Min(orderCount, orderDataList.Count);

        List<Order_Data> availableData = new(orderDataList);

        Shuffle(availableData);

        for (int i = 0; i < count; i++)
        {
            Order order = CreateOrder(availableData[i]);

            if (order != null)
                generatedOrders.Add(order);
        }

        return generatedOrders;
    }

    private Order CreateOrder(Order_Data data)
    {
        Transform parent = orderContainer != null
            ? orderContainer
            : transform;

        Order order = Instantiate(orderPrefab, parent);

        if (order == null)
            return null;

        order.Initialize(data);
        order.name = $"Order_{data.OrderName}";

        return order;
    }

    private bool ValidateSetup()
    {
        if (orderPrefab == null)
        {
            Debug.LogError("Order Generator: Order Prefab is not assigned.", this);
            return false;
        }

        if (orderDataList == null || orderDataList.Count == 0)
        {
            Debug.LogError("Order Generator: No Order Data assets assigned.", this);
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