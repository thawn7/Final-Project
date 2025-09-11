using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public Text questionText;
    public InputField answerInput;
    public Button submitButton;
    public Text timerText;
    public Text progressText; // e.g., "Q 1/5"
    public Button backButton; // if you already added this

    [Header("Settings")]
    public int totalQuestions = 5;

    [Header("SFX")]                           // <-- NEW
    public AudioClip correctClip;             // <-- NEW (assign in Inspector)
    [Range(0f, 1f)] public float correctVolume = 1f; // <-- NEW

    private AudioSource sfx;                  // <-- NEW

    private int currentQuestionIndex = 0; // also equals solved count
    private int currentAnswer = 0;
    private float elapsedSeconds = 0f;
    private int timeLimitSeconds;

    private string mode;
    private bool gameOver = false;

    void Awake()                               // <-- NEW
    {
        // Create/get AudioSource without RequireComponent (prevents console log)
        sfx = GetComponent<AudioSource>();
        if (sfx == null) sfx = gameObject.AddComponent<AudioSource>();
        sfx.playOnAwake = false;
        sfx.spatialBlend = 0f; // 2D
    }

    void Start()
    {
        // Load selection from Intro
        mode = GameConfig.SelectedProblem;
        timeLimitSeconds = Mathf.Max(1, GameConfig.SelectedMinutes) * 60;

        // Use the Intro dropdown value here
        totalQuestions = Mathf.Max(1, GameConfig.SelectedTotalQuestions);
        GameConfig.TotalQuestions = totalQuestions; // keep End scene in sync

        // Wire UI
        submitButton.onClick.AddListener(SubmitAnswer);
        answerInput.onEndEdit.AddListener(OnEndEditReturn);

        if (backButton != null) backButton.onClick.AddListener(onBackClicked); // if you use Back

        // Init
        currentQuestionIndex = 0;
        elapsedSeconds = 0f;
        gameOver = false;

        NextQuestion();
        UpdateProgress();
        UpdateTimerUI();
        answerInput.text = "";
        answerInput.ActivateInputField();
    }

    void Update()
    {
        if (gameOver) return;

        elapsedSeconds += Time.deltaTime;
        UpdateTimerUI();

        if (elapsedSeconds >= timeLimitSeconds)
        {
            EndGame(passed: false);
        }
    }

    void OnEndEditReturn(string _)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            SubmitAnswer();
    }

    void SubmitAnswer()
    {
        if (gameOver) return;

        if (int.TryParse(answerInput.text, out int user))
        {
            if (user == currentAnswer)
            {
                PlayCorrectSfx(); // <-- NEW: play success sound

                currentQuestionIndex++; // one more solved

                if (currentQuestionIndex >= totalQuestions)
                {
                    // Completed all before time limit
                    if (elapsedSeconds <= timeLimitSeconds)
                    {
                        EndGame(passed: true);
                        return;
                    }
                }
                else
                {
                    NextQuestion();
                    UpdateProgress();
                }
            }
        }

        // Prepare next input
        answerInput.text = "";
        answerInput.ActivateInputField();
        answerInput.caretPosition = 0;
    }

    // <-- NEW
    void PlayCorrectSfx()
    {
        if (correctClip != null) sfx.PlayOneShot(correctClip, correctVolume);
    }

    void NextQuestion()
    {
        int a = Random.Range(0, 10);
        int b = Random.Range(0, 10);

        switch (mode)
        {
            case "Add":
                currentAnswer = a + b;
                questionText.text = $"{a} + {b} = ?";
                break;

            case "Subtract":
                if (b > a) { int t = a; a = b; b = t; }
                currentAnswer = a - b;
                questionText.text = $"{a} - {b} = ?";
                break;

            case "Multiply":
                currentAnswer = a * b;
                questionText.text = $"{a} × {b} = ?";
                break;

            case "Divide":
                b = Random.Range(1, 10);
                int q = Random.Range(0, 10);
                a = b * q;
                currentAnswer = q;
                questionText.text = $"{a} ÷ {b} = ?";
                break;

            default:
                currentAnswer = a + b;
                questionText.text = $"{a} + {b} = ?";
                break;
        }
    }

    void UpdateProgress()
    {
        progressText.text = $"Question: {Mathf.Min(currentQuestionIndex + 1, totalQuestions)}/{totalQuestions}";
    }

    void UpdateTimerUI()
    {
        int t = Mathf.FloorToInt(elapsedSeconds);
        int m = t / 60;
        int s = t % 60;
        int limitM = timeLimitSeconds / 60;
        timerText.text = $"Time {m}:{s:00} / {limitM}:00";
    }

    void EndGame(bool passed)
    {
        if (gameOver) return;
        gameOver = true;

        // Save results → End scene
        GameConfig.Passed = passed;
        GameConfig.ProblemsSolved = Mathf.Clamp(currentQuestionIndex, 0, totalQuestions);
        GameConfig.ElapsedSeconds = elapsedSeconds;

        SceneManager.LoadScene("End");
    }

    public void onBackClicked() // if you use a Back button
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Intro");
    }
}
