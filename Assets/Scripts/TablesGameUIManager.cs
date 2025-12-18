using System.Collections;
using DG.Tweening;
using Eduzo.Games.Tables.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Eduzo.Games.Tables.UI
{
    public class TablesGameUIManager : MonoBehaviour
    {
        public GameObject gameHUD;
        public TMP_Text timerText;

        public Transform livesHolder;
        public Sprite liveOn;
        public Sprite liveOff;

        public TablesCard answerCard;
        public TablesCard gameEndedCard;

        public TablesFeedbackVFX correctVFX;
        public TablesFeedbackVFX wrongVFX;

        public GameObject correctAnswerConfetti;

        public static TablesGameUIManager Instance;

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
            TablesGameplayManager.Instance.OnTablesGameInitialized += ResetLives;
        }

        private void OnDestroy()
        {
            TablesGameplayManager.Instance.OnTablesGameInitialized -= ResetLives;
        }

        public void ToggleHUD(bool active)
        {
            gameHUD.SetActive(active);
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

        public void PlayCorrectAnswerConfetti()
        {
            correctAnswerConfetti.SetActive(true);
            StartCoroutine(DisableCorrectAnswerConfetti());
        }

        IEnumerator DisableCorrectAnswerConfetti()
        {
            yield return new WaitForSeconds(3f);
            correctAnswerConfetti.SetActive(false);
        }

    }
}