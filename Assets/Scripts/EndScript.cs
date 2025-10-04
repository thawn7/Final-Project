using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using System.Globalization; // for formatting consistency

public class EndScript : MonoBehaviour
{
    [Header("UI (current run)")]
    public Text problemText;
    public Text finishTimeText;
    public Text bestTimeText;

    [Header("Result Message (one text)")]
    public Text resultText;

    [Header("UI (all best times by mode) - times only")]
    public Text bestAddTimeText;
    public Text bestSubtractTimeText;
    public Text bestMultiplyTimeText;
    public Text bestDivideTimeText;

    [Header("Buttons")]
    public Button retryButton;
    public Button homeButton;
    public Button exitButton;

    [Header("End Scene SFX")]
    public AudioClip endClip;
    [Range(0f, 1f)] public float endVolume = 1f;

    private AudioSource sfx;
    private string savePath;

    [System.Serializable]
    public class HighScoreEntry
    {
        public string mode;
        public float bestTime;
    }

    private List<HighScoreEntry> highScores = new List<HighScoreEntry>();

    void Awake()
    {
        sfx = GetComponent<AudioSource>();
        if (sfx == null) sfx = gameObject.AddComponent<AudioSource>();
        sfx.playOnAwake = false;
        sfx.spatialBlend = 0f;

        savePath = Path.Combine(Application.dataPath, "gamesavefile.json");
        LoadHighScores();
    }

    void Start()
    {
        if (endClip != null) sfx.PlayOneShot(endClip, endVolume);

        if (retryButton) retryButton.onClick.AddListener(() => SceneManager.LoadScene("Game"));
        if (homeButton) homeButton.onClick.AddListener(() => SceneManager.LoadScene("Intro"));
        if (exitButton) exitButton.onClick.AddListener(Exit);

        string mode = GameConfig.SelectedProblem;
        int totalQs = Mathf.Max(1, GameConfig.TotalQuestions);
        int solved = GameConfig.ProblemsSolved;
        float secs = GameConfig.ElapsedSeconds;
        bool passed = GameConfig.Passed;

        if (problemText) problemText.text = PrettyMode(mode);
        if (finishTimeText) finishTimeText.text = FormatTime(secs);

        string msg = passed ? "YOU WON!!" : "Time's Up! You Lost!";
        if (resultText) resultText.text = msg;

        if (passed && solved >= totalQs)
        {
            TryUpdateBestTime(mode, secs);
            SaveHighScores();
        }

        float bestForMode = GetBestTime(mode);
        if (bestTimeText) bestTimeText.text = (bestForMode > 0f) ? FormatTime(bestForMode) : "—";

        ShowAllBestTimes();
    }

    void Exit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // ---------- Save & Load ----------

    void TryUpdateBestTime(string mode, float newTime)
    {
        HighScoreEntry entry = highScores.Find(e => e.mode == mode);

        if (entry == null)
        {
            highScores.Add(new HighScoreEntry { mode = mode, bestTime = newTime });
        }
        else
        {
            // Only update if new time is better (lower)
            if (newTime < entry.bestTime)
            {
                entry.bestTime = newTime;
                Debug.Log($"New record for {mode}: {newTime}");
            }
            else
            {
                Debug.Log($"Did not beat {mode} record ({entry.bestTime}), keeping old score.");
            }
        }
    }

    float GetBestTime(string mode)
    {
        HighScoreEntry entry = highScores.Find(e => e.mode == mode);
        return (entry != null) ? entry.bestTime : 0f;
    }

    void SaveHighScores()
    {
        using (StreamWriter writer = new StreamWriter(savePath, false))
        {
            foreach (var entry in highScores)
            {
                // Round to 2 decimal places and add "seconds"
                string formattedTime = entry.bestTime.ToString("F2", CultureInfo.InvariantCulture);
                writer.WriteLine($"mode: {entry.mode}, bestTime: {formattedTime} seconds");
            }
        }
        Debug.Log("Saved high scores to " + savePath);
    }

    void LoadHighScores()
    {
        highScores.Clear();
        if (File.Exists(savePath))
        {
            string[] lines = File.ReadAllLines(savePath);
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (!line.Contains("bestTime:")) continue;

                string[] parts = line.Split(',');
                if (parts.Length >= 2)
                {
                    string modePart = parts[0].Trim().Replace("mode: ", "");
                    string timePart = parts[1].Trim().Replace("bestTime: ", "").Replace("seconds", "").Trim();

                    if (float.TryParse(timePart, NumberStyles.Float, CultureInfo.InvariantCulture, out float timeValue))
                    {
                        highScores.Add(new HighScoreEntry { mode = modePart, bestTime = timeValue });
                    }
                }
            }
            Debug.Log("Loaded high scores from " + savePath);
        }
    }

    // ---------- UI Helpers ----------

    void ShowAllBestTimes()
    {
        SetBestTimeText(bestAddTimeText, "Add");
        SetBestTimeText(bestSubtractTimeText, "Subtract");
        SetBestTimeText(bestMultiplyTimeText, "Multiply");
        SetBestTimeText(bestDivideTimeText, "Divide");
    }

    void SetBestTimeText(Text target, string mode)
    {
        if (!target) return;
        float t = GetBestTime(mode);
        target.text = (t > 0f) ? FormatTime(t) : "—";
    }

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
        return $"{seconds:F2} seconds";
    }
}
