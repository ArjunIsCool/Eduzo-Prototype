using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class TablesManager : MonoBehaviour
{

    public Transform questionsHolder;
    public GameObject questionObj;

    public Transform answersHolder;
    public GameObject answerObj;

    public event Action OnFeedbackComplete;

    public static TablesManager Instance;

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

    private void Start()
    {
        GameplayManager.Instance.OnGameInitialized += ResetTable;
    }

    private void OnDestroy()
    {
        GameplayManager.Instance.OnGameInitialized -= ResetTable;
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

    public void AddQuestion(QuestionSet.QuestionData questionData)
    {
        GameObject questionInst = Instantiate(questionObj, questionsHolder);

        questionInst.GetComponentInChildren<TMP_Text>().text = questionData.questionText;

        questionInst.transform.GetChild(0).GetComponent<RectTransform>().DOPunchAnchorPos(new Vector2(0f, 50f), 1f).SetEase(Ease.OutQuad);

        List<int> options = new List<int>();
        options.Add(questionData.answer);
        options.AddRange(questionData.wrongOptions);

        for (int i = 0; i < options.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, options.Count - 1);
            (options[i], options[randomIndex]) = (options[randomIndex], options[i]);
        }
    }


    public IEnumerator DisplayOptionsFeedback(string chosenOption, string correctAnswer)
    {
        if (chosenOption == correctAnswer)
        {
            
        } else
        {
            
        }

        yield return new WaitForSeconds(1f);

        AddAnswer(correctAnswer);

        yield return new WaitForSeconds(1f);

        OnFeedbackComplete?.Invoke();
    }

    public void AddAnswer(string answer)
    {
        GameObject answerInst = Instantiate(answerObj, answersHolder);

        answerInst.GetComponentInChildren<TMP_Text>().text = answer;

        answerInst.transform.GetChild(0).GetComponent<RectTransform>().DOPunchScale(Vector3.one * 1.5f, 0.5f).SetEase(Ease.OutFlash);
    }
}
