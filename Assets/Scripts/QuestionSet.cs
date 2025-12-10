using System;
using System.Collections.Generic;

[Serializable]
public class QuestionSet
{
    [Serializable]
    public class QuestionData
    {
        public string questionText;
        public int answer;
    }

    public List<QuestionData> data = new List<QuestionData>();
}
