using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Rating_UI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rating_Manager ratingManager;
    [SerializeField] private GameObject ratingPanel;
    [SerializeField] private Image[] starImages;
    [SerializeField] private Image[] emptyStarImages;
    [SerializeField] private Sprite filledStar;
    [SerializeField] private Sprite emptyStar;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text ratingText;

    [Header("Display")]
    [SerializeField] private bool showPanelOnStart;

    private bool followsCurrentRating;

    private void Awake()
    {
        ValidateReferences();

        followsCurrentRating = true;

        SetPanelVisible(showPanelOnStart);
    }

    private void Update()
    {
        if (followsCurrentRating && ratingManager != null)
            UpdateRatingDisplay(ratingManager.CurrentRating);
    }

    public void ShowDeliveryResult(Rating_Result result)
    {
        followsCurrentRating = true;
        UpdateRatingDisplay(result.Score);
        UpdateScore(result.Score);
        UpdateRating(result.Score);
        ShowPanel();
    }

    public void ShowOverallRating()
    {
        if (ratingManager == null)
            return;

        float overallRating = ratingManager.GetOverallRating();

        followsCurrentRating = false;
        UpdateRatingDisplay(overallRating);
        UpdateScore(overallRating);
        UpdateRating(overallRating);

        ShowPanel();
    }

    public void HidePanel()
    {
        SetPanelVisible(false);
    }

    public void ShowPanel()
    {
        SetPanelVisible(true);
    }

    private void SetPanelVisible(bool visible)
    {
        if (ratingPanel == null)
            return;

        Cursor_Controller cursorController = Cursor_Controller.Instance;

        if (cursorController != null)
        {
            if (visible)
                cursorController.OpenUI(ratingPanel);
            else
                cursorController.CloseUI(ratingPanel);
        }

        ratingPanel.SetActive(visible);
    }

    public void Clear()
    {
        followsCurrentRating = false;
        UpdateRatingDisplay(0f);

        if (scoreText != null)
            scoreText.text = "0";

        if (ratingText != null)
            ratingText.text = "0.0";
    }

    private void UpdateRatingDisplay(float rating)
    {
        rating = Mathf.Clamp(rating, 0f, 5f);

        if (starImages == null)
            return;

        for (int i = 0; i < starImages.Length; i++)
        {
            if (emptyStarImages != null &&
                i < emptyStarImages.Length &&
                emptyStarImages[i] != null)
            {
                emptyStarImages[i].sprite = emptyStar;
                emptyStarImages[i].type = Image.Type.Simple;
            }

            if (starImages[i] == null)
                continue;

            starImages[i].sprite = filledStar;
            starImages[i].type = Image.Type.Filled;
            starImages[i].fillMethod = Image.FillMethod.Horizontal;
            starImages[i].fillOrigin = (int)Image.OriginHorizontal.Left;
            starImages[i].fillAmount = Mathf.Clamp01(rating - i);
        }
    }

    private void UpdateScore(float score)
    {
        if (scoreText == null)
            return;

        scoreText.text = $"{score:F1}";
    }

    private void UpdateRating(float rating)
    {
        if (ratingText == null)
            return;

        ratingText.text = $"{rating:F1}";
    }

    private void ValidateReferences()
    {
    }
}