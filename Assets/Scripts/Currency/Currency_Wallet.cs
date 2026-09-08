using System;
using UnityEngine;

public class Currency_Wallet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Currency_Data currencyData;

    private int currentAmount;

    public int CurrentAmount => currentAmount;
    public Currency_Data CurrencyData => currencyData;

    public event Action<int> OnBalanceChanged;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (currencyData == null)
        {
            Debug.LogError(
                $"{nameof(Currency_Wallet)}: Currency Data is not assigned.",
                this
            );

            return;
        }

        currentAmount = currencyData.ClampAmount(currencyData.StartingAmount);
    }

    public bool Add(int amount)
    {
        if (amount <= 0 || currencyData == null)
            return false;

        long newAmount = (long)currentAmount + amount;

        int clampedAmount = currencyData.ClampAmount(
            newAmount > int.MaxValue
                ? int.MaxValue
                : (int)newAmount
        );

        if (clampedAmount == currentAmount)
            return false;

        currentAmount = clampedAmount;
        OnBalanceChanged?.Invoke(currentAmount);

        return true;
    }

    public bool Remove(int amount)
    {
        if (amount <= 0 || currentAmount < amount)
            return false;

        currentAmount -= amount;
        OnBalanceChanged?.Invoke(currentAmount);

        return true;
    }

    public bool CanAfford(int amount)
    {
        if (amount <= 0)
            return false;

        return currentAmount >= amount;
    }

    public void SetAmount(int amount)
    {
        if (currencyData == null)
            return;

        int newAmount = currencyData.ClampAmount(amount);

        if (newAmount == currentAmount)
            return;

        currentAmount = newAmount;
        OnBalanceChanged?.Invoke(currentAmount);
    }
}