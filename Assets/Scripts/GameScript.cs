using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameScript : MonoBehaviour
{
    public Text questionText, timerText, progressText;
    public InputField answerInput;
    public Button submitButton, backButton;
    public AudioClip correctClip;
    [Range(0f, 1f)] public float correctVolume = 1f;
    public int totalQuestions = 5;

    AudioSource sfx;
    string mode;
    int currentQuestion, currentAnswer, timeLimit;
    float elapsed;
    bool gameOver;

    void Start()
    {
        // Setup
        sfx = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        sfx.playOnAwake = false;

        mode = GameConfig.SelectedProblem;
        timeLimit = Mathf.Max(1, GameConfig.SelectedMinutes) * 60;
        totalQuestions = Mathf.Max(1, GameConfig.SelectedTotalQuestions);
        GameConfig.TotalQuestions = totalQuestions;

        submitButton.onClick.AddListener(SubmitAnswer);
        answerInput.onEndEdit.AddListener(OnEnter);
        backButton.onClick.AddListener(() => SceneManager.LoadScene("Intro"));

        elapsed = 0f;
        currentQuestion = 0;
        gameOver = false;

        NextQuestion();
        UpdateUI();
    }

    void Update()
    {
        if (gameOver) return;

        elapsed += Time.deltaTime;
        UpdateTimer();

        if (elapsed >= timeLimit) EndGame(false);
    }

    void OnEnter(string _)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            SubmitAnswer();
    }

    void SubmitAnswer()
    {
        if (gameOver || !int.TryParse(answerInput.text, out int user)) return;

        if (user == currentAnswer)
        {
            PlayCorrect();
            currentQuestion++;
            if (currentQuestion >= totalQuestions) EndGame(true);
            else NextQuestion();
        }

        answerInput.text = "";
        answerInput.ActivateInputField();
    }

    void PlayCorrect()
    {
        if (correctClip) sfx.PlayOneShot(correctClip, correctVolume);
    }

    void NextQuestion()
    {
        int a = Random.Range(0, 10), b = Random.Range(0, 10);

        switch (mode)
        {
            case "Subtract":
                if (b > a) (a, b) = (b, a);
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

            default: // Add
                currentAnswer = a + b;
                questionText.text = $"{a} + {b} = ?";
                break;
        }


        UpdateUI();
    }

    void UpdateUI()
    {
        progressText.text = $"Question: {Mathf.Min(currentQuestion + 1, totalQuestions)}/{totalQuestions}";
        UpdateTimer();
    }

    void UpdateTimer()
    {
        int totalSeconds = Mathf.FloorToInt(elapsed);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        int limitM = timeLimit / 60;

        if (elapsed < 60f)
            timerText.text = $"Time: {minutes}:{seconds:00} seconds";
        else
            timerText.text = $"Time: {minutes}:{seconds:00} / {limitM}:00 minutes";
    }



    void EndGame(bool passed)
    {
        if (gameOver) return;
        gameOver = true;

        GameConfig.Passed = passed;
        GameConfig.ProblemsSolved = Mathf.Clamp(currentQuestion, 0, totalQuestions);
        GameConfig.ElapsedSeconds = elapsed;

        SceneManager.LoadScene("End");
    }
}
