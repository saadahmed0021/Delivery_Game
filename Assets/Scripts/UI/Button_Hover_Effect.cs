using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Button_Hover_Effect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Text")]
    [SerializeField] private TMP_Text buttonText;

    [Header("Hover Color")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.yellow;

    [Header("Hover Scale")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float animationSpeed = 10f;

    private Vector3 normalScale;
    private Vector3 targetScale;
    private Color targetColor;

    private void Awake()
    {
        normalScale = transform.localScale;
        targetScale = normalScale;

        if (buttonText != null)
            buttonText.color = normalColor;

        targetColor = normalColor;
    }

    private void Update()
    {
        // Smooth scale animation
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * animationSpeed
        );

        // Smooth color animation
        if (buttonText != null)
        {
            buttonText.color = Color.Lerp(
                buttonText.color,
                targetColor,
                Time.deltaTime * animationSpeed
            );
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = normalScale * hoverScale;
        targetColor = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = normalScale;
        targetColor = normalColor;
    }
}