using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Rating_UI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rating_Manager ratingManager;
    [SerializeField] private GameObject ratingPanel;
    [SerializeField] private Image[] starImages;
    [SerializeField] private Sprite filledStar;
    [SerializeField] private Sprite emptyStar;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text ratingText;

    [Header("Display")]
    [SerializeField] private bool showPanelOnStart;

    private void Awake()
    {
        ValidateReferences();

        if (ratingPanel != null)
            ratingPanel.SetActive(showPanelOnStart);
    }

    public void ShowDeliveryResult(Rating_Result result)
    {
        UpdateStars(result.Stars);
        UpdateScore(result.Score);
        UpdateRating(result.Stars);
        ShowPanel();
    }

    public void ShowOverallRating()
    {
        if (ratingManager == null)
            return;

        float overallRating = ratingManager.GetOverallRating();

        UpdateStars(Mathf.RoundToInt(overallRating));
        UpdateScore(overallRating);
        UpdateRating(overallRating);

        ShowPanel();
    }

    public void HidePanel()
    {
        if (ratingPanel != null)
            ratingPanel.SetActive(false);
    }

    public void ShowPanel()
    {
        if (ratingPanel != null)
            ratingPanel.SetActive(true);
    }

    public void Clear()
    {
        UpdateStars(0);

        if (scoreText != null)
            scoreText.text = "0";

        if (ratingText != null)
            ratingText.text = "0.0";
    }

    private void UpdateStars(int stars)
    {
        if (starImages == null)
            return;

        stars = Mathf.Clamp(stars, 0, 5);

        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] == null)
                continue;

            starImages[i].sprite = i < stars
                ? filledStar
                : emptyStar;
        }
    }

    private void UpdateScore(float score)
    {
        if (scoreText == null)
            return;

        scoreText.text = $"{score:F0}";
    }

    private void UpdateRating(float rating)
    {
        if (ratingText == null)
            return;

        ratingText.text = $"{rating:F1}";
    }

    private void ValidateReferences()
    {
        if (ratingManager == null)
            Debug.LogError("Rating UI: Rating_Manager is not assigned.", this);

        if (ratingPanel == null)
            Debug.LogError("Rating UI: Rating Panel is not assigned.", this);

        if (starImages == null || starImages.Length == 0)
            Debug.LogError("Rating UI: Star Images are not assigned.", this);

        if (filledStar == null)
            Debug.LogError("Rating UI: Filled Star sprite is not assigned.", this);

        if (emptyStar == null)
            Debug.LogError("Rating UI: Empty Star sprite is not assigned.", this);
    }
}