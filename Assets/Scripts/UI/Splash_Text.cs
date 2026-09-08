using UnityEngine;

public class Splash_Text : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float duration = 0.6f;
    [SerializeField] private float overshoot = 1.15f;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    private void Start()
    {
        StartCoroutine(PopAnimation());
    }

    private System.Collections.IEnumerator PopAnimation()
    {
        // 0 -> 1.15
        yield return ScaleTo(Vector3.zero, originalScale * overshoot, duration * 0.6f);

        // 1.15 -> 0.95
        yield return ScaleTo(
            originalScale * overshoot,
            originalScale * 0.95f,
            duration * 0.2f
        );

        // 0.95 -> 1
        yield return ScaleTo(
            originalScale * 0.95f,
            originalScale,
            duration * 0.2f
        );
    }

    private System.Collections.IEnumerator ScaleTo(
        Vector3 from,
        Vector3 to,
        float time)
    {
        float elapsed = 0f;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / time);

            // Smooth animation
            t = Mathf.SmoothStep(0f, 1f, t);

            transform.localScale = Vector3.Lerp(from, to, t);

            yield return null;
        }

        transform.localScale = to;
    }
}