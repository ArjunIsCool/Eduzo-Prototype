using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public TMP_Text timerText;

    public Transform livesHolder;
    public Sprite liveOn;
    public Sprite liveOff;

    public EndCard endCard;

    public static GameUIManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        GameplayManager.Instance.OnGameInitialized += ResetLives;
    }

    private void OnDisable()
    {
        GameplayManager.Instance.OnGameInitialized -= ResetLives;
    }

    public void ResetLives()
    {
        foreach (Transform life in livesHolder)
        {
            life.GetComponent<Image>().sprite = liveOn;
        }
    }

    public void UpdateLivesUIOnLostLife(int currentLives)
    {
        Transform lostLife = livesHolder.GetChild(currentLives);
        lostLife.DOShakeScale(0.5f).OnComplete(() =>
        {

            lostLife.DOScale(1.1f, 0.15f).SetLoops(2, LoopType.Yoyo);

            lostLife.GetComponent<Image>().sprite = liveOff;

        });
    }

    public void UpdateTimerUI(float timeSeconds)
    {
        int minutes = Mathf.FloorToInt(timeSeconds / 60);
        int seconds = Mathf.FloorToInt(timeSeconds % 60);

        if (seconds < 10)
        {
            timerText.text = $"0{minutes}:0{seconds}";
        }
        else
        {
            timerText.text = $"0{minutes}:{seconds}";
        }
    }

    public void ShowEndCard(string message)
    {
        endCard.DisplayMessage(message);
    }




}
