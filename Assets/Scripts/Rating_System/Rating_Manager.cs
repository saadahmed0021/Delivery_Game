using UnityEngine;

public class Rating_Manager : MonoBehaviour
{
    [SerializeField] private Rating_Data ratingData;

    private Rating_Calculator ratingCalculator;

    private float totalRatingScore;
    private int completedDeliveries;
    private float currentRating;
    private float currentLateDeliverySeconds;
    private int damageCollisionCount;
    private bool orderInProgress;
    private bool currentOrderFinalized;

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

    public float CurrentRating => currentRating;
    public int DamageCollisionCount => damageCollisionCount;

    public Rating_Calculator Calculator => ratingCalculator;

    private void Awake()
    {
        Initialize();

        Order_Damage orderDamage = FindFirstObjectByType<Order_Damage>();

        if (orderDamage != null)
            orderDamage.DamageRegistered += RegisterDamage;
    }

    private void OnDestroy()
    {
        Order_Damage orderDamage = FindFirstObjectByType<Order_Damage>();

        if (orderDamage != null)
            orderDamage.DamageRegistered -= RegisterDamage;
    }

    private void Initialize()
    {
        if (ratingData == null)
        {
            return;
        }

        ratingCalculator = new Rating_Calculator(ratingData);
        currentRating = ratingData.BaseRating;
    }

    public void StartOrder()
    {
        currentRating = ratingData != null
            ? ratingData.BaseRating
            : 5f;
        orderInProgress = true;
        currentOrderFinalized = false;
        currentLateDeliverySeconds = 0f;
        damageCollisionCount = 0;
    }

    public void UpdateLiveRating(
        float lateDeliverySeconds,
        float damage)
    {
        if (!orderInProgress || currentOrderFinalized)
            return;

        if (ratingCalculator == null)
            return;

        currentLateDeliverySeconds = Mathf.Max(0f, lateDeliverySeconds);
        currentRating = ratingCalculator.Calculate(
            currentLateDeliverySeconds,
            damageCollisionCount
        ).Score;
    }

    public Rating_Result CompleteDelivery(
        float lateDeliverySeconds,
        float damage)
    {
        if (ratingCalculator == null)
        {
            return new Rating_Result(0f, 1);
        }

        Rating_Result result = ratingCalculator.Calculate(
            lateDeliverySeconds,
            damageCollisionCount
        );

        currentRating = result.Score;
        AddDeliveryRating(result);
        orderInProgress = false;
        currentOrderFinalized = true;

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
        currentRating = ratingData != null
            ? ratingData.BaseRating
            : 5f;
        orderInProgress = false;
        currentOrderFinalized = false;
        currentLateDeliverySeconds = 0f;
        damageCollisionCount = 0;
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
        totalRatingScore += result.Score;
        completedDeliveries++;
    }

    private void RegisterDamage()
    {
        if (!orderInProgress || currentOrderFinalized)
            return;

        damageCollisionCount++;

        if (ratingCalculator != null)
        {
            currentRating = ratingCalculator.Calculate(
                currentLateDeliverySeconds,
                damageCollisionCount
            ).Score;
        }
    }
}