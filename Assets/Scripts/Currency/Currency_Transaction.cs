using UnityEngine;

public class Currency_Transaction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Currency_Manager currencyManager;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (currencyManager == null)
        {
            currencyManager = Currency_Manager.Instance;
        }

        if (currencyManager == null)
        {
            Debug.LogError(
                $"{nameof(Currency_Transaction)}: Currency Manager is not assigned or found.",
                this
            );
        }
    }

    public bool AddReward(int amount)
    {
        if (!IsReady())
            return false;

        return currencyManager.Add(amount);
    }

    public bool Spend(int amount)
    {
        if (!IsReady())
            return false;

        return currencyManager.Spend(amount);
    }

    public bool CanAfford(int amount)
    {
        if (!IsReady())
            return false;

        return currencyManager.CanAfford(amount);
    }

    public int GetCurrentAmount()
    {
        if (!IsReady())
            return 0;

        return currencyManager.CurrentAmount;
    }

    private bool IsReady()
    {
        return currencyManager != null;
    }
}