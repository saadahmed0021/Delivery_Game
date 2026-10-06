using UnityEngine;

public class Order_Delivery : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Order_Manager orderManager;
    [SerializeField] private Transform player;
    [SerializeField] private Order_Result_UI resultUI;

    [Header("Delivery Point")]
    [SerializeField] private Transform deliveryPoint;

    [Header("Interaction")]
    [SerializeField, Min(0.1f)] private float deliveryRadius = 2f;

    [Header("Delivery Arrow")]
    [SerializeField] private GameObject deliveryArrowPrefab;
    [SerializeField] private float arrowHeight = 2f;

    private GameObject spawnedArrow;

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

        if (resultUI == null)
            resultUI = FindFirstObjectByType<Order_Result_UI>(
                FindObjectsInactive.Include
            );
    }

    public void SpawnArrow()
    {
        if (deliveryArrowPrefab == null || spawnedArrow != null)
            return;

        Vector3 spawnPosition = DeliveryPoint.position;
        spawnPosition.y += arrowHeight;

        spawnedArrow = Instantiate(
            deliveryArrowPrefab,
            spawnPosition,
            deliveryArrowPrefab.transform.rotation
        );
    }

    public void DestroyArrow()
    {
        if (spawnedArrow == null)
            return;

        Destroy(spawnedArrow);
        spawnedArrow = null;
    }

    public bool TryDeliver()
    {
        if (!HasValidOrder())
            return false;

        if (!IsPlayerInRange())
            return false;

        if (!orderManager.DeliverOrder())
            return false;

        if (resultUI != null)
            resultUI.Show(orderManager.ActiveOrder);
        else
            orderManager.CompleteOrder();

        return true;
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