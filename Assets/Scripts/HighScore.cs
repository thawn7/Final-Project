using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

public class HighScore : MonoBehaviour
{
    public Text Addition, Subtraction, Multiplication, Division;

    string savePath;
    List<Entry> scores = new();

    [System.Serializable]
    public class Entry { public string mode; public float bestTime; }

    void Awake()
    {
    #if UNITY_EDITOR || UNITY_STANDALONE
            savePath = Path.Combine(Application.dataPath, "gamesavefile.json");
    #else
        savePath = Path.Combine(Application.persistentDataPath, "gamesavefile.json");
    #endif

        LoadScores();
    }

    void Start()
    {
        string mode = GameConfig.SelectedProblem;
        float time = GameConfig.ElapsedSeconds;
        bool passed = GameConfig.Passed;
        int solved = GameConfig.ProblemsSolved, total = GameConfig.TotalQuestions;

        if (passed && solved >= total) UpdateBest(mode, time);
        SaveScores();
        ShowAll();
    }


    void UpdateBest(string mode, float newTime)
    {
        var e = scores.Find(x => x.mode == mode);
        if (e == null) scores.Add(new Entry { mode = mode, bestTime = newTime });
        else if (newTime < e.bestTime) e.bestTime = newTime;
    }

    void SaveScores()
    {
        using StreamWriter w = new(savePath, false);
        foreach (var e in scores)
            w.WriteLine($"mode: {e.mode}, bestTime: {e.bestTime:F2} seconds");
        Debug.Log("Score saved to gamesavefile.json! Wait a few second to show up in Asset Folder!");
    }

    void LoadScores()
    {
        scores.Clear();
        if (!File.Exists(savePath)) return;

        foreach (var line in File.ReadAllLines(savePath))
        {
            if (!line.Contains("bestTime:")) continue;
            string[] p = line.Split(',');
            string mode = p[0].Replace("mode:", "").Trim();
            string timeStr = p[1].Replace("bestTime:", "").Replace("seconds", "").Trim();

            if (float.TryParse(timeStr, NumberStyles.Float, CultureInfo.InvariantCulture, out float t))
                scores.Add(new Entry { mode = mode, bestTime = t });
        }
    }

    void ShowAll()
    {
        Set(Addition, "Add");
        Set(Subtraction, "Subtract");
        Set(Multiplication, "Multiply");
        Set(Division, "Divide");
    }

    void Set(Text target, string mode)
    {
        if (!target) return;
        float t = Get(mode);
        target.text = (t > 0f) ? FormatTime(t) : "—";
    }

    float Get(string mode) => scores.Find(x => x.mode == mode)?.bestTime ?? 0f;

    string FormatTime(float seconds)
    {
        if (seconds < 60f)
            return $"{seconds:F2} seconds";
        int m = Mathf.FloorToInt(seconds / 60);
        int s = Mathf.FloorToInt(seconds % 60);
        return $"{m}:{s:00} minutes";
    }
}
