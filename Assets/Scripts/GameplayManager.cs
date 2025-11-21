using TMPro;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public QuestionSet questionSet;

    int currentQuestion = 0;

    int currentLives = 3;
    int currentScore = 0;

    private void Start()
    {
        InitializeGameplay();
    }

    void InitializeGameplay()
    {
        currentQuestion = 0;
        currentScore = 0;
        currentLives = 3;
        ShowNextQuestion();
    }

    public void ShowNextQuestion()
    {
        TablesManager.Instance.AddQuestion(questionSet.data[currentQuestion]);
    }

    public void SelectOption(TMP_Text selectedOption)
    {
        DisplayAnswer();
        currentQuestion++;
        ShowNextQuestion();
    }

    public void DisplayAnswer()
    {
        TablesManager.Instance.AddAnswer(questionSet.data[currentQuestion].answer.ToString());
    }
}
