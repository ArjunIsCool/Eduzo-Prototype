using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    public Toggle practiceModeToggle;

    public TMP_Dropdown tablesDropdown;
    public Button questionsDropdown;

    public TMP_Text noOfQuestionsSelectedText;
    public GameObject questionsDropdownPanel;

    public Transform questionsHolder;
    public GameObject questionItemPrefab;

    GameSettings chosenGameSettings;

    public static MenuUIManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        chosenGameSettings = new GameSettings();
        InitializeOptions();
    }

    void InitializeOptions()
    {
        practiceModeToggle.onValueChanged.AddListener(OnChangeGameMode);

        tablesDropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> optionDataList = new List<TMP_Dropdown.OptionData>();

        for (int i = 1; i <= GlobalConstants.TOTAL_TABLES; i++)
        {
            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
            optionData.text = $"{i} Tables";
            optionDataList.Add(optionData);
        }

        tablesDropdown.options = optionDataList;
        tablesDropdown.value = 0;

        tablesDropdown.onValueChanged.AddListener(OnSelectTables);

        OnSelectTables(0);

        questionsDropdown.onClick.AddListener(ToggleQuestionsDropdown);

    }

    public void OnChangeGameMode(bool isPracticeMode)
    {
        if(isPracticeMode)
        {
            chosenGameSettings.gameMode = GameSettings.GameMode.PRACTICE;
        } else
        {
            chosenGameSettings.gameMode = GameSettings.GameMode.TEST;
        }
    }

    public void OnSelectTables(int value)
    {
        chosenGameSettings.tablesNo = value + 1;
        chosenGameSettings.questions.Clear();
        UpdateQuestionsDropDown();
    }

    public void UpdateQuestionsDropDown()
    {
        noOfQuestionsSelectedText.text = "0 Selected";
        foreach(Transform questionItemObj in questionsHolder)
        {
            Destroy(questionItemObj.gameObject);
        }
        for (int i = 0; i < GlobalConstants.TOTAL_POSSIBLE_QUESTIONS; i++)
        {
            GameObject questionItem = Instantiate(questionItemPrefab, questionsHolder);
            questionItem.GetComponent<QuestionItem>().Initalize(chosenGameSettings.tablesNo, i + 1);
        }
    }

    public void ToggleQuestionsDropdown()
    {
        questionsDropdownPanel.gameObject.SetActive(!questionsDropdownPanel.gameObject.activeSelf);
    }

    public void OnSelectQuestion(int questionVal, bool use)
    {
        if (use && !chosenGameSettings.questions.Contains(questionVal))
        {
            chosenGameSettings.questions.Add(questionVal);
        }
        if(!use && chosenGameSettings.questions.Contains(questionVal))
        {
            chosenGameSettings.questions.Remove(questionVal);
        }

        noOfQuestionsSelectedText.text = $"{chosenGameSettings.questions.Count} Selected";
    }

    public GameSettings GetGameSettings()
    {
        return chosenGameSettings;
    }




}
