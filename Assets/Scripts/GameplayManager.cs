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

    int currentQuestion = -1;
    int currentLives = 3;
    int currentScore = 0;
    float currentTimeSeconds = 0;

    bool gameplayInitialized = false;

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
        InitializeGameplay();
    }

    void InitializeGameplay()
    {
        currentQuestion = -1;
        currentScore = 0;
        currentLives = 3;
        currentTimeSeconds = questionSet.totalTimeSeconds;
        ResetLives();
        UpdateTimerUI();
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


    private void Update()
    {
        if (!gameplayInitialized) return;
        currentTimeSeconds -= Time.deltaTime;
        UpdateTimerUI();
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
        Transform lostLife = livesHolder.GetChild(currentLives);
        lostLife.DOShakeScale(0.5f).OnComplete(() =>
        {

            lostLife.DOScale(1.1f, 0.15f).SetLoops(2, LoopType.Yoyo);

            lostLife.GetComponent<Image>().sprite = liveOff;


        });
    }

    public void ShowNextQuestion()
    {
        currentQuestion++;
        TablesManager.Instance.AddQuestion(questionSet.data[currentQuestion]);
    }

    public void SelectOption(TMP_Text selectedOption)
    {
        if(selectedOption.text == questionSet.data[currentQuestion].answer.ToString())
        {
            currentScore++;
        }else
        {
            LoseLife();
        }

        TablesManager.Instance.DisplayOptionsFeedback(selectedOption.text, questionSet.data[currentQuestion].answer.ToString());

    }
}
