using System.Collections.Generic;
using UnityEngine;

public class Order_Manager : MonoBehaviour
{
    [Header("Order Setup")]
    [SerializeField] private Order orderPrefab;
    [SerializeField] private Transform orderContainer;
    [SerializeField] private int ordersToGenerate = 4;

    private readonly List<Order> availableOrders = new();
    private Order activeOrder;

    public IReadOnlyList<Order> AvailableOrders => availableOrders;
    public Order ActiveOrder => activeOrder;

    public bool HasActiveOrder => activeOrder != null;

    private void Start()
    {
        GenerateOrders();
    }

    public void SetAvailableOrders(List<Order> orders)
    {
        ClearAvailableOrders();

        if (orders == null)
            return;

        foreach (Order order in orders)
        {
            if (order == null)
                continue;

            availableOrders.Add(order);
        }
    }

    public void AddOrder(Order order)
    {
        if (order == null || availableOrders.Contains(order))
            return;

        availableOrders.Add(order);
    }

    public bool AcceptOrder(Order order)
    {
        if (order == null)
            return false;

        if (activeOrder != null)
            return false;

        if (!availableOrders.Contains(order))
            return false;

        if (!order.Accept())
            return false;

        activeOrder = order;
        availableOrders.Remove(order);

        return true;
    }

    public bool PickupOrder()
    {
        if (activeOrder == null)
            return false;

        return activeOrder.Pickup();
    }

    public bool DeliverOrder()
    {
        if (activeOrder == null)
            return false;

        return activeOrder.Deliver();
    }

    public bool CompleteOrder()
    {
        if (activeOrder == null)
            return false;

        if (!activeOrder.Complete())
            return false;

        activeOrder = null;
        return true;
    }

    public bool CancelActiveOrder()
    {
        if (activeOrder == null)
            return false;

        if (!activeOrder.Cancel())
            return false;

        activeOrder = null;
        return true;
    }

    public void GenerateOrders()
    {
        ClearAvailableOrders();

        if (orderPrefab == null)
        {
            Debug.LogError("Order Manager: Order Prefab is not assigned.", this);
            return;
        }

        int count = Mathf.Max(1, ordersToGenerate);

        for (int i = 0; i < count; i++)
        {
            Order order = InstantiateOrder();

            if (order != null)
                availableOrders.Add(order);
        }
    }

    private Order InstantiateOrder()
    {
        Transform parent = orderContainer != null ? orderContainer : transform;

        Order order = Instantiate(orderPrefab, parent);

        if (order == null)
            return null;

        order.gameObject.name = $"Order_{availableOrders.Count + 1}";

        return order;
    }

    public void ClearAvailableOrders()
    {
        for (int i = availableOrders.Count - 1; i >= 0; i--)
        {
            Order order = availableOrders[i];

            if (order != null)
                Destroy(order.gameObject);
        }

        availableOrders.Clear();
    }

    public void RefreshOrders()
    {
        if (activeOrder != null)
            return;

        GenerateOrders();
    }
}