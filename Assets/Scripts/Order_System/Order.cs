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

    public string OrderName =>
        data != null ? data.OrderName : string.Empty;

    public string RestaurantName =>
        data != null ? data.RestaurantName : string.Empty;

    public string CustomerName =>
        data != null ? data.CustomerName : string.Empty;

    public int Reward =>
        data != null ? data.BaseReward : 0;

    public string RestaurantLocationID =>
        data != null ? data.RestaurantLocationID : string.Empty;

    public string CustomerLocationID =>
        data != null ? data.CustomerLocationID : string.Empty;

    public Order_Location RestaurantLocation =>
        GetLocation(RestaurantLocationID);

    public Order_Location CustomerLocation =>
        GetLocation(CustomerLocationID);

    public bool IsAvailable =>
        currentState == OrderState.Available;

    public bool IsAccepted =>
        currentState == OrderState.Accepted;

    public bool IsPickedUp =>
        currentState == OrderState.PickedUp;

    public bool IsDelivered =>
        currentState == OrderState.Delivered;

    public bool IsCompleted =>
        currentState == OrderState.Completed;

    public bool IsCancelled =>
        currentState == OrderState.Cancelled;

    public event Action<OrderState> OnStateChanged;

    public void Initialize(Order_Data orderData)
    {
        if (orderData == null)
        {
            Debug.LogError(
                "Order cannot be initialized with null Order_Data.",
                this
            );

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

        if (!HasValidLocations())
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

    public bool HasValidLocations()
    {
        if (data == null)
            return false;

        if (string.IsNullOrWhiteSpace(RestaurantLocationID))
        {
            Debug.LogError(
                $"Order '{OrderName}' has no restaurant Location ID.",
                this
            );

            return false;
        }

        if (string.IsNullOrWhiteSpace(CustomerLocationID))
        {
            Debug.LogError(
                $"Order '{OrderName}' has no customer Location ID.",
                this
            );

            return false;
        }

        if (Order_Location_Manager.Instance == null)
        {
            Debug.LogError(
                "Order_Location_Manager instance was not found.",
                this
            );

            return false;
        }

        if (!Order_Location_Manager.Instance.HasLocation(
                RestaurantLocationID))
        {
            Debug.LogError(
                $"Restaurant Location ID '{RestaurantLocationID}' " +
                $"was not found for order '{OrderName}'.",
                this
            );

            return false;
        }

        if (!Order_Location_Manager.Instance.HasLocation(
                CustomerLocationID))
        {
            Debug.LogError(
                $"Customer Location ID '{CustomerLocationID}' " +
                $"was not found for order '{OrderName}'.",
                this
            );

            return false;
        }

        return true;
    }

    private Order_Location GetLocation(string locationID)
    {
        if (string.IsNullOrWhiteSpace(locationID))
            return null;

        if (Order_Location_Manager.Instance == null)
            return null;

        return Order_Location_Manager.Instance.GetLocation(locationID);
    }

    private void SetState(OrderState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;

        OnStateChanged?.Invoke(currentState);
    }
}