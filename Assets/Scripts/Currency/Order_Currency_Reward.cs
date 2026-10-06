using UnityEngine;

public class Order_Currency_Reward : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Currency_Transaction currencyTransaction;

    [Header("Reward")]
    [SerializeField, Min(0)] private int rewardAmount = 100;

    [Header("Payment Penalties")]
    [Range(0f, 1f)]
    [SerializeField] private float damagePenaltyPerDamagePercent = 0.25f;
    [Range(0f, 1f)]
    [SerializeField] private float latenessPenaltyPercent = 0.2f;
    [SerializeField, Min(0f)] private float latenessPenaltyStartSeconds = 30f;
    [SerializeField, Min(0f)] private float latenessPenaltyEndSeconds = 120f;

    private bool rewardGiven;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (currencyTransaction == null)
        {
            currencyTransaction = GetComponent<Currency_Transaction>();
        }

        if (currencyTransaction == null)
        {
        }
    }

    public bool GiveReward()
    {
        return GiveReward(rewardAmount);
    }

    public bool GiveReward(int amount)
    {
        if (rewardGiven)
            return false;

        if (amount <= 0)
            return false;

        if (currencyTransaction == null)
            return false;

        bool success = currencyTransaction.AddReward(amount);

        if (!success)
            return false;

        rewardGiven = true;

        return true;
    }

    public int GetRewardAmount()
    {
        return rewardAmount;
    }

    public int CalculateFinalReward(
        int baseReward,
        float accumulatedDamagePercent,
        float lateDeliverySeconds)
    {
        float damagePercent = Mathf.Clamp01(
            Mathf.Max(0f, accumulatedDamagePercent)
        );
        float damagePenalty =
            baseReward * damagePenaltyPerDamagePercent * damagePercent;
        float latenessPenalty = CalculateLatenessPenalty(
            baseReward,
            lateDeliverySeconds
        );
        float finalReward = baseReward - damagePenalty - latenessPenalty;

        return Mathf.Max(0, Mathf.RoundToInt(finalReward));
    }

    public bool HasRewardBeenGiven()
    {
        return rewardGiven;
    }

    public void ResetReward()
    {
        rewardGiven = false;
    }
}