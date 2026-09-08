using UnityEngine;

[CreateAssetMenu(
    fileName = "Rating_Data",
    menuName = "Delivery Game/Rating Data"
)]
public class Rating_Data : ScriptableObject
{
    [Header("Performance Weights")]
    [Tooltip("How much delivery time affects the final score.")]
    [Range(0f, 1f)]
    [SerializeField] private float deliveryTimeWeight = 0.40f;

    [Tooltip("How much package condition affects the final score.")]
    [Range(0f, 1f)]
    [SerializeField] private float packageConditionWeight = 0.25f;

    [Tooltip("How much driving performance affects the final score.")]
    [Range(0f, 1f)]
    [SerializeField] private float drivingPerformanceWeight = 0.20f;

    [Tooltip("How much successful completion affects the final score.")]
    [Range(0f, 1f)]
    [SerializeField] private float completionWeight = 0.15f;


    [Header("Star Rating")]
    [Tooltip("Minimum score required for 5 stars.")]
    [Range(0f, 100f)]
    [SerializeField] private float fiveStarThreshold = 90f;

    [Tooltip("Minimum score required for 4 stars.")]
    [Range(0f, 100f)]
    [SerializeField] private float fourStarThreshold = 75f;

    [Tooltip("Minimum score required for 3 stars.")]
    [Range(0f, 100f)]
    [SerializeField] private float threeStarThreshold = 60f;

    [Tooltip("Minimum score required for 2 stars.")]
    [Range(0f, 100f)]
    [SerializeField] private float twoStarThreshold = 40f;


    [Header("Score Limits")]
    [Min(0f)]
    [SerializeField] private float minimumScore = 0f;

    [Min(0f)]
    [SerializeField] private float maximumScore = 100f;



    public float DeliveryTimeWeight => deliveryTimeWeight;
    public float PackageConditionWeight => packageConditionWeight;
    public float DrivingPerformanceWeight => drivingPerformanceWeight;
    public float CompletionWeight => completionWeight;



    public float FiveStarThreshold => fiveStarThreshold;
    public float FourStarThreshold => fourStarThreshold;
    public float ThreeStarThreshold => threeStarThreshold;
    public float TwoStarThreshold => twoStarThreshold;



    public float MinimumScore => minimumScore;
    public float MaximumScore => maximumScore;


#if UNITY_EDITOR

    private void OnValidate()
    {
        ValidateWeights();
        ValidateThresholds();
        ValidateScoreLimits();
    }

    private void ValidateWeights()
    {
        float totalWeight =
            deliveryTimeWeight +
            packageConditionWeight +
            drivingPerformanceWeight +
            completionWeight;

        if (!Mathf.Approximately(totalWeight, 1f))
        {
            Debug.LogWarning(
                $"{name}: Rating weights should add up to 1.0. " +
                $"Current total: {totalWeight:F2}",
                this
            );
        }
    }

    private void ValidateThresholds()
    {
        fiveStarThreshold = Mathf.Clamp(
            fiveStarThreshold,
            fourStarThreshold,
            maximumScore
        );

        fourStarThreshold = Mathf.Clamp(
            fourStarThreshold,
            threeStarThreshold,
            fiveStarThreshold
        );

        threeStarThreshold = Mathf.Clamp(
            threeStarThreshold,
            twoStarThreshold,
            fourStarThreshold
        );

        twoStarThreshold = Mathf.Clamp(
            twoStarThreshold,
            minimumScore,
            threeStarThreshold
        );
    }

    private void ValidateScoreLimits()
    {
        minimumScore = Mathf.Max(0f, minimumScore);
        maximumScore = Mathf.Max(minimumScore, maximumScore);
    }

#endif
}