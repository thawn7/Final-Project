using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndScript : MonoBehaviour
{
    [Header("UI")]
    public Text titleText;
    public Text detailText;
    public Text highScoresText;
    public Button retryButton;
    public Button homeButton;
    public Button exitButton;

    [Header("End Scene SFX")]
    public AudioClip endClip;           // assign your single sound here
    [Range(0f, 1f)] public float endVolume = 1f;

    private AudioSource sfx;

    void Awake()
    {
        // Create/get AudioSource without triggering the RequireComponent editor log
        sfx = GetComponent<AudioSource>();
        if (sfx == null) sfx = gameObject.AddComponent<AudioSource>();
        sfx.playOnAwake = false;
        sfx.spatialBlend = 0f; // 2D
    }

    void Start()
    {
        if (endClip != null) sfx.PlayOneShot(endClip, endVolume);

        // Buttons
        retryButton.onClick.AddListener(OnRetry);
        homeButton.onClick.AddListener(OnHome);
        exitButton.onClick.AddListener(OnExit);

        // Build result display
        string mode = GameConfig.SelectedProblem;
        int solved = GameConfig.ProblemsSolved;
        int total = GameConfig.TotalQuestions;
        float secs = GameConfig.ElapsedSeconds;

        if (GameConfig.Passed)
        {
            titleText.text = "YOU WON!!";
            detailText.text = $"You solved: {solved} problem{(solved == 1 ? "" : "s")} in {FormatTime(secs)}.";
        }
        else
        {
            titleText.text = "Times Up! You Lost!";
            detailText.text = $"You solved: {solved} of {total} within the time limit.";
        }

        UpdateHighScores(mode, solved, secs, GameConfig.Passed);
        ShowHighScores(mode);
    }

    void OnRetry() => SceneManager.LoadScene("Game");
    void OnHome() => SceneManager.LoadScene("Intro");
    void OnExit() => Application.Quit();

    string FormatTime(float seconds)
    {
        if (seconds < 60f) return $"{seconds:F2} seconds";
        int t = Mathf.FloorToInt(seconds);
        int m = t / 60; int s = t % 60;
        return $"{m}:{s:00} minutes";
    }

    string FastestKey(string mode) => $"HS_{mode}_FastestTime";
    string MostSolvedKey(string mode) => $"HS_{mode}_MostSolved";

    void UpdateHighScores(string mode, int solved, float secs, bool passed)
    {
        int prevMost = PlayerPrefs.GetInt(MostSolvedKey(mode), 0);
        if (solved > prevMost) PlayerPrefs.SetInt(MostSolvedKey(mode), solved);

        if (passed && solved >= GameConfig.TotalQuestions)
        {
            float prevFastest = PlayerPrefs.GetFloat(FastestKey(mode), 0f);
            if (prevFastest <= 0f || secs < prevFastest)
                PlayerPrefs.SetFloat(FastestKey(mode), secs);
        }
        PlayerPrefs.Save();
    }

    void ShowHighScores(string mode)
    {
        int most = PlayerPrefs.GetInt(MostSolvedKey(mode), 0);
        float fastest = PlayerPrefs.GetFloat(FastestKey(mode), 0f);
        string fastestStr = (fastest > 0f) ? FormatTime(fastest) : "—";

        highScoresText.text =
            $"High Scores ({PrettyMode(mode)})\n" +
            $"• Fastest time to solve all {GameConfig.TotalQuestions}: {fastestStr}\n" +
            $"• Most solved within time: {most}";
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
}
