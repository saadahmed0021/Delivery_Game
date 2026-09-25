using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] private Order_Damage orderDamage;

    [SerializeField, Range(0f, 1f)]
    private float damageAmount = 0.1f;

    [Header("Damageable Objects")]
    [SerializeField] private string damageTag = "Obstacle";

    private void Start()
    {
        if (orderDamage == null)
            orderDamage = GetComponent<Order_Damage>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(damageTag))
            return;

        if (orderDamage != null)
        {
            orderDamage.AddDamage(damageAmount);
        }
    }
}