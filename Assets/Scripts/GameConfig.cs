public static class GameConfig  // use to access data across scenes
{

    // Options are: Add, Subtract, Multiply, Divide
    public static string SelectedProblem = "Add";  // change here for default
    public static int SelectedMinutes = 1;              // just a placeholder (ignore this)

    public static int SelectedTotalQuestions = 5;       // just a placeholder (Game writes this)
        
    public static int ProblemsSolved = 0;               // just a placeholder (Game writes this)
    public static int TotalQuestions = 0;               // just a placeholder (Game writes this)
    public static float ElapsedSeconds = 0f;            // just a placeholder (Game writes this)
    public static bool Passed = false;                  // just a placeholder (Game writes this)
}
