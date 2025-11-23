using System.Threading;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    public QuestionSet questionSet;

    public TMP_Text timerText;

    public Transform livesHolder;
    public Sprite liveOn;
    public Sprite liveOff;

    public EndCard endCard;

    public Button menuBtn;

    [Header("SOUNDS")]
    public AudioSource correctAnswerSFX;
    public AudioSource wrongAnswerSFX;
    public AudioSource gameEndedSFX;

    int currentQuestion = -1;
    int currentLives = 3;
    int currentScore = 0;
    float currentTimeSeconds = 0;

    bool gameplayInitialized = false;
    bool gameOver = false; //If we have lost game ONLY
    enum GameOverReason { OUT_OF_LIVES, TIME_UP};

    GameOverReason gameOverReason;

    CancellationTokenSource gameplayCTS;

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
        InitializeGameplay();
    }

    private void OnDisable()
    {
        gameplayCTS.Cancel();
    }

    async void InitializeGameplay()
    {
        gameplayCTS = new CancellationTokenSource();

        gameOver = false;

        menuBtn.gameObject.SetActive(true);

        currentQuestion = -1;
        currentScore = 0;
        currentLives = 3;
        currentTimeSeconds = questionSet.totalTimeSeconds;
        ResetLives();
        UpdateTimerUI();

        TablesManager.Instance.ResetTable();

        await Task.Delay(1000);

        ShowNextQuestion();

        gameplayInitialized = true;
    }

    void ResetLives()
    {
        foreach(Transform life in livesHolder)
        {
            life.GetComponent<Image>().sprite = liveOn;
        }
    }


    public bool IsGameOver() { return gameOver; }


    private void Update()
    {
        if (!gameplayInitialized) return;
        currentTimeSeconds -= Time.deltaTime;
        UpdateTimerUI();

        if (currentTimeSeconds <= 0f)
        {
            currentTimeSeconds = 0f;
            UpdateTimerUI();

            gameOver = true;
            gameplayInitialized = false;

            if (!TablesManager.Instance.IsGameplayInProcess())
            {
                gameOverReason = GameOverReason.TIME_UP;
                EndGameplay();
            }
        }
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(currentTimeSeconds / 60);
        int seconds = Mathf.FloorToInt(currentTimeSeconds % 60);

        if (seconds < 10)
        {
            timerText.text = $"0{minutes}:0{seconds}";
        } else
        {
            timerText.text = $"0{minutes}:{seconds}";
        }
    }

    void LoseLife()
    {
        currentLives--;

        if(currentLives <= 0)
        {
            gameOver = true;
            gameplayInitialized = false;

            if(!TablesManager.Instance.IsGameplayInProcess())
            {
                gameOverReason = GameOverReason.OUT_OF_LIVES;
                EndGameplay();
            }
        }

        Transform lostLife = livesHolder.GetChild(currentLives);
        lostLife.DOShakeScale(0.5f).OnComplete(() =>
        {

            lostLife.DOScale(1.1f, 0.15f).SetLoops(2, LoopType.Yoyo);

            lostLife.GetComponent<Image>().sprite = liveOff;


        });
    }

    public void ShowNextQuestion()
    {
        if (gameOver) return;
        if(currentQuestion == questionSet.data.Count - 1)
        {
            gameplayInitialized = false;
            EndGameplay();

            return;
        }

        currentQuestion++;
        TablesManager.Instance.AddQuestion(questionSet.data[currentQuestion]);
    }

    public async void EndGameplay()
    {
        menuBtn.gameObject.SetActive(false);
        if (gameOver)
        {
            if (gameOverReason == GameOverReason.OUT_OF_LIVES)
            {
                endCard.DisplayMessage("OUT OF LIVES!");
            } else if (gameOverReason == GameOverReason.TIME_UP)
            {
                endCard.DisplayMessage("TIME'S UP!");
            }

        }else
        {
            endCard.DisplayMessage("COMPLETED!");
        }

        gameEndedSFX.Play();

        await Task.Delay(2000);

        UIManager.Instance.ShowEndScreenUI();

        int totalScore = currentScore * currentLives;
        int maxPossibleScore = questionSet.data.Count * 3;

        EndUIManager.Instance.UpdateEndScreenInfo(!gameOver, totalScore, maxPossibleScore);
    }

    public void SelectOption(TMP_Text selectedOption)
    {
        TablesManager.Instance.DisplayOptionsFeedback(selectedOption.text, questionSet.data[currentQuestion].answer.ToString(), gameplayCTS.Token);


        if (selectedOption.text == questionSet.data[currentQuestion].answer.ToString())
        {
            currentScore++;
            correctAnswerSFX.Play();
        }else
        {
            LoseLife();
            wrongAnswerSFX.Play();
        }

    }
}
