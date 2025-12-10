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
    public AudioSource cameraShutterSound;
    public AudioSource correctAnswerSFX;
    public AudioSource wrongAnswerSFX;
    public AudioSource gameEndedSFX;

    int currentQuestion = -1;
    int currentLives = 3;
    int currentScore = 0;
    int currentTimeSeconds = 0;
    int timeSinceLastQuestion = 0;

    bool gameOver = false;
    enum GameOverReason { OUT_OF_LIVES, TIME_UP, COMPLETED};
    GameOverReason gameOverReason;

    GameSettings currentGameSettings;
    QuestionSet questionSet;
    UserData currentUserData;

    Coroutine gameplayTimerRoutine;

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
        currentUserData = new UserData();
        gameOver = false;
        gameOverReason = GameOverReason.OUT_OF_LIVES; //Ensure it isnt marked as COMPLETED
        currentQuestion = -1;
        currentScore = 0;
        currentLives = 3;
        currentTimeSeconds = currentGameSettings.totalTimeSeconds;
        timeSinceLastQuestion = 0;

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

    //Timer is always run for purpose of stats, but behaviour depends on game mode
    IEnumerator RunGameplayTimer()
    {
        while (!gameOver)
        {
            yield return new WaitForSeconds(1f);
            currentTimeSeconds --;
            timeSinceLastQuestion ++;

            if(currentTimeSeconds <= 0f && currentGameSettings.gameMode == GameSettings.GameMode.TEST)
            {
                currentTimeSeconds = 0;
                gameOver = true;
                gameOverReason = GameOverReason.TIME_UP;
            }

            if (currentGameSettings.gameMode == GameSettings.GameMode.TEST)
            {
                GameUIManager.Instance.UpdateTimerUI(currentTimeSeconds);
            }
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
            gameOver = true;
            gameOverReason = GameOverReason.COMPLETED;
        }

        if (gameOver)
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

        timeSinceLastQuestion = 0;
        gameplayTimerRoutine = StartCoroutine(RunGameplayTimer());

        cameraPreview.SetActive(true);
        qrController.Reset();
    }

    public void EndGameplay()
    {
        StopCoroutine(gameplayTimerRoutine);

        //This is when the game ended due to out of lives or time, and not all questions were answered
        int unAnsweredQuestions = questionSet.data.Count - currentUserData.questionsSummary.Count;

        if (unAnsweredQuestions > 0)
        {
            for (int i = currentUserData.questionsSummary.Count; i < questionSet.data.Count; i++)
            {
                QuestionSet.QuestionData questionData = questionSet.data[i];
                UserData.QuestionSummary questionSummary = new UserData.QuestionSummary();

                questionSummary.question = questionData.questionText;
                questionSummary.answer = questionData.answer.ToString();
                questionSummary.attempted = false;

                currentUserData.questionsSummary.Add(questionSummary);
            }
        }


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

        SaveUserDataToJSON();

        UIManager.Instance.ShowEndScreenUI();

        int scorePercentage = (int)(Math.Round((double)currentScore / questionSet.data.Count, 2) * 100);

        EndUIManager.Instance.UpdateEndScreenInfo(scorePercentage);
    }

    void AddQuestionSummaryData(string chosenAnswer, bool isCorrect)
    {
        if(isCorrect)
        {
            currentUserData.correctAnswers++;
        } else
        {
            currentUserData.wrongAnswers++;
        }

        UserData.QuestionSummary questionSummary = new UserData.QuestionSummary();

        questionSummary.question = questionSet.data[currentQuestion].questionText;
        questionSummary.answer = questionSet.data[currentQuestion].answer.ToString();
        questionSummary.attempted = true;
        questionSummary.userResponse = chosenAnswer;
        questionSummary.result = isCorrect ? "CORRECT" : "WRONG";
        questionSummary.responseTime = timeSinceLastQuestion;

        currentUserData.questionsSummary.Add(questionSummary);
    }

    void SaveUserDataToJSON()
    {
        currentUserData.gameMode = currentGameSettings.gameMode == GameSettings.GameMode.PRACTICE ? "PRACTICE" : "TEST";
        currentUserData.totalQuestions = currentGameSettings.questions.Count;
        currentUserData.scorePercentage = Math.Round((double)currentScore / questionSet.data.Count, 2) * 100;
        currentUserData.remainingLives = currentLives;
        currentUserData.totalTimeTaken = currentGameSettings.totalTimeSeconds - currentTimeSeconds;

        UserDataManager.SaveUserData(currentUserData);
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

        cameraShutterSound.Play();
        StopCoroutine(gameplayTimerRoutine);
        GameUIManager.Instance.questionCard.CloseImmediate();
        cameraPreview.SetActive(false);

        bool isCorrect = false;

        if (result == questionSet.data[currentQuestion].answer)
        {
            isCorrect = true;
            GameUIManager.Instance.correctCard.DisplayMessage("CORRECT!", 2f);
            GameUIManager.Instance.PlayCorrectAnswerConfetti();
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

        AddQuestionSummaryData(answer, isCorrect);

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
