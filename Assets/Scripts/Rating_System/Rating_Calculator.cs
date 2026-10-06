using UnityEngine;

public class Rating_Calculator
{
    private readonly Rating_Data data;
    public Rating_Calculator(Rating_Data ratingData)
    {
        data = ratingData;
    }

    public Rating_Result Calculate(
        float lateDeliverySeconds,
        int damageCollisionCount)
    {
        if (data == null)
        {
            return new Rating_Result(0f, 1);
        }

        lateDeliverySeconds = Mathf.Max(0f, lateDeliverySeconds);
        damageCollisionCount = Mathf.Max(0, damageCollisionCount);

        float timePenalty = CalculateTimePenalty(lateDeliverySeconds);
        float damagePenalty =
            damageCollisionCount * data.RatingPenaltyPerDamageCollision;
        float finalScore = data.BaseRating - timePenalty - damagePenalty;

        finalScore = Mathf.Clamp(
            finalScore,
            data.MinimumScore,
            data.MaximumScore
        );

        int stars = CalculateStars(finalScore, damageCollisionCount);

        return new Rating_Result(finalScore, stars);
    }

    public Rating_Result Calculate(
        float lateDeliverySeconds,
        float accumulatedDamagePercent)
    {
        if (data == null)
        {
            return new Rating_Result(0f, 1);
        }

        lateDeliverySeconds = Mathf.Max(0f, lateDeliverySeconds);
        accumulatedDamagePercent = Mathf.Clamp01(
            Mathf.Max(0f, accumulatedDamagePercent)
        );

        float timePenalty = CalculateTimePenalty(lateDeliverySeconds);
        float damagePenalty =
            accumulatedDamagePercent * data.RatingPenaltyPerDamagePercent;
        float finalScore = data.BaseRating - timePenalty - damagePenalty;

        finalScore = Mathf.Clamp(
            finalScore,
            data.MinimumScore,
            data.MaximumScore
        );

        int stars = CalculateStars(
            finalScore,
            accumulatedDamagePercent > 0f
        );

        return new Rating_Result(finalScore, stars);
    }

    private float CalculateTimePenalty(float lateDeliverySeconds)
    {
        if (lateDeliverySeconds < 5f)
            return 0f;

        if (lateDeliverySeconds < 10f)
            return data.RatingPenaltyAt5SecondsLate;

        if (lateDeliverySeconds < 20f)
            return data.RatingPenaltyAt10SecondsLate;

        return data.RatingPenaltyAt20SecondsLate;
    }

    private int CalculateStars(float score, int damageCollisionCount)
    {
        if (damageCollisionCount > 0 && score <= data.MinimumScore)
            return 0;

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