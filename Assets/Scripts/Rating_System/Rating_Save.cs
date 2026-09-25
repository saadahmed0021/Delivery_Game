using UnityEngine;

public class Rating_Save : MonoBehaviour
{
    private const string TotalRatingScoreKey = "Rating_TotalScore";
    private const string CompletedDeliveriesKey = "Rating_CompletedDeliveries";

    [SerializeField] private Rating_Manager ratingManager;

    private void Awake()
    {
        ValidateReferences();
        LoadRating();
    }

    public void SaveRating()
    {
        if (ratingManager == null)
            return;

        PlayerPrefs.SetFloat(
            TotalRatingScoreKey,
            ratingManager.TotalRatingScore
        );

        PlayerPrefs.SetInt(
            CompletedDeliveriesKey,
            ratingManager.CompletedDeliveries
        );

        PlayerPrefs.Save();
    }

    public void LoadRating()
    {
        if (ratingManager == null)
            return;

        float totalRatingScore = PlayerPrefs.GetFloat(
            TotalRatingScoreKey,
            0f
        );

        int completedDeliveries = PlayerPrefs.GetInt(
            CompletedDeliveriesKey,
            0
        );

        ratingManager.LoadRating(
            totalRatingScore,
            completedDeliveries
        );
    }

    public void DeleteSavedRating()
    {
        PlayerPrefs.DeleteKey(TotalRatingScoreKey);
        PlayerPrefs.DeleteKey(CompletedDeliveriesKey);
        PlayerPrefs.Save();

        if (ratingManager != null)
            ratingManager.ResetRating();
    }

    private void ValidateReferences()
    {
        if (ratingManager == null)
        {
        }
    }
}