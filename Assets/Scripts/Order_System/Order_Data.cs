using UnityEngine;

[CreateAssetMenu(fileName = "Order_Data", menuName = "Order/Order Data")]
public class Order_Data : ScriptableObject
{
    [Header("Order")]
    [SerializeField] private string orderName = "Food Delivery";
    [SerializeField] private string restaurantName = "Restaurant";
    [SerializeField] private string customerName = "Customer";

    [Header("Reward")]
    [Min(0)]
    [SerializeField] private int baseReward = 100;

    [Header("Locations")]
    [SerializeField] private Order_Location restaurantLocation;
    [SerializeField] private Order_Location customerLocation;

    public string OrderName => orderName;
    public string RestaurantName => restaurantName;
    public string CustomerName => customerName;

    public int BaseReward => baseReward;

    public Order_Location RestaurantLocation => restaurantLocation;
    public Order_Location CustomerLocation => customerLocation;
}