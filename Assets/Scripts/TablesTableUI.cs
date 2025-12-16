using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using Eduzo.Games.Tables.Core;

namespace Eduzo.Games.Tables.UI
{
    public class TablesTableUI : MonoBehaviour
    {
        public int tableSize;
        public Transform questionsHolder;
        public GameObject questionObj;

        public Transform answersHolder;
        public GameObject answerObj;

        public event Action OnTablesFeedbackComplete;

        public static TablesTableUI Instance;

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
            TablesGameplayManager.Instance.OnTablesGameInitialized += ResetTable;
        }

        private void OnDestroy()
        {
            TablesGameplayManager.Instance.OnTablesGameInitialized -= ResetTable;
        }

        public void ResetTable()
        {
            foreach (Transform question in questionsHolder)
            {
                Destroy(question.gameObject);
            }
            foreach (Transform answer in answersHolder)
            {
                Destroy(answer.gameObject);
            }
        }

        public void ClearTableSmoothlyBeforeNextQuestion(string question)
        {
            Sequence sequence = DOTween.Sequence();
            foreach (Transform questionObj in questionsHolder)
            {
                sequence.Join(questionObj.DOScaleY(0f, 1f));
                RectTransform rectTransform = questionObj.GetComponent<RectTransform>();
                sequence.Join(rectTransform.DOSizeDelta(new Vector2(rectTransform.sizeDelta.x, 0f), 1f));
            }
            foreach (Transform answer in answersHolder)
            {
                sequence.Join(answer.DOScaleY(0f, 1f));
                RectTransform rectTransform = answer.GetComponent<RectTransform>();
                sequence.Join(rectTransform.DOSizeDelta(new Vector2(rectTransform.sizeDelta.x, 0f), 1f));
            }

            sequence.Play(); //Safety incase it doesnt play automatically

            sequence.onComplete += () => { ResetTable(); }; //Adding question after emptying table
        }

        public void AddQuestion(string question)
        {
            GameObject questionInst = Instantiate(questionObj, questionsHolder);

            questionInst.GetComponentInChildren<TMP_Text>().text = question;

            questionInst.transform.GetChild(0).GetComponent<RectTransform>().DOPunchAnchorPos(new Vector2(0f, 50f), 1f).SetEase(Ease.OutQuad);
        }


        public IEnumerator DisplayAnswerFeedback(string question, string correctAnswer)
        {
            if (questionsHolder.childCount >= tableSize)
            {
                ClearTableSmoothlyBeforeNextQuestion(question);
                yield return new WaitForSeconds(2f);
            }

            AddQuestion(question);

            yield return new WaitForSeconds(1f);

            AddAnswer(correctAnswer);

            yield return new WaitForSeconds(1f);

            OnTablesFeedbackComplete?.Invoke();
        }



        public void AddAnswer(string answer)
        {
            GameObject answerInst = Instantiate(answerObj, answersHolder);

            answerInst.GetComponentInChildren<TMP_Text>().text = answer;

            answerInst.transform.GetChild(0).GetComponent<RectTransform>().DOPunchScale(Vector3.one * 1.5f, 0.5f).SetEase(Ease.OutFlash);
        }
    }
}