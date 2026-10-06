using System;
using UnityEngine;

public class Order_Damage : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField, Range(0f, 1f)] private float damage;

    public float Damage => damage;
    public float Condition => 1f - damage;
    public bool IsDamaged => damage > 0f;
    public bool IsDestroyed => damage >= 1f;

    public event Action DamageRegistered;
    public event Action DamageChanged;

    public void AddDamage(float amount)
    {
        if (amount <= 0f)
            return;

        float previousDamage = damage;
        damage = Mathf.Clamp01(damage + amount);

        if (!Mathf.Approximately(previousDamage, damage))
        {
            DamageRegistered?.Invoke();
            DamageChanged?.Invoke();
        }
    }

    public void SetDamage(float value)
    {
        float previousDamage = damage;
        damage = Mathf.Clamp01(value);

        if (!Mathf.Approximately(previousDamage, damage))
            DamageChanged?.Invoke();
    }

    public void ResetDamage()
    {
        if (damage == 0f)
            return;

        damage = 0f;
        DamageChanged?.Invoke();
    }

    public void SetDamagePercent(float percent)
    {
        damage = Mathf.Clamp01(percent / 100f);
    }

    public float GetDamagePercent()
    {
        return damage * 100f;
    }

    public float GetConditionPercent()
    {
        return Condition * 100f;
    }
}