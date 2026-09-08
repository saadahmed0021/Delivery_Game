using UnityEngine;

public class Currency_Save : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Currency_Wallet wallet;

    [Header("Save Settings")]
    [SerializeField] private string saveKey = "PLAYER_CURRENCY";

    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void Initialize()
    {
        if (wallet == null)
        {
            Debug.LogError(
                $"{nameof(Currency_Save)}: Currency Wallet is not assigned.",
                this
            );
        }
    }

    private void Start()
    {
        Load();
    }

    private void Subscribe()
    {
        if (wallet == null)
            return;

        wallet.OnBalanceChanged += HandleBalanceChanged;
    }

    private void Unsubscribe()
    {
        if (wallet == null)
            return;

        wallet.OnBalanceChanged -= HandleBalanceChanged;
    }

    private void HandleBalanceChanged(int amount)
    {
        Save(amount);
    }

    public void Save()
    {
        if (wallet == null)
            return;

        Save(wallet.CurrentAmount);
    }

    private void Save(int amount)
    {
        PlayerPrefs.SetInt(saveKey, amount);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        if (wallet == null)
            return;

        if (!PlayerPrefs.HasKey(saveKey))
            return;

        int savedAmount = PlayerPrefs.GetInt(saveKey);
        wallet.SetAmount(savedAmount);
    }

    public void DeleteSave()
    {
        if (!PlayerPrefs.HasKey(saveKey))
            return;

        PlayerPrefs.DeleteKey(saveKey);
        PlayerPrefs.Save();
    }
}