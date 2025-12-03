using System;
using System.Collections;
using System.Runtime.Serialization;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    public QuestionSet questionSet;

    [Header("SOUNDS")]
    public AudioSource correctAnswerSFX;
    public AudioSource wrongAnswerSFX;
    public AudioSource gameEndedSFX;

    int currentQuestion = -1;
    int currentLives = 3;
    int currentScore = 0;
    int currentTimeSeconds = 0;

    bool gameOver = false; //If we have lost game ONLY
    enum GameOverReason { OUT_OF_LIVES, TIME_UP};

    GameOverReason gameOverReason;

    enum GameplayState { INITIALIZING, SHOWING_QUESTION, SHOWING_FEEDBACK, GAME_ENDED};

    GameplayState currentGameplayState;

    public Action OnGameInitialized;

    public static GameplayManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        TablesManager.Instance.OnFeedbackComplete += HandleFeedbackCompleted;
        StartCoroutine(InitializeGameplay());
    }

    private void OnDisable()
    {
        TablesManager.Instance.OnFeedbackComplete -= HandleFeedbackCompleted;
        StopAllCoroutines();
    }

    IEnumerator InitializeGameplay()
    {
        gameOver = false;
        currentGameplayState = GameplayState.INITIALIZING;
        currentQuestion = -1;
        currentScore = 0;
        currentLives = 3;
        currentTimeSeconds = questionSet.totalTimeSeconds;

        OnGameInitialized?.Invoke();

        yield return new WaitForSeconds(1f);

        ShowNextQuestion();
        StartCoroutine(RunGameplayTimer());
    }


    IEnumerator RunGameplayTimer()
    {
        while (!gameOver)
        {
            yield return new WaitForSeconds(1f);
            currentTimeSeconds --;

            if(currentTimeSeconds <= 0f)
            {
                currentTimeSeconds = 0;
                gameOver = true;
                gameOverReason = GameOverReason.TIME_UP;
            }

            GameUIManager.Instance.UpdateTimerUI(currentTimeSeconds);
        }
    }


    void LoseLife()
    {
        currentLives--;

        if(currentLives <= 0)
        {
            currentLives = 0;
            gameOver = true;
            gameOverReason = GameOverReason.OUT_OF_LIVES;
        }

        GameUIManager.Instance.UpdateLivesUIOnLostLife(currentLives);
    }

    void HandleFeedbackCompleted()
    {
        if (gameOver)
        {
            EndGameplay();
        } else
        {
            if (currentQuestion >= questionSet.data.Count - 1)
            {
                EndGameplay();
            } else
            {
                ShowNextQuestion();
            }
        }
    }

    public void ShowNextQuestion()
    {
        currentGameplayState = GameplayState.SHOWING_QUESTION;

        currentQuestion++;
        TablesManager.Instance.AddQuestion(questionSet.data[currentQuestion]);
    }

    public void EndGameplay()
    {
        StartCoroutine(EndGameplayRoutine());
    }

    public IEnumerator EndGameplayRoutine()
    {
        string endCardMessage = "";
        if (gameOver)
        {
            if (gameOverReason == GameOverReason.OUT_OF_LIVES)
            {
                endCardMessage = "OUT OF LIVES!";
            }
            else if (gameOverReason == GameOverReason.TIME_UP)
            {
                endCardMessage = "TIME'S UP!";
            }
        }
        else
        {
            endCardMessage = "COMPLETED!";
        }
        GameUIManager.Instance.ShowEndCard(endCardMessage);

        gameEndedSFX.Play();

        yield return new WaitForSeconds(2f);

        UIManager.Instance.ShowEndScreenUI();

        int totalScore = currentScore * currentLives;
        int maxPossibleScore = questionSet.data.Count * 3;

        EndUIManager.Instance.UpdateEndScreenInfo(!gameOver, totalScore, maxPossibleScore);
    }

    public void SelectOption(TMP_Text selectedOption)
    {
        currentGameplayState = GameplayState.SHOWING_FEEDBACK;

        if (selectedOption.text == questionSet.data[currentQuestion].answer.ToString())
        {
            currentScore++;
            correctAnswerSFX.Play();
        }else
        {
            LoseLife();
            wrongAnswerSFX.Play();
        }

        StartCoroutine(TablesManager.Instance.DisplayOptionsFeedback(selectedOption.text, questionSet.data[currentQuestion].answer.ToString()));
    }
}
