using System.Collections.Generic;
using UnityEngine;

public class Order_Manager : MonoBehaviour
{
    private readonly List<Order> availableOrders = new();

    private Order activeOrder;

    public IReadOnlyList<Order> AvailableOrders => availableOrders;
    public Order ActiveOrder => activeOrder;

    public bool HasActiveOrder => activeOrder != null;

    public void SetAvailableOrders(List<Order> orders)
    {
        availableOrders.Clear();

        if (orders == null)
            return;

        foreach (Order order in orders)
        {
            if (order != null && order.IsAvailable)
                availableOrders.Add(order);
        }
    }

    public void AddOrder(Order order)
    {
        if (order == null)
            return;

        if (!order.IsAvailable)
            return;

        if (availableOrders.Contains(order))
            return;

        availableOrders.Add(order);
    }

    public bool AcceptOrder(Order order)
    {
        if (order == null)
            return false;

        if (HasActiveOrder)
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

    public void RefreshOrders(List<Order> orders)
    {
        if (HasActiveOrder)
            return;

        SetAvailableOrders(orders);
    }
}