using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TablesManager : MonoBehaviour
{

    public Transform questionsHolder;
    public GameObject questionObj;

    public Transform answersHolder;
    public GameObject answerObj;

    public List<TMP_Text> optionsList;

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

    public void AddQuestion(QuestionSet.QuestionData questionData)
    {
        GameObject questionInst = Instantiate(questionObj, questionsHolder);

        questionInst.GetComponent<TMP_Text>().text = questionData.questionText;

        List<int> options = new List<int>();
        options.Add(questionData.answer);
        options.AddRange(questionData.wrongOptions);

        for (int i = 0; i < options.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, options.Count - 1);
            (options[i], options[randomIndex]) = (options[randomIndex], options[i]);
        }

        int optionIndex = 0;
        foreach (TMP_Text optionText in optionsList)
        {
            optionText.text = options[optionIndex].ToString();
            optionIndex++;
        }
    }

    public void AddAnswer(string answer)
    {
        GameObject answerInst = Instantiate(answerObj, answersHolder);

        answerInst.GetComponent<TMP_Text>().text = answer;
    }
}
