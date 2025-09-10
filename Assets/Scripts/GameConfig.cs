public static class GameConfig
{
    public static string SelectedProblem = "Add"; // already there
    public static int SelectedMinutes = 1;        // already there

    public static int SelectedTotalQuestions = 5; // NEW: from Intro dropdown

    // Results for End scene (you already have these)
    public static int ProblemsSolved = 0;
    public static int TotalQuestions = 5;
    public static float ElapsedSeconds = 0f;
    public static bool Passed = false;
}
