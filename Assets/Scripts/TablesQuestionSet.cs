using System;
using System.Collections.Generic;

namespace Eduzo.Games.Tables.Data
{
    [Serializable]
    public class TablesQuestionSet
    {
        [Serializable]
        public class QuestionData
        {
            public string questionText;
            public int answer;
        }

        public List<QuestionData> data = new List<QuestionData>();
    }
}