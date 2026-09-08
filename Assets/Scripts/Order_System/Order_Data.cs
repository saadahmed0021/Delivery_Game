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

    [Header("Location IDs")]
    [SerializeField] private string restaurantLocationID;
    [SerializeField] private string customerLocationID;

    public string OrderName => orderName;
    public string RestaurantName => restaurantName;
    public string CustomerName => customerName;

    public int BaseReward => baseReward;

    public string RestaurantLocationID => restaurantLocationID;
    public string CustomerLocationID => customerLocationID;
}
