using System.Collections.Generic;
using UnityEngine;

public class Order_Manager : MonoBehaviour
{
    [SerializeField] private Order_Currency_Reward currencyReward;
    [SerializeField] private Order_Travel_Time travelTime;
    [SerializeField] private Order_Generator orderGenerator;

    private readonly List<Order> availableOrders = new();

    private Order activeOrder;
    private int activeOrderSlotIndex = -1;

    public IReadOnlyList<Order> AvailableOrders => availableOrders;
    public Order ActiveOrder => activeOrder;

    public bool HasActiveOrder => activeOrder != null;

    public void InsertAvailableOrder(Order order, int index)
    {
        if (order == null)
            return;

        if (!order.IsAvailable)
            return;

        if (availableOrders.Contains(order))
            return;

        if (index < 0)
            index = 0;

        if (index > availableOrders.Count)
            index = availableOrders.Count;

        availableOrders.Insert(index, order);
    }

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

        int orderIndex = availableOrders.IndexOf(order);

        if (orderIndex < 0)
            return false;

        if (!order.Accept())
            return false;

        activeOrder = order;
        activeOrderSlotIndex = orderIndex;
        availableOrders.RemoveAt(orderIndex);

        if (currencyReward == null)
            currencyReward = GetComponent<Order_Currency_Reward>();

        if (travelTime == null)
            travelTime = FindFirstObjectByType<Order_Travel_Time>();

        currencyReward?.ResetReward();
        travelTime?.StartPickupCountdown(activeOrder);

        return true;
    }

    public bool PickupOrder()
    {
        if (activeOrder == null)
            return false;

        if (!activeOrder.Pickup())
            return false;

        if (travelTime == null)
            travelTime = FindFirstObjectByType<Order_Travel_Time>();

        travelTime?.StartDeliveryCountdown(activeOrder);

        return true;
    }

    public bool DeliverOrder()
    {
        if (activeOrder == null)
            return false;

        if (!activeOrder.Deliver())
            return false;

        if (travelTime == null)
            travelTime = FindFirstObjectByType<Order_Travel_Time>();

        travelTime?.StopCountdown();

        return true;
    }

    public bool CompleteOrder()
    {
        if (activeOrder == null)
            return false;

        if (!activeOrder.IsDelivered)
            return false;

        if (currencyReward == null)
            currencyReward = GetComponent<Order_Currency_Reward>();

        if (currencyReward == null ||
            !currencyReward.GiveReward(activeOrder.Reward))
        {
            return false;
        }

        if (!activeOrder.Complete())
            return false;

        if (travelTime == null)
            travelTime = FindFirstObjectByType<Order_Travel_Time>();

        travelTime?.StopCountdown();

        int replacedOrderIndex = activeOrderSlotIndex;
        activeOrder = null;
        activeOrderSlotIndex = -1;

        if (replacedOrderIndex >= 0)
        {
            Order_Generator generator = orderGenerator != null
                ? orderGenerator
                : FindFirstObjectByType<Order_Generator>();

            if (generator != null)
                generator.GenerateReplacementOrder(replacedOrderIndex);
        }

        return true;
    }

    public bool CancelActiveOrder()
    {
        if (activeOrder == null)
            return false;

        if (!activeOrder.Cancel())
            return false;

        if (travelTime == null)
            travelTime = FindFirstObjectByType<Order_Travel_Time>();

        travelTime?.StopCountdown();

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