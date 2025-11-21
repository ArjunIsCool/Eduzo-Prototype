using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestionSet", menuName = "Scriptable Objects/QuestionSet")]
public class QuestionSet : ScriptableObject
{
    [Serializable]
    public class QuestionData
    {
        public string questionText;
        public int answer;
        public List<int> wrongOptions;
    }

    public List<QuestionData> data;
}
