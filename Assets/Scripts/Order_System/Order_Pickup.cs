using UnityEngine;

public class Order_Pickup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Order_Manager orderManager;
    [SerializeField] private Transform player;

    [Header("Pickup Point")]
    [SerializeField] private Transform pickupPoint;

    [Header("Interaction")]
    [SerializeField, Min(0.1f)] private float pickupRadius = 2f;

    public Transform PickupPoint => pickupPoint != null ? pickupPoint : transform;

    public bool CanPickup =>
        HasValidOrder() && IsPlayerInRange();

    private void Reset()
    {
        pickupPoint = transform;
    }

    private void Awake()
    {
        if (pickupPoint == null)
            pickupPoint = transform;
    }

    public bool TryPickup()
    {
        if (!HasValidOrder())
            return false;

        if (!IsPlayerInRange())
            return false;

        return orderManager.PickupOrder();
    }

    private bool HasValidOrder()
    {
        if (orderManager == null)
            return false;

        Order activeOrder = orderManager.ActiveOrder;

        if (activeOrder == null)
            return false;

        return activeOrder.IsAccepted &&
               activeOrder.RestaurantLocation != null;
    }

    private bool IsPlayerInRange()
    {
        if (player == null)
            return false;

        float distance = Vector3.Distance(
            player.position,
            PickupPoint.position
        );

        return distance <= pickupRadius;
    }

    private void OnDrawGizmosSelected()
    {
        Transform point = pickupPoint != null ? pickupPoint : transform;

        Gizmos.DrawWireSphere(point.position, pickupRadius);
    }
}