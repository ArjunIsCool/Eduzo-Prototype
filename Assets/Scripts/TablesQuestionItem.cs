using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Eduzo.Games.Tables.UI
{
    public class TablesQuestionItem : MonoBehaviour
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
            TablesMenuUIManager.Instance.OnSelectQuestion(questionVal, active);
        }
    }
}