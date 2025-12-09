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
    QuestionSet questionSet;

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
        currentTimeSeconds = currentGameSettings.totalTimeSeconds;

        SetupQuestionSet();

        OnGameInitialized?.Invoke();

        if (currentGameSettings.gameMode == GameSettings.GameMode.PRACTICE)
        {
            GameUIManager.Instance.ToggleHUD(false);
        }else
        {
            GameUIManager.Instance.ToggleHUD(true);
        }

        ShowNextQuestion();
    }

    void SetupQuestionSet()
    {
        questionSet = new QuestionSet();
        foreach (int question in currentGameSettings.questions)
        {
            QuestionSet.QuestionData questionData = new QuestionSet.QuestionData();
            questionData.questionText = $"{currentGameSettings.tablesNo} x {question}";
            questionData.answer = currentGameSettings.tablesNo * question;

            questionSet.data.Add(questionData);
        }
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
        if(currentQuestion >= questionSet.data.Count - 1) //This was the last question, so we complete the game
        {
            gameOverReason = GameOverReason.COMPLETED;
        }

        if (gameOver || gameOverReason == GameOverReason.COMPLETED)
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
        GameUIManager.Instance.questionCard.DisplayMessage($"What is {questionSet.data[currentQuestion].questionText}?", -1f);
        //TablesManager.Instance.AddQuestion(questionSet.data[currentQuestion]);

        if (currentGameSettings.gameMode == GameSettings.GameMode.TEST)
        {
            StartCoroutine(RunGameplayTimer());
        }

        cameraPreview.SetActive(true);
        qrController.Reset();
    }

    public void EndGameplay()
    {
        gameOver = true;
        StopCoroutine(RunGameplayTimer());
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
        GameUIManager.Instance.gameEndedCard.DisplayMessage(endCardMessage, -1f);

        gameEndedSFX.Play();

        yield return new WaitForSeconds(2f);

        UIManager.Instance.ShowEndScreenUI();


        double scorePercentage = Math.Round((double)currentScore / questionSet.data.Count, 2);

        EndUIManager.Instance.UpdateEndScreenInfo(!gameOver, scorePercentage);
    }

    public void OnScannedAnswer(string answer)
    {
        Debug.Log($"Scanned Answer: {answer}");

        if(!int.TryParse(answer, out int result))
        {
            Debug.Log("Not a valid answer type, ignoring..");
            qrController.Reset();
            return;
        }

        StopCoroutine(RunGameplayTimer());
        GameUIManager.Instance.questionCard.CloseImmediate();
        cameraPreview.SetActive(false);

        bool isCorrect = false;

        if (result == questionSet.data[currentQuestion].answer)
        {
            isCorrect = true;
            GameUIManager.Instance.correctCard.DisplayMessage("CORRECT!", 2f);
            currentScore++;
            correctAnswerSFX.Play();
        }
        else
        {
            isCorrect = false;
            GameUIManager.Instance.wrongCard.DisplayMessage("WRONG!", 2f);
            if (currentGameSettings.gameMode == GameSettings.GameMode.TEST)
            {
                LoseLife();
            }
            wrongAnswerSFX.Play();
        }

        StartCoroutine(DisplayAnswerFeedback(isCorrect));
    }

    IEnumerator DisplayAnswerFeedback(bool answeredCorrectly)
    {
        string question = questionSet.data[currentQuestion].questionText;
        string answer = questionSet.data[currentQuestion].answer.ToString();

        yield return new WaitForSeconds(3f);

        GameUIManager.Instance.answerCard.DisplayMessage($"{question} = {answer}", 2f);

        yield return new WaitForSeconds(3f);

        StartCoroutine(TablesManager.Instance.DisplayAnswerFeedback(question, answer));
    }
}
