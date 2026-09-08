using UnityEngine;

public class Rating_Calculator
{
    private readonly Rating_Data data;
    public Rating_Calculator(Rating_Data ratingData)
    {
        data = ratingData;
    }

    public Rating_Result Calculate(
        float deliveryTimeScore,
        float packageConditionScore,
        float drivingPerformanceScore,
        float completionScore)
    {
        if (data == null)
        {
            Debug.LogError("Rating Calculator: Rating_Data is not assigned.");

            return new Rating_Result(0f, 1);
        }


        deliveryTimeScore = ClampScore(deliveryTimeScore);
        packageConditionScore = ClampScore(packageConditionScore);
        drivingPerformanceScore = ClampScore(drivingPerformanceScore);
        completionScore = ClampScore(completionScore);

        float finalScore =
            deliveryTimeScore * data.DeliveryTimeWeight +
            packageConditionScore * data.PackageConditionWeight +
            drivingPerformanceScore * data.DrivingPerformanceWeight +
            completionScore * data.CompletionWeight;

        finalScore = Mathf.Clamp(
            finalScore,
            data.MinimumScore,
            data.MaximumScore
        );

        int stars = CalculateStars(finalScore);

        return new Rating_Result(finalScore, stars);
    }

    private int CalculateStars(float score)
    {
        if (score >= data.FiveStarThreshold)
            return 5;

        if (score >= data.FourStarThreshold)
            return 4;

        if (score >= data.ThreeStarThreshold)
            return 3;

        if (score >= data.TwoStarThreshold)
            return 2;

        return 1;
    }

    private float ClampScore(float score)
    {
        return Mathf.Clamp(
            score,
            data.MinimumScore,
            data.MaximumScore
        );
    }
}

public readonly struct Rating_Result
{
    public float Score { get; }
    public int Stars { get; }


    public Rating_Result(float score, int stars)
    {
        Score = score;
        Stars = stars;
    }
}