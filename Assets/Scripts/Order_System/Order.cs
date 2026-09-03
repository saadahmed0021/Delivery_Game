using System;
using UnityEngine;

public class Order : MonoBehaviour
{
    public enum OrderState
    {
        Available,
        Accepted,
        PickedUp,
        Delivered,
        Completed,
        Cancelled
    }

    [Header("Order Data")]
    [SerializeField] private Order_Data data;

    private OrderState currentState = OrderState.Available;

    public Order_Data Data => data;
    public OrderState State => currentState;

    public string OrderName => data != null ? data.OrderName : string.Empty;
    public string RestaurantName => data != null ? data.RestaurantName : string.Empty;
    public string CustomerName => data != null ? data.CustomerName : string.Empty;
    public int Reward => data != null ? data.BaseReward : 0;

    public Order_Location RestaurantLocation =>
        data != null ? data.RestaurantLocation : null;

    public Order_Location CustomerLocation =>
        data != null ? data.CustomerLocation : null;

    public bool IsAvailable => currentState == OrderState.Available;
    public bool IsAccepted => currentState == OrderState.Accepted;
    public bool IsPickedUp => currentState == OrderState.PickedUp;
    public bool IsDelivered => currentState == OrderState.Delivered;
    public bool IsCompleted => currentState == OrderState.Completed;

    public event Action<OrderState> OnStateChanged;

    public void Initialize(Order_Data orderData)
    {
        if (orderData == null)
        {
            Debug.LogError("Order cannot be initialized with null Order_Data.", this);
            return;
        }

        data = orderData;
        currentState = OrderState.Available;

        OnStateChanged?.Invoke(currentState);
    }

    public bool Accept()
    {
        if (currentState != OrderState.Available)
            return false;

        SetState(OrderState.Accepted);
        return true;
    }

    public bool Pickup()
    {
        if (currentState != OrderState.Accepted)
            return false;

        SetState(OrderState.PickedUp);
        return true;
    }

    public bool Deliver()
    {
        if (currentState != OrderState.PickedUp)
            return false;

        SetState(OrderState.Delivered);
        return true;
    }

    public bool Complete()
    {
        if (currentState != OrderState.Delivered)
            return false;

        SetState(OrderState.Completed);
        return true;
    }

    public bool Cancel()
    {
        if (currentState == OrderState.Completed ||
            currentState == OrderState.Cancelled)
        {
            return false;
        }

        SetState(OrderState.Cancelled);
        return true;
    }

    private void SetState(OrderState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;
        OnStateChanged?.Invoke(currentState);
    }
}