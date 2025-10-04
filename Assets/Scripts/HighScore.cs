using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using System.Globalization; // for formatting consistency

public class HighScore : MonoBehaviour
{


    [Header("HIGHSCORE")]
    public Text bestAddTimeText;
    public Text bestSubtractTimeText;
    public Text bestMultiplyTimeText;
    public Text bestDivideTimeText;

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

        savePath = Path.Combine(Application.dataPath, "gamesavefile.json");
        LoadHighScores();
    }

    void Start()
    {


        string mode = GameConfig.SelectedProblem;
        int totalQs = Mathf.Max(1, GameConfig.TotalQuestions);
        int solved = GameConfig.ProblemsSolved;
        float secs = GameConfig.ElapsedSeconds;
        bool passed = GameConfig.Passed;


        if (passed && solved >= totalQs)
        {
            TryUpdateBestTime(mode, secs);
            SaveHighScores();
        }

        float bestForMode = GetBestTime(mode);

        ShowAllBestTimes();
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


    string FormatTime(float seconds)
    {
        return $"{seconds:F2} seconds";
    }
}
