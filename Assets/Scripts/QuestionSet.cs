using System;
using System.Collections.Generic;

public class QuestionSet
{
    [Serializable]
    public class QuestionData
    {
        public string questionText;
        public int answer;
        public List<int> wrongOptions = new List<int>();
    }

    public List<QuestionData> data = new List<QuestionData>();
}
