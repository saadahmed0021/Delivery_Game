using UnityEngine;
using UnityEngine.UI;

public class Bike_Damage : MonoBehaviour
{
    [SerializeField] private Order_Damage orderDamage;
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;

    private void Start()
    {
        if (orderDamage == null)
            orderDamage = GetComponent<Order_Damage>();

        slider.minValue = 0f;
        slider.maxValue = 1f;

        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (orderDamage == null)
            return;

        float damage = orderDamage.Damage;

        slider.value = damage;

        if (fillImage != null)
            fillImage.fillAmount = damage;
    }
}