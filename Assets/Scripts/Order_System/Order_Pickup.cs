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

    [Header("Pickup Arrow")]
    [SerializeField] private GameObject pickupArrowPrefab;
    [SerializeField] private float arrowHeight = 2f;

    private GameObject spawnedArrow;

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

    public void SpawnArrow()
    {
        if (pickupArrowPrefab == null || spawnedArrow != null)
            return;

        Vector3 spawnPosition = PickupPoint.position;
        spawnPosition.y += arrowHeight;

        spawnedArrow = Instantiate(
            pickupArrowPrefab,
            spawnPosition,
            pickupArrowPrefab.transform.rotation
        );
    }

    public void DestroyArrow()
    {
        if (spawnedArrow == null)
            return;

        Destroy(spawnedArrow);
        spawnedArrow = null;
    }

    public bool TryPickup()
    {
        if (!HasValidOrder())
            return false;

        if (!IsPlayerInRange())
            return false;

        bool pickedUp = orderManager.PickupOrder();

        if (pickedUp)
            DestroyArrow();

        return pickedUp;
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