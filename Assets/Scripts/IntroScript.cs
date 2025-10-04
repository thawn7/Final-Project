using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class IntroScript : MonoBehaviour
{
    public Dropdown timerDropdown, questionsDropdown;
    public Button playButton, add, sub, multiply, divide;
    public Text DisplayText;

    string selectedProblem = "Add";
    readonly int[] minuteMap = { 1, 3, 5, 10 };
    readonly int[] questionMap = { 5, 10, 15, 20, 30 };

    void Start()
    {
        // default from GameConfig or Addition
        selectedProblem = string.IsNullOrEmpty(GameConfig.SelectedProblem) ? "Add" : GameConfig.SelectedProblem;
        DisplayText.text = GetLabel(selectedProblem);

        // quick listeners
        add.onClick.AddListener(() => SetProblem("Add"));
        sub.onClick.AddListener(() => SetProblem("Subtract"));
        multiply.onClick.AddListener(() => SetProblem("Multiply"));
        divide.onClick.AddListener(() => SetProblem("Divide"));

        if (timerDropdown) timerDropdown.value = 0;
        if (questionsDropdown) questionsDropdown.value = 0;
        if (playButton) playButton.interactable = true;
    }

    void SetProblem(string mode)
    {
        selectedProblem = mode;
        DisplayText.text = GetLabel(mode);
        GameConfig.SelectedProblem = mode;
    }

    string GetLabel(string mode)
    {
        switch (mode)
        {
            case "Add": return "Addition";
            case "Subtract": return "Subtraction";
            case "Multiply": return "Multiplication";
            case "Divide": return "Division";
            default: return GetLabel(GameConfig.SelectedProblem);
        }
    }


    public void StartGame()
    {
        GameConfig.SelectedProblem = selectedProblem;
        GameConfig.SelectedMinutes = minuteMap[Mathf.Clamp(timerDropdown.value, 0, minuteMap.Length - 1)];
        GameConfig.SelectedTotalQuestions = questionMap[Mathf.Clamp(questionsDropdown.value, 0, questionMap.Length - 1)];
        SceneManager.LoadScene("Game");
    }

    public void Exit()
    {
        Application.Quit();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
