using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndScript : MonoBehaviour
{
    public Text problemTitle, finishTimeText, resultText;
    public Button retryButton, homeButton, exitButton;
    public AudioClip endClip;
    [Range(0f, 1f)] private float endVolume = 1f;

    AudioSource sfx;

    void Awake()
    {
        sfx = GetComponent<AudioSource>();
        if (!sfx)
        {
            sfx = gameObject.AddComponent<AudioSource>();
            sfx.playOnAwake = false;
            sfx.spatialBlend = 0f;
        }
    }

    void Start()
    {

        if (endClip)
        {
            sfx.Stop();
            sfx.PlayOneShot(endClip, endVolume);
        }

        retryButton.onClick.AddListener(() => SceneManager.LoadScene("Game")); 
        homeButton.onClick.AddListener(() => SceneManager.LoadScene("Intro")); 
        exitButton.onClick.AddListener(Exit);

        string mode = GameConfig.SelectedProblem ?? "Add";
        float secs = GameConfig.ElapsedSeconds;
        bool passed = GameConfig.Passed;

        if (problemTitle) problemTitle.text = whattypeofproblem(mode);
        if (finishTimeText) finishTimeText.text = FormatPlayTime(secs);

        string message = passed ? "YOU WON!!" : "Time’s Up! You Lost!";
        if (resultText) resultText.text = message;
    }


    void Exit()
    {
        Application.Quit();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    string whattypeofproblem(string mode) 
    { 
        switch (mode) 
    { 
            case "Add": return "Addition"; 
            case "Subtract": return "Subtraction"; 
            case "Multiply": return "Multiplication"; 
            case "Divide": 
            return "Division"; 
            default: return mode; 
        } 
    }

    string FormatPlayTime(float seconds)
    {
        if (seconds <= 0f) return "0:00 seconds";

        int m = Mathf.FloorToInt(seconds / 60);
        int s = Mathf.FloorToInt(seconds % 60);

        if (seconds < 60f)
            return $"{m}:{s:00} seconds";
        else
            return $"{m}:{s:00} minutes";
    }

}
