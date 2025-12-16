using System;
using System.Collections.Generic;

namespace Eduzo.Games.Tables.Data
{
    [Serializable]
    public class TablesUserData
    {
        public string gameMode; //Practice or test mode
        public int totalQuestions; //Total no of questions for the session, including unattempted
        public int correctAnswers; //No of correctly answered questions
        public int wrongAnswers; //No of wrongly answered questions

        public double scorePercentage; //Percentage of correct / total questions
        public int remainingLives; //How many lives left
        public double totalTimeTaken; //Total game time used (seconds)

        [Serializable]
        public class QuestionSummary
        {
            public string question;
            public string answer;
            public bool attempted;
            public string userResponse; //Empty if not attempted
            public string result; //CORRECT or WRONG, empty if not attempted
            public double responseTime; //Time taken (seconds) to answer the question, 0 if not attempted
        }

        public List<QuestionSummary> questionsSummary = new List<QuestionSummary>();
    }
}