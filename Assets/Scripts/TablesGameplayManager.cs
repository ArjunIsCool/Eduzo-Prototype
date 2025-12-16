using System;
using System.Collections;
using System.Runtime.Serialization;
using DG.Tweening;
using Eduzo.Games.Tables.Data;
using Eduzo.Games.Tables.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Eduzo.Games.Tables.Core
{
    public class TablesGameplayManager : MonoBehaviour
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
        enum GameOverReason { OUT_OF_LIVES, TIME_UP, COMPLETED };
        GameOverReason gameOverReason;

        TablesGameSettings currentGameSettings;
        TablesQuestionSet questionSet;
        TablesUserData currentUserData;

        Coroutine gameplayTimerRoutine;

        public event Action OnTablesGameInitialized;

        public static TablesGameplayManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            TablesTableUI.Instance.OnTablesFeedbackComplete += HandleFeedbackCompleted;
            qrController.onQRScanFinished.AddListener(OnScannedAnswer);
        }

        private void OnDestroy()
        {
            TablesTableUI.Instance.OnTablesFeedbackComplete -= HandleFeedbackCompleted;
            qrController.onQRScanFinished.RemoveListener(OnScannedAnswer);
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }


        public void InitializeGameplay(TablesGameSettings gameSettings)
        {
            currentGameSettings = gameSettings;
            currentUserData = new TablesUserData();
            gameOver = false;
            gameOverReason = GameOverReason.OUT_OF_LIVES; //Ensure it isnt marked as COMPLETED
            currentQuestion = -1;
            currentScore = 0;
            currentLives = 3;
            currentTimeSeconds = currentGameSettings.totalTimeSeconds;
            timeSinceLastQuestion = 0;

            SetupQuestionSet();

            OnTablesGameInitialized?.Invoke();

            if (currentGameSettings.gameMode == TablesGameSettings.GameMode.PRACTICE)
            {
                TablesGameUIManager.Instance.ToggleHUD(false);
            }
            else
            {
                TablesGameUIManager.Instance.ToggleHUD(true);
            }

            ShowNextQuestion();
        }

        void SetupQuestionSet()
        {
            questionSet = new TablesQuestionSet();
            foreach (int question in currentGameSettings.questions)
            {
                TablesQuestionSet.QuestionData questionData = new TablesQuestionSet.QuestionData();
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
                currentTimeSeconds--;
                timeSinceLastQuestion++;

                if (currentTimeSeconds <= 0f && currentGameSettings.gameMode == TablesGameSettings.GameMode.TEST)
                {
                    currentTimeSeconds = 0;
                    gameOver = true;
                    gameOverReason = GameOverReason.TIME_UP;
                }

                if (currentGameSettings.gameMode == TablesGameSettings.GameMode.TEST)
                {
                    TablesGameUIManager.Instance.UpdateTimerUI(currentTimeSeconds);
                }
            }
        }


        void LoseLife()
        {
            currentLives--;

            if (currentLives <= 0)
            {
                currentLives = 0;
                gameOver = true;
                gameOverReason = GameOverReason.OUT_OF_LIVES;
            }

            TablesGameUIManager.Instance.UpdateLivesUIOnLostLife(currentLives);
        }

        void HandleFeedbackCompleted()
        {
            if (currentQuestion >= questionSet.data.Count - 1) //This was the last question, so we complete the game
            {
                gameOver = true;
                gameOverReason = GameOverReason.COMPLETED;
            }

            if (gameOver)
            {
                EndGameplay();
            }
            else
            {
                ShowNextQuestion();
            }
        }

        public void ShowNextQuestion()
        {
            currentQuestion++;
            TablesGameUIManager.Instance.questionCard.DisplayMessage($"What is {questionSet.data[currentQuestion].questionText}?", -1f);
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
                    TablesQuestionSet.QuestionData questionData = questionSet.data[i];
                    TablesUserData.QuestionSummary questionSummary = new TablesUserData.QuestionSummary();

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
            switch (gameOverReason)
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
            TablesGameUIManager.Instance.gameEndedCard.DisplayMessage(endCardMessage, -1f);

            gameEndedSFX.Play();

            yield return new WaitForSeconds(2f);

            SaveUserDataToJSON();

            TablesUIManager.Instance.ShowEndScreenUI();

            int scorePercentage = (int)(Math.Round((double)currentScore / questionSet.data.Count, 2) * 100);

            TablesEndUIManager.Instance.UpdateEndScreenInfo(scorePercentage);
        }

        void AddQuestionSummaryData(string chosenAnswer, bool isCorrect)
        {
            if (isCorrect)
            {
                currentUserData.correctAnswers++;
            }
            else
            {
                currentUserData.wrongAnswers++;
            }

            TablesUserData.QuestionSummary questionSummary = new TablesUserData.QuestionSummary();

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
            currentUserData.gameMode = currentGameSettings.gameMode == TablesGameSettings.GameMode.PRACTICE ? "PRACTICE" : "TEST";
            currentUserData.totalQuestions = currentGameSettings.questions.Count;
            currentUserData.scorePercentage = Math.Round((double)currentScore / questionSet.data.Count, 2) * 100;
            currentUserData.remainingLives = currentLives;
            currentUserData.totalTimeTaken = currentGameSettings.totalTimeSeconds - currentTimeSeconds;

            TablesUserDataManager.SaveUserData(currentUserData);
        }

        public void OnScannedAnswer(string answer)
        {
            Debug.Log($"Scanned Answer: {answer}");

            if (!int.TryParse(answer, out int result))
            {
                Debug.Log("Not a valid answer type, ignoring..");
                qrController.Reset();
                return;
            }

            cameraShutterSound.Play();
            StopCoroutine(gameplayTimerRoutine);
            TablesGameUIManager.Instance.questionCard.CloseImmediate();
            cameraPreview.SetActive(false);

            bool isCorrect = false;

            if (result == questionSet.data[currentQuestion].answer)
            {
                isCorrect = true;
                TablesGameUIManager.Instance.correctCard.DisplayMessage("CORRECT!", 2f);
                TablesGameUIManager.Instance.PlayCorrectAnswerConfetti();
                currentScore++;
                correctAnswerSFX.Play();
            }
            else
            {
                isCorrect = false;
                TablesGameUIManager.Instance.wrongCard.DisplayMessage("WRONG!", 2f);
                if (currentGameSettings.gameMode == TablesGameSettings.GameMode.TEST)
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

            TablesGameUIManager.Instance.answerCard.DisplayMessage($"{question} = {answer}", 2f);

            yield return new WaitForSeconds(3f);

            StartCoroutine(TablesTableUI.Instance.DisplayAnswerFeedback(question, answer));
        }
    }
}