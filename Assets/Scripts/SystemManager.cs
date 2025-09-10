using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SystemManager : MonoBehaviour
{
    [Header("UI (Legacy)")]
    public Dropdown timerDropdown;      // existing
    public Dropdown questionsDropdown;  // NEW
    public Button playButton;           // existing
    public Button[] problemButtons;     // existing

    private string selectedProblem = "";

    private readonly int[] minuteMap = { 1, 3, 5, 10 };
    private readonly int[] questionMap = { 5, 10, 15, 20, 30 }; // match your dropdown options

    void Awake()
    {
        if (playButton == null) Debug.LogError("PlayUIManager: playButton not assigned.");
        if (timerDropdown == null) Debug.LogError("PlayUIManager: timerDropdown not assigned.");
        if (questionsDropdown == null) Debug.LogError("PlayUIManager: questionsDropdown not assigned."); // NEW
        if (problemButtons == null || problemButtons.Length == 0) Debug.LogError("PlayUIManager: problemButtons not assigned.");
    }

    void Start()
    {
        if (timerDropdown != null) timerDropdown.value = 0;       // default 1 minute
        if (questionsDropdown != null) questionsDropdown.value = 0; // default 5 questions
        if (playButton != null) playButton.interactable = false;

        if (problemButtons != null)
        {
            foreach (var btn in problemButtons)
                if (btn != null) btn.onClick.AddListener(() => OnProblemButtonClicked(btn));
        }

        if (timerDropdown != null) timerDropdown.onValueChanged.AddListener(_ => Validate());
        if (questionsDropdown != null) questionsDropdown.onValueChanged.AddListener(_ => Validate()); // not required but fine
    }

    void OnProblemButtonClicked(Button btn)
    {
        if (btn == null) return;
        selectedProblem = btn.name; // Add/Subtract/Multiply/Divide
        Validate();
    }

    void Validate()
    {
        if (playButton == null) return;
        playButton.interactable = !string.IsNullOrEmpty(selectedProblem);
    }

    int GetSelectedMinutesSafe()
    {
        if (timerDropdown == null) return 1;
        int idx = Mathf.Clamp(timerDropdown.value, 0, minuteMap.Length - 1);
        return minuteMap[idx];
    }

    int GetSelectedQuestionsSafe()
    {
        if (questionsDropdown == null) return 5;
        int idx = Mathf.Clamp(questionsDropdown.value, 0, questionMap.Length - 1);
        return questionMap[idx];
    }

    // Hook this to Play button OnClick()
    public void StartGame()
    {
        if (string.IsNullOrEmpty(selectedProblem))
        {
            Debug.LogError("PlayUIManager: No problem selected.");
            return;
        }

        GameConfig.SelectedProblem = selectedProblem;
        GameConfig.SelectedMinutes = GetSelectedMinutesSafe();
        GameConfig.SelectedTotalQuestions = GetSelectedQuestionsSafe();

        SceneManager.LoadScene("Game");
    }
}
