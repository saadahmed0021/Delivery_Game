using TMPro;
using UnityEngine;

public class Currency_UI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Currency_Manager currencyManager;
    [SerializeField] private TMP_Text currencyText;

    private Currency_Wallet wallet;

    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        Subscribe();
        RefreshUI();
    }

    private void OnDisable()
    {
        Unsubscribe();
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
                $"{nameof(Currency_UI)}: Currency Manager is not assigned or found.",
                this
            );

            return;
        }

        wallet = currencyManager.Wallet;

        if (wallet == null)
        {
            Debug.LogError(
                $"{nameof(Currency_UI)}: Currency Wallet is not assigned.",
                this
            );
        }

        if (currencyText == null)
        {
            Debug.LogError(
                $"{nameof(Currency_UI)}: Currency Text is not assigned.",
                this
            );
        }
    }

    private void Subscribe()
    {
        if (wallet == null)
            return;

        wallet.OnBalanceChanged += UpdateCurrencyText;
    }

    private void Unsubscribe()
    {
        if (wallet == null)
            return;

        wallet.OnBalanceChanged -= UpdateCurrencyText;
    }

    private void RefreshUI()
    {
        if (wallet == null || currencyText == null)
            return;

        UpdateCurrencyText(wallet.CurrentAmount);
    }

    private void UpdateCurrencyText(int amount)
    {
        if (currencyText == null)
            return;

        currencyText.text = amount.ToString("N0");
    }
}