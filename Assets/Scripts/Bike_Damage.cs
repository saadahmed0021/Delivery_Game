using UnityEngine;
using UnityEngine.UI;

public class Bike_Damage : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;

    [Header("Testing")]
    [SerializeField] private bool useTestValue = true;
    [SerializeField, Range(0f, 1f)] private float testValue = 1f;

    private void OnEnable()  => slider.onValueChanged.AddListener(UpdateFill);
    private void OnDisable() => slider.onValueChanged.RemoveListener(UpdateFill);

    private void UpdateFill(float value)
    {
        if (useTestValue) return; // ignore real slider input while testing

        fillImage.fillAmount = Mathf.InverseLerp(slider.minValue, slider.maxValue, value);
    }

    // Runs in the editor whenever you tweak a value in the Inspector,
    // even while not in Play mode.
    private void OnValidate()
    {
        if (useTestValue && fillImage != null)
        {
            fillImage.fillAmount = testValue;
        }
    }
}