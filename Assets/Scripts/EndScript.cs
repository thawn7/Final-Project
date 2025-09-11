using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndScript : MonoBehaviour
{
    [Header("UI (current run)")]
    public Text problemText;        // e.g., "Subtraction"
    public Text finishTimeText;     // this run's time (time string only)
    public Text bestTimeText;       // best time for the selected mode (time string only)

    [Header("Result Message (one text)")]
    public Text resultText;         // <-- NEW: single pass/fail message text

    [Header("UI (all best times by mode) - times only")]
    public Text bestAddTimeText;       // time only
    public Text bestSubtractTimeText;  // time only
    public Text bestMultiplyTimeText;  // time only
    public Text bestDivideTimeText;    // time only

    [Header("Buttons")]
    public Button retryButton;
    public Button homeButton;
    public Button exitButton;

    [Header("End Scene SFX")]
    public AudioClip endClip;                 // single sound to play on load
    [Range(0f, 1f)] public float endVolume = 1f;

    private AudioSource sfx;
    public void Exit()
    {
        Application.Quit();

    #if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }
    void Awake()
    {
        // silent auto-create AudioSource
        sfx = GetComponent<AudioSource>();
        if (sfx == null) sfx = gameObject.AddComponent<AudioSource>();
        sfx.playOnAwake = false;
        sfx.spatialBlend = 0f; // 2D
    }

    void Start()
    {
        if (endClip != null) sfx.PlayOneShot(endClip, endVolume);

        if (retryButton) retryButton.onClick.AddListener(() => SceneManager.LoadScene("Game"));
        if (homeButton) homeButton.onClick.AddListener(() => SceneManager.LoadScene("Intro"));
        //if (exitButton) exitButton.onClick.AddListener(Application.Quit);

        // Pull current session data
        string mode = GameConfig.SelectedProblem;            // "Add","Subtract","Multiply","Divide"
        int minutes = Mathf.Max(1, GameConfig.SelectedMinutes);
        int totalQs = Mathf.Max(1, GameConfig.TotalQuestions);
        int solved = GameConfig.ProblemsSolved;
        float secs = GameConfig.ElapsedSeconds;
        bool passed = GameConfig.Passed;

        // One display text for pass/fail + summary  --------------------------
        string msg = passed
            ? $"YOU WON!!"
            : $"Time's Up! You Lost!";
        if (resultText) resultText.text = msg;
        // --------------------------------------------------------------------

        // Display chosen problem + this run's time (just the values)
        if (problemText) problemText.text = PrettyMode(mode);
        if (finishTimeText) finishTimeText.text = FormatTime(secs);

        // Update best time for the selected mode only when all solved
        if (passed && solved >= totalQs)
        {
            float prevFastest = PlayerPrefs.GetFloat(FastestKey(mode, minutes, totalQs), 0f);
            if (prevFastest <= 0f || secs < prevFastest)
                PlayerPrefs.SetFloat(FastestKey(mode, minutes, totalQs), secs);
            PlayerPrefs.Save();
        }

        // Optional: show best time for the selected mode in bestTimeText
        if (bestTimeText)
        {
            float selectedBest = PlayerPrefs.GetFloat(FastestKey(mode, minutes, totalQs), 0f);
            bestTimeText.text = (selectedBest > 0f) ? FormatTime(selectedBest) : "—";
        }

        // Show best times (times only) for ALL four modes using SAME minutes & totalQs
        ShowAllBestTimes(minutes, totalQs);
    }

    // ---------- All-modes display ----------
    void ShowAllBestTimes(int minutes, int totalQs)
    {
        SetBestTimeText(bestAddTimeText, "Add", minutes, totalQs);
        SetBestTimeText(bestSubtractTimeText, "Subtract", minutes, totalQs);
        SetBestTimeText(bestMultiplyTimeText, "Multiply", minutes, totalQs);
        SetBestTimeText(bestDivideTimeText, "Divide", minutes, totalQs);
    }

    void SetBestTimeText(Text target, string modeKey, int minutes, int totalQs)
    {
        if (!target) return;
        float t = PlayerPrefs.GetFloat(FastestKey(modeKey, minutes, totalQs), 0f);
        target.text = (t > 0f) ? FormatTime(t) : "—";   // time only
    }

    // ---------- Keys & helpers ----------
    // Best time is stored per (mode, minutes, totalQs)
    string FastestKey(string mode, int minutes, int totalQs)
        => $"HS_{mode}_{minutes}m_{totalQs}q_FastestTime";

    string PrettyMode(string mode)
    {
        switch (mode)
        {
            case "Add": return "Addition";
            case "Subtract": return "Subtraction";
            case "Multiply": return "Multiplication";
            case "Divide": return "Division";
            default: return mode;
        }
    }

    string FormatTime(float seconds)
    {
        if (seconds < 60f) return $"{seconds:F2} seconds";
        int t = Mathf.FloorToInt(seconds);
        int m = t / 60, s = t % 60;
        return $"{m}:{s:00} minutes";
    }
}
