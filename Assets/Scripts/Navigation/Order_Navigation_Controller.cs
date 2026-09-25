using UnityEngine;

public class Order_Navigation_Controller : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Order_Manager orderManager;
    [SerializeField] private Bike_NavMeshPathfinder pathfinder;
    [SerializeField] private Order_Result_UI resultUI;

    [Header("Arrival")]
    [SerializeField] private float restaurantArrivalDistance = 4f;
    [SerializeField] private float customerArrivalDistance = 4f;

    private Order trackedOrder;

    private void Awake()
    {
        if (orderManager == null)
            orderManager = FindFirstObjectByType<Order_Manager>();

        if (pathfinder == null)
            pathfinder = GetComponent<Bike_NavMeshPathfinder>();

        if (resultUI == null)
            resultUI = FindFirstObjectByType<Order_Result_UI>(
                FindObjectsInactive.Include
            );
    }

    private void Update()
    {
        if (orderManager == null || pathfinder == null)
            return;

        Order activeOrder = orderManager.ActiveOrder;

        if (activeOrder == null)
        {
            HandleNoActiveOrder();
            return;
        }

        TrackOrder(activeOrder);

        HandleOrderState(activeOrder);
    }

    private void TrackOrder(Order order)
    {
        if (trackedOrder == order)
            return;

        trackedOrder = order;

        if (order.State == Order.OrderState.Accepted)
            StartRestaurantNavigation(order);
    }

    private void HandleOrderState(Order order)
    {
        switch (order.State)
        {
            case Order.OrderState.Accepted:
                HandleRestaurantNavigation(order);
                break;

            case Order.OrderState.PickedUp:
                HandleCustomerNavigation(order);
                break;

            case Order.OrderState.Delivered:
                break;

            case Order.OrderState.Completed:
            case Order.OrderState.Cancelled:
                ClearNavigation();
                break;
        }
    }

    private void HandleRestaurantNavigation(Order order)
    {
        if (!pathfinder.IsGoingToRestaurant)
        {
            StartRestaurantNavigation(order);
            return;
        }

        if (!pathfinder.HasReachedRestaurant(
                restaurantArrivalDistance))
        {
            return;
        }

        PickupOrder(order);
    }

    private void StartRestaurantNavigation(Order order)
    {
        if (order == null)
            return;

        if (order.State != Order.OrderState.Accepted)
            return;

        Order_Location restaurant =
            order.RestaurantLocation;

        if (restaurant == null)
        {
            return;
        }

        bool pathFound =
            pathfinder.StartRestaurantRoute(
                restaurant.Position
            );

        if (!pathFound)
        {
            return;
        }
    }

    private void PickupOrder(Order order)
    {
        if (!orderManager.PickupOrder())
            return;

        StartCustomerNavigation(order);
    }

    private void StartCustomerNavigation(Order order)
    {
        if (order == null)
            return;

        if (order.State != Order.OrderState.PickedUp)
            return;

        Order_Location customer =
            order.CustomerLocation;

        if (customer == null)
        {
            return;
        }

        bool pathFound =
            pathfinder.StartCustomerRoute(
                customer.Position
            );

        if (!pathFound)
        {
            return;
        }
    }

    private void HandleCustomerNavigation(Order order)
    {
        if (!pathfinder.IsGoingToCustomer)
        {
            StartCustomerNavigation(order);
            return;
        }

        if (!pathfinder.HasReachedCustomer(
                customerArrivalDistance))
        {
            return;
        }

        DeliverOrder(order);
    }

    private void DeliverOrder(Order order)
    {
        if (!orderManager.DeliverOrder())
            return;

        if (resultUI != null)
            resultUI.Show(order);
        else
            CompleteOrder(order);
    }

    private void CompleteOrder(Order order)
    {
        if (!orderManager.CompleteOrder())
            return;

        trackedOrder = null;

        ClearNavigation();
    }

    private void HandleNoActiveOrder()
    {
        if (trackedOrder == null)
            return;

        trackedOrder = null;

        ClearNavigation();
    }

    private void ClearNavigation()
    {
        pathfinder.ClearRoute();
    }

    private void OnDisable()
    {
        if (pathfinder != null)
            pathfinder.ClearRoute();
    }
}