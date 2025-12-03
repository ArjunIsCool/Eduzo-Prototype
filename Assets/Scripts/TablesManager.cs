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

    public RectTransform optionsHolder;
    public List<Option> optionsObjs;
    Vector3 optionsHolderStartPos;

    public RectTransform tableAnswerMiddlePos;

    [Header("SOUNDS")]
    public AudioSource optionsWooshIn;
    public AudioSource optionsWooshOut;

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

        optionsHolderStartPos = optionsHolder.anchoredPosition;
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

        foreach (Option option in optionsObjs)
        {
            option.ResetOptionBtn();
        }

        HideOptions();
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

        int optionIndex = 0;
        foreach (Option optionObj in optionsObjs)
        {
            optionObj.optionText.text = options[optionIndex].ToString();
            optionIndex++;
        }

        ShowOptions();
    }

    public void ShowOptions()
    {
        optionsHolder.DOAnchorPos(optionsHolderStartPos, 1f).SetEase(Ease.OutBounce);
        optionsWooshIn.Play();
    }

    public void HideOptions()
    {
        optionsHolder.DOAnchorPos(new Vector3(0f, -1000f, 0f), 1f).SetEase(Ease.OutBounce);
        optionsWooshOut.Play();

        foreach (Option optionObj in optionsObjs)
        {
            optionObj.ResetOptionBtn();
        }
    }


    public IEnumerator DisplayOptionsFeedback(string chosenOption, string correctAnswer)
    {
        foreach (Option optionObj in optionsObjs)
        {
            optionObj.optionButton.GetComponent<Button>().enabled = false;
        }


        Option correctOptionObj = null;
        if (chosenOption == correctAnswer)
        {
            foreach(Option optionObj in optionsObjs)
            {
                if (optionObj.optionText.text == chosenOption)
                {
                    optionObj.ShowCorrectUI();
                    optionObj.correctVFX.Play();
                    correctOptionObj = optionObj;
                }
            }
        } else
        {
            foreach (Option optionObj in optionsObjs)
            {
                if (optionObj.optionText.text == chosenOption)
                {
                    optionObj.ShowWrongUI();
                }
                if(optionObj.optionText.text == correctAnswer)
                {
                    optionObj.ShowCorrectUI();
                    correctOptionObj = optionObj;
                }

            }
        }

        yield return new WaitForSeconds(1f);


        yield return StartCoroutine(correctOptionObj.MoveTowardsTable(GetLocalPosIn(tableAnswerMiddlePos, (RectTransform)correctOptionObj.optionButton.parent)));
        

        AddAnswer(correctAnswer);

        HideOptions();

        yield return new WaitForSeconds(1f);

        OnFeedbackComplete?.Invoke();
    }

    public static Vector2 GetLocalPosIn(RectTransform target, RectTransform relativeTo)
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, target.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(relativeTo, screenPos, null, out Vector2 localPoint);
        return localPoint;
    }


    public void AddAnswer(string answer)
    {
        GameObject answerInst = Instantiate(answerObj, answersHolder);

        answerInst.GetComponentInChildren<TMP_Text>().text = answer;

        answerInst.transform.GetChild(0).GetComponent<RectTransform>().DOPunchScale(Vector3.one * 1.5f, 0.5f).SetEase(Ease.OutFlash);
    }
}
