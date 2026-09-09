using UnityEngine;

public class Order_Damage : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField, Range(0f, 1f)] private float damage;

    public float Damage => damage;
    public float Condition => 1f - damage;
    public bool IsDamaged => damage > 0f;
    public bool IsDestroyed => damage >= 1f;

    public void AddDamage(float amount)
    {
        if (amount <= 0f)
            return;

        damage = Mathf.Clamp01(damage + amount);
    }

    public void SetDamage(float value)
    {
        damage = Mathf.Clamp01(value);
    }

    public void ResetDamage()
    {
        damage = 0f;
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