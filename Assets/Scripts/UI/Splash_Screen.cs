using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float displayTime = 3f;

    [Header("Next Scene")]
    [SerializeField] private string nextSceneName = "MainMenu";

    private void Start()
    {
        StartCoroutine(SplashSequence());
    }

    private IEnumerator SplashSequence()
    {
        // Keep splash screen visible for 3 seconds
        yield return new WaitForSeconds(displayTime);

        // Load next scene
        SceneManager.LoadScene(nextSceneName);
    }
}