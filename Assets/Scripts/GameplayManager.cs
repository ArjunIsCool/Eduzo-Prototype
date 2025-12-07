using System;
using System.Collections;
using System.Runtime.Serialization;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    public QuestionSet questionSet;
    public GameObject cameraPreview;
    public QRCodeDecodeController qrController;

    [Header("SOUNDS")]
    public AudioSource correctAnswerSFX;
    public AudioSource wrongAnswerSFX;
    public AudioSource gameEndedSFX;

    int currentQuestion = -1;
    int currentLives = 3;
    int currentScore = 0;
    int currentTimeSeconds = 0;

    bool gameOver = false;
    enum GameOverReason { OUT_OF_LIVES, TIME_UP, COMPLETED};
    GameOverReason gameOverReason;

    GameSettings currentGameSettings;

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

    private void Start()
    {
        TablesManager.Instance.OnFeedbackComplete += HandleFeedbackCompleted;
        qrController.onQRScanFinished.AddListener(OnScannedAnswer);
    }

    private void OnDestroy()
    {
        TablesManager.Instance.OnFeedbackComplete -= HandleFeedbackCompleted;
        qrController.onQRScanFinished.RemoveListener(OnScannedAnswer);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }


    public void InitializeGameplay(GameSettings gameSettings)
    {
        currentGameSettings = gameSettings;
        gameOver = false;
        currentQuestion = -1;
        currentScore = 0;
        currentLives = 3;
        currentTimeSeconds = questionSet.totalTimeSeconds;

        OnGameInitialized?.Invoke();

        if (currentGameSettings.gameMode == GameSettings.GameMode.PRACTICE)
        {
            GameUIManager.Instance.ToggleHUD(false);
        }else
        {
            GameUIManager.Instance.ToggleHUD(true);
            StartCoroutine(RunGameplayTimer());
        }

        ShowNextQuestion();
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
        if (gameOver || (currentQuestion >= questionSet.data.Count - 1))
        {
            EndGameplay();
        } else
        {
            ShowNextQuestion();
        }
    }

    public void ShowNextQuestion()
    {
        currentQuestion++;
        TablesManager.Instance.AddQuestion(questionSet.data[currentQuestion]);

        cameraPreview.SetActive(true);
        qrController.Reset();
    }

    public void EndGameplay()
    {
        gameOver = true;
        StartCoroutine(EndGameplayRoutine());
    }

    public IEnumerator EndGameplayRoutine()
    {
        string endCardMessage = "";
        switch(gameOverReason)
        {
            case GameOverReason.OUT_OF_LIVES:
                endCardMessage = "OUT OF LIVES!";
                break;
            case GameOverReason.TIME_UP:
                endCardMessage = "TIME'S UP!";
                break;
            case GameOverReason.COMPLETED:
                endCardMessage = "COMPLETED!";
                break;
            default:
                endCardMessage = "COMPLETED!";
                break;
        }
        GameUIManager.Instance.ShowEndCard(endCardMessage);

        gameEndedSFX.Play();

        yield return new WaitForSeconds(2f);

        UIManager.Instance.ShowEndScreenUI();

        int totalScore = currentScore * currentLives;
        int maxPossibleScore = questionSet.data.Count * 3;

        EndUIManager.Instance.UpdateEndScreenInfo(!gameOver, totalScore, maxPossibleScore);
    }

    public void OnScannedAnswer(string answer)
    {
        Debug.Log($"Scanned Answer: {answer}");

        cameraPreview.SetActive(false);

        if (int.Parse(answer) == questionSet.data[currentQuestion].answer)
        {
            currentScore++;
            correctAnswerSFX.Play();
        }
        else
        {
            if (currentGameSettings.gameMode == GameSettings.GameMode.TEST)
            {
                LoseLife();
            }
            wrongAnswerSFX.Play();
        }

        StartCoroutine(TablesManager.Instance.DisplayOptionsFeedback(answer, questionSet.data[currentQuestion].answer.ToString()));
    }
}
