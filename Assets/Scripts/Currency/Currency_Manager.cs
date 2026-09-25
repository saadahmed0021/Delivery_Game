using UnityEngine;

public class Currency_Manager : MonoBehaviour
{
    public static Currency_Manager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Currency_Wallet wallet;

    public Currency_Wallet Wallet => wallet;

    public int CurrentAmount => wallet != null ? wallet.CurrentAmount : 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Initialize();
    }

    private void Initialize()
    {
        if (wallet == null)
        {
        }
    }

    public bool Add(int amount)
    {
        if (!IsWalletReady())
            return false;

        return wallet.Add(amount);
    }

    public bool Spend(int amount)
    {
        if (!IsWalletReady())
            return false;

        return wallet.Remove(amount);
    }

    public bool CanAfford(int amount)
    {
        if (!IsWalletReady())
            return false;

        return wallet.CanAfford(amount);
    }

    public void SetAmount(int amount)
    {
        if (!IsWalletReady())
            return;

        wallet.SetAmount(amount);
    }

    private bool IsWalletReady()
    {
        return wallet != null;
    }
}