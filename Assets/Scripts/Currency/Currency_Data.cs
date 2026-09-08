using UnityEngine;

[CreateAssetMenu(fileName = "Currency_Data", menuName = "Game/Currency Data")]
public class Currency_Data : ScriptableObject
{
    [Header("Currency")]
    [SerializeField] private string currencyName = "Money";
    [SerializeField] private string currencySymbol = "$";

    [Header("Limits")]
    [SerializeField, Min(0)] private int startingAmount = 0;
    [SerializeField, Min(0)] private int maximumAmount = 999999999;

    public string CurrencyName => currencyName;
    public string CurrencySymbol => currencySymbol;
    public int StartingAmount => startingAmount;
    public int MaximumAmount => maximumAmount;

    public int ClampAmount(int amount)
    {
        return Mathf.Clamp(amount, 0, maximumAmount);
    }
}