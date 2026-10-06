using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Order_Travel_Time : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private TMP_Text activeTimerText;
    [SerializeField] private Order_Manager orderManager;
    [SerializeField] private Order_Result_UI resultUI;
    [SerializeField] private Rating_Manager ratingManager;
    [SerializeField] private AudioSource lowTimeAudioSource;

    [Header("Travel Settings")]
    [SerializeField, Min(0.1f)] private float averageSpeed = 10f;
    [Tooltip("Extra time added to both pickup and delivery.")]
    [SerializeField, Min(0f)] private float extraDeliveryTime = 10f;
    [SerializeField] private int areaMask = NavMesh.AllAreas;
    [SerializeField, Min(0.1f)] private float sampleDistance = 5f;

    [Header("Low Time Warning")]
    [SerializeField, Min(0f)] private float lowTimeWarningThreshold = 10f;
    [SerializeField, Min(0.05f)] private float lowTimeBlinkInterval = 0.5f;
    [SerializeField] private Color lowTimeWarningColor = Color.red;

    private NavMeshPath navMeshPath;

    private float pickupDistance;
    private float pickupTime;

    private float deliveryDistance;
    private float deliveryTime;

    private float activeTimeRemaining;
    private float deliveryElapsedTime;
    private bool deliveryPhase;
    private bool countdownRunning;
    private Color originalTimerColor;
    private bool hasOriginalTimerColor;
    private bool lowTimeWarningActive;
    private bool lowTimeTextVisible;
    private bool lowTimeAudioStarted;
    private float lowTimeBlinkTimer;

    public bool IsCountdownRunning => countdownRunning;
    public float ActiveTimeRemaining => activeTimeRemaining;
    public float DeliveryElapsedTime => deliveryElapsedTime;
    public float LateDeliverySeconds => Mathf.Max(
        0f,
        deliveryElapsedTime - deliveryTime
    );
    public string ActiveTimeFormatted => FormatTime(activeTimeRemaining);

    private void Awake()
    {
        navMeshPath = new NavMeshPath();

        if (activeTimerText != null)
        {
            originalTimerColor = activeTimerText.color;
            hasOriginalTimerColor = true;
        }

        if (player == null)
            player = transform;

        if (orderManager == null)
            orderManager = FindFirstObjectByType<Order_Manager>();

        if (ratingManager == null)
            ratingManager = FindFirstObjectByType<Rating_Manager>();

        if (resultUI == null)
        {
            resultUI = FindFirstObjectByType<Order_Result_UI>(
                FindObjectsInactive.Include
            );
        }

        StopCountdown();
    }

    private void Update()
    {
        if (!countdownRunning)
            return;

        activeTimeRemaining = Mathf.Max(
            0f,
            activeTimeRemaining - Time.deltaTime
        );

        if (deliveryPhase)
            deliveryElapsedTime += Time.deltaTime;

        ratingManager?.UpdateLiveRating(
            LateDeliverySeconds,
            orderManager != null
                ? orderManager.AccumulatedDeliveryDamage
                : 0f
        );

        UpdateActiveTimerText();
        UpdateLowTimeWarning();

        if (activeTimeRemaining <= 0f && !deliveryPhase)
            HandleTimeout();
    }

    public Transform Player => player;
    public float AverageSpeed => averageSpeed;
    public float ExtraDeliveryTime => extraDeliveryTime;

    public float PickupDistance => pickupDistance;
    public float PickupTime => pickupTime;
    public string PickupTimeFormatted => FormatTime(pickupTime);
    public string PickupTimeDisplayFormatted => FormatDuration(pickupTime);

    public float DeliveryDistance => deliveryDistance;
    public float DeliveryTime => deliveryTime;
    public string DeliveryTimeFormatted => FormatTime(deliveryTime);

    public bool HasPickupRoute => pickupDistance > 0f;
    public bool HasDeliveryRoute => deliveryDistance > 0f;

    public void SetPlayer(Transform target)
    {
        player = target;
    }

    public void SetAverageSpeed(float speed)
    {
        averageSpeed = Mathf.Max(0.1f, speed);
    }

    public bool StartPickupCountdown(Order order)
    {
        ResetLowTimeWarning();

        if (order == null || order.RestaurantLocation == null)
        {
            activeTimeRemaining = 0f;
            deliveryPhase = false;
            countdownRunning = true;
            ShowActiveTimer();
            UpdateActiveTimerText();
            return false;
        }

        bool routeFound = CalculatePickupTime(
            order.RestaurantLocation.Transform
        );

        if (routeFound)
        {
            Debug.Log(
                $"Selected restaurant '{order.RestaurantName}' pickup route distance: {pickupDistance / 1000f:F3} km.",
                this
            );
        }

        activeTimeRemaining = Mathf.Max(0f, pickupTime);
        deliveryPhase = false;
        countdownRunning = true;
        ShowActiveTimer();
        UpdateActiveTimerText();

        return routeFound;
    }

    public bool StartDeliveryCountdown(Order order)
    {
        ResetLowTimeWarning();

        if (order == null ||
            order.RestaurantLocation == null ||
            order.CustomerLocation == null)
        {
            activeTimeRemaining = Mathf.Max(0f, activeTimeRemaining);
            countdownRunning = true;
            ShowActiveTimer();
            UpdateActiveTimerText();
            return false;
        }

        bool routeFound = CalculateDeliveryTime(
            order.RestaurantLocation.Transform,
            order.CustomerLocation.Transform
        );

        if (routeFound)
        {
            Debug.Log(
                $"Delivery route distance ({order.RestaurantName} to {order.CustomerName}): {deliveryDistance / 1000f:F3} km.",
                this
            );
        }

        activeTimeRemaining = Mathf.Max(
            0f,
            deliveryTime
        );
        deliveryElapsedTime = 0f;
        deliveryPhase = true;
        ratingManager?.UpdateLiveRating(
            0f,
            orderManager != null
                ? orderManager.AccumulatedDeliveryDamage
                : 0f
        );

        countdownRunning = true;
        ShowActiveTimer();
        UpdateActiveTimerText();

        return routeFound;
    }

    public void StopCountdown()
    {
        countdownRunning = false;
        deliveryPhase = false;
        activeTimeRemaining = 0f;
        ResetLowTimeWarning();

        if (activeTimerText != null)
            activeTimerText.gameObject.SetActive(false);
    }

    public bool CalculatePickupTime(Transform restaurant)
    {
        if (restaurant == null)
        {
            ClearPickupData();
            return false;
        }

        Vector3 startPosition = player != null
            ? player.position
            : transform.position;

        if (!TryCalculateRoute(
                startPosition,
                restaurant.position,
                out float distance))
        {
            ClearPickupData();
            return false;
        }

        pickupDistance = distance;
        pickupTime = CalculateTime(distance) + extraDeliveryTime;

        return true;
    }

    public bool CalculateDeliveryTime(
        Transform restaurant,
        Transform customer)
    {
        if (restaurant == null || customer == null)
        {
            ClearDeliveryData();
            return false;
        }

        if (!TryCalculateRoute(
                restaurant.position,
                customer.position,
                out float distance))
        {
            ClearDeliveryData();
            return false;
        }

        deliveryDistance = distance;
        deliveryTime = CalculateTime(distance) + extraDeliveryTime;

        return true;
    }

    public bool TryCalculateRoute(
        Vector3 startPosition,
        Vector3 destinationPosition,
        out float distance)
    {
        distance = 0f;

        if (!TryGetNavMeshPosition(
                startPosition,
                out Vector3 start))
        {
            return false;
        }

        if (!TryGetNavMeshPosition(
                destinationPosition,
                out Vector3 destination))
        {
            return false;
        }

        navMeshPath.ClearCorners();

        if (!NavMesh.CalculatePath(
                start,
                destination,
                areaMask,
                navMeshPath))
        {
            return false;
        }

        if (navMeshPath.status != NavMeshPathStatus.PathComplete)
        {
            return false;
        }

        Vector3[] corners = navMeshPath.corners;

        if (corners == null || corners.Length < 2)
        {
            return false;
        }

        for (int i = 1; i < corners.Length; i++)
        {
            distance += Vector3.Distance(
                corners[i - 1],
                corners[i]);
        }

        return distance > 0f;
    }

    public float CalculateTime(float distance)
    {
        if (distance <= 0f || averageSpeed <= 0f)
        {
            return 0f;
        }

        return distance / averageSpeed;
    }

    public string FormatTime(float seconds)
    {
        seconds = Mathf.Max(0f, seconds);

        int totalSeconds = Mathf.CeilToInt(seconds);
        int minutes = totalSeconds / 60;
        int remainingSeconds = totalSeconds % 60;

        return $"{minutes:00} : {remainingSeconds:00}";
    }

    public string GetPickupTimeFormatted()
    {
        return FormatTime(pickupTime);
    }

    public string GetPickupTimeDisplayFormatted()
    {
        return FormatDuration(pickupTime);
    }

    public string GetDeliveryTimeFormatted()
    {
        return FormatTime(deliveryTime);
    }

    public string FormatDuration(float seconds)
    {
        seconds = Mathf.Max(0f, seconds);

        int totalSeconds = Mathf.CeilToInt(seconds);
        int minutes = totalSeconds / 60;
        int remainingSeconds = totalSeconds % 60;

        return $"{minutes} min {remainingSeconds} sec";
    }

    public void ClearPickupData()
    {
        pickupDistance = 0f;
        pickupTime = 0f;
    }

    public void ClearDeliveryData()
    {
        deliveryDistance = 0f;
        deliveryTime = 0f;
    }

    public void ClearAll()
    {
        ClearPickupData();
        ClearDeliveryData();
    }

    private void HandleTimeout()
    {
        Order timedOutOrder = orderManager != null
            ? orderManager.ActiveOrder
            : null;

        StopCountdown();

        if (timedOutOrder == null || orderManager == null)
            return;

        if (orderManager.CancelActiveOrder() && resultUI != null)
            resultUI.ShowFailure(timedOutOrder);
    }

    private void ShowActiveTimer()
    {
        if (activeTimerText != null)
            activeTimerText.gameObject.SetActive(true);
    }

    private void UpdateActiveTimerText()
    {
        if (activeTimerText != null)
            activeTimerText.text = FormatTime(activeTimeRemaining);
    }

    private void UpdateLowTimeWarning()
    {
        if (!countdownRunning ||
            activeTimeRemaining >= lowTimeWarningThreshold)
        {
            if (lowTimeWarningActive)
                ResetLowTimeWarning();

            return;
        }

        if (!lowTimeWarningActive)
        {
            lowTimeWarningActive = true;
            lowTimeTextVisible = true;
            lowTimeBlinkTimer = 0f;

            if (lowTimeAudioSource != null &&
                lowTimeAudioSource.clip != null)
            {
                lowTimeAudioSource.Play();
                lowTimeAudioStarted = true;
            }
        }

        lowTimeBlinkTimer += Time.deltaTime;

        if (lowTimeBlinkTimer >= lowTimeBlinkInterval)
        {
            lowTimeBlinkTimer %= lowTimeBlinkInterval;
            lowTimeTextVisible = !lowTimeTextVisible;
        }

        if (activeTimerText != null)
        {
            activeTimerText.color = lowTimeTextVisible
                ? lowTimeWarningColor
                : new Color(
                    lowTimeWarningColor.r,
                    lowTimeWarningColor.g,
                    lowTimeWarningColor.b,
                    0f
                );
        }
    }

    private void ResetLowTimeWarning()
    {
        lowTimeWarningActive = false;
        lowTimeTextVisible = false;
        lowTimeBlinkTimer = 0f;

        if (activeTimerText != null && hasOriginalTimerColor)
            activeTimerText.color = originalTimerColor;

        if (lowTimeAudioStarted && lowTimeAudioSource != null)
            lowTimeAudioSource.Stop();

        lowTimeAudioStarted = false;
    }

    private bool TryGetNavMeshPosition(
        Vector3 position,
        out Vector3 result)
    {
        if (NavMesh.SamplePosition(
                position,
                out NavMeshHit hit,
                sampleDistance,
                areaMask))
        {
            result = hit.position;
            return true;
        }

        result = default;
        return false;
    }

    private void OnValidate()
    {
        averageSpeed = Mathf.Max(0.1f, averageSpeed);
        sampleDistance = Mathf.Max(0.1f, sampleDistance);
        lowTimeWarningThreshold = Mathf.Max(0f, lowTimeWarningThreshold);
        lowTimeBlinkInterval = Mathf.Max(0.05f, lowTimeBlinkInterval);
    }
}