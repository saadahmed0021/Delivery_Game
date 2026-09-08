using UnityEngine;

public class Order_Currency_Reward : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Currency_Transaction currencyTransaction;

    [Header("Reward")]
    [SerializeField, Min(0)] private int rewardAmount = 100;

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
            Debug.LogError(
                $"{nameof(Order_Currency_Reward)}: Currency Transaction is not assigned.",
                this
            );
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

    public bool HasRewardBeenGiven()
    {
        return rewardGiven;
    }

    public void ResetReward()
    {
        rewardGiven = false;
    }
}