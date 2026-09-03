using UnityEngine;

public class Order_Delivery : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Order_Manager orderManager;
    [SerializeField] private Transform player;

    [Header("Delivery Point")]
    [SerializeField] private Transform deliveryPoint;

    [Header("Interaction")]
    [SerializeField, Min(0.1f)] private float deliveryRadius = 2f;

    public Transform DeliveryPoint =>
        deliveryPoint != null ? deliveryPoint : transform;

    public bool CanDeliver =>
        HasValidOrder() && IsPlayerInRange();

    private void Reset()
    {
        deliveryPoint = transform;
    }

    private void Awake()
    {
        if (deliveryPoint == null)
            deliveryPoint = transform;
    }

    public bool TryDeliver()
    {
        if (!HasValidOrder())
            return false;

        if (!IsPlayerInRange())
            return false;

        return orderManager.DeliverOrder();
    }

    private bool HasValidOrder()
    {
        if (orderManager == null)
            return false;

        Order activeOrder = orderManager.ActiveOrder;

        if (activeOrder == null)
            return false;

        return activeOrder.IsPickedUp &&
               activeOrder.CustomerLocation != null;
    }

    private bool IsPlayerInRange()
    {
        if (player == null)
            return false;

        float distance = Vector3.Distance(
            player.position,
            DeliveryPoint.position
        );

        return distance <= deliveryRadius;
    }

    private void OnDrawGizmosSelected()
    {
        Transform point = deliveryPoint != null
            ? deliveryPoint
            : transform;

        Gizmos.DrawWireSphere(
            point.position,
            deliveryRadius
        );
    }
}