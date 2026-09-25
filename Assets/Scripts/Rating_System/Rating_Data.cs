using UnityEngine;

[CreateAssetMenu(
    fileName = "Rating_Data",
    menuName = "Delivery Game/Rating Data"
)]
public class Rating_Data : ScriptableObject
{
    [Header("Penalty Settings")]
    [Min(0f)]
    [SerializeField] private float baseRating = 5f;

    [Min(0f)]
    [Tooltip("Rating lost at 5-9 seconds late.")]
    [SerializeField] private float ratingPenaltyAt5SecondsLate = 0.2f;

    [Tooltip("Rating lost at 10-19 seconds late.")]
    [Min(0f)]
    [SerializeField] private float ratingPenaltyAt10SecondsLate = 0.5f;

    [Tooltip("Rating lost at 20 or more seconds late.")]
    [Min(0f)]
    [SerializeField] private float ratingPenaltyAt20SecondsLate = 0.75f;

    [Tooltip("Rating lost for each registered damage collision.")]
    [Min(0f)]
    [SerializeField] private float ratingPenaltyPerDamageCollision = 0.4f;


    [Header("Star Rating")]
    [Tooltip("Minimum score required for 5 stars.")]
    [Range(0f, 5f)]
    [SerializeField] private float fiveStarThreshold = 5f;

    [Tooltip("Minimum score required for 4 stars.")]
    [Range(0f, 5f)]
    [SerializeField] private float fourStarThreshold = 4f;

    [Tooltip("Minimum score required for 3 stars.")]
    [Range(0f, 5f)]
    [SerializeField] private float threeStarThreshold = 3f;

    [Tooltip("Minimum score required for 2 stars.")]
    [Range(0f, 5f)]
    [SerializeField] private float twoStarThreshold = 2f;


    [Header("Score Limits")]
    [Min(0f)]
    [SerializeField] private float minimumScore = 0f;

    [Min(0f)]
    [SerializeField] private float maximumScore = 5f;



    public float BaseRating => baseRating;
    public float RatingPenaltyAt5SecondsLate => ratingPenaltyAt5SecondsLate;
    public float RatingPenaltyAt10SecondsLate => ratingPenaltyAt10SecondsLate;
    public float RatingPenaltyAt20SecondsLate => ratingPenaltyAt20SecondsLate;
    public float RatingPenaltyPerDamageCollision => ratingPenaltyPerDamageCollision;



    public float FiveStarThreshold => fiveStarThreshold;
    public float FourStarThreshold => fourStarThreshold;
    public float ThreeStarThreshold => threeStarThreshold;
    public float TwoStarThreshold => twoStarThreshold;



    public float MinimumScore => minimumScore;
    public float MaximumScore => maximumScore;


#if UNITY_EDITOR

    private void OnValidate()
    {
        ValidateThresholds();
        ValidateScoreLimits();
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
        ratingPenaltyAt5SecondsLate = Mathf.Max(0f, ratingPenaltyAt5SecondsLate);
        ratingPenaltyAt10SecondsLate = Mathf.Max(0f, ratingPenaltyAt10SecondsLate);
        ratingPenaltyAt20SecondsLate = Mathf.Max(0f, ratingPenaltyAt20SecondsLate);
        ratingPenaltyPerDamageCollision = Mathf.Max(
            0f,
            ratingPenaltyPerDamageCollision
        );
    }

#endif
}