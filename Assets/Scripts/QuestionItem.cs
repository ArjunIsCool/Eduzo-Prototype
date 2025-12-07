using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionItem : MonoBehaviour
{
    public Toggle questionToggle;
    public TMP_Text questionText;
    public int questionVal;

    public void Initalize(int table, int val)
    {
        questionVal = val;
        questionText.text = $"{table} x {questionVal}";
        questionToggle.onValueChanged.AddListener(OnTogglePressed);
        questionToggle.isOn = false;
    }

    public void OnTogglePressed(bool active)
    {
        MenuUIManager.Instance.OnSelectQuestion(questionVal, active);
    }
}
