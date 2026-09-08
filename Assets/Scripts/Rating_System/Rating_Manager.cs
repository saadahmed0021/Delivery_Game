using UnityEngine;

public class Rating_Manager : MonoBehaviour
{
    [SerializeField] private Rating_Data ratingData;

    private Rating_Calculator ratingCalculator;

    private float totalRatingScore;
    private int completedDeliveries;

    public float OverallRating
    {
        get
        {
            if (completedDeliveries <= 0)
                return 0f;

            return totalRatingScore / completedDeliveries;
        }
    }

    public int CompletedDeliveries => completedDeliveries;

    public float TotalRatingScore => totalRatingScore;

    public Rating_Calculator Calculator => ratingCalculator;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (ratingData == null)
        {
            Debug.LogError(
                "Rating Manager: Rating_Data is not assigned.",
                this
            );

            return;
        }

        ratingCalculator = new Rating_Calculator(ratingData);
    }

    public Rating_Result CompleteDelivery(
        float deliveryTimeScore,
        float packageConditionScore,
        float drivingPerformanceScore,
        float completionScore)
    {
        if (ratingCalculator == null)
        {
            Debug.LogError(
                "Rating Manager: Rating system is not initialized.",
                this
            );

            return new Rating_Result(0f, 1);
        }

        Rating_Result result = ratingCalculator.Calculate(
            deliveryTimeScore,
            packageConditionScore,
            drivingPerformanceScore,
            completionScore
        );

        AddDeliveryRating(result);

        return result;
    }

    public void AddRating(Rating_Result result)
    {
        AddDeliveryRating(result);
    }

    public void LoadRating(
        float savedTotalRatingScore,
        int savedCompletedDeliveries)
    {
        totalRatingScore = Mathf.Max(0f, savedTotalRatingScore);
        completedDeliveries = Mathf.Max(0, savedCompletedDeliveries);
    }

    public void ResetRating()
    {
        totalRatingScore = 0f;
        completedDeliveries = 0;
    }

    public float GetOverallRating()
    {
        return Mathf.Round(OverallRating * 10f) / 10f;
    }

    public float GetOverallRatingPercentage()
    {
        if (completedDeliveries <= 0)
            return 0f;

        return (OverallRating / 5f) * 100f;
    }

    private void AddDeliveryRating(Rating_Result result)
    {
        totalRatingScore += result.Stars;
        completedDeliveries++;
    }
}