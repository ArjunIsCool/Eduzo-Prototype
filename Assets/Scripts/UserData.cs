using System;
using System.Collections.Generic;

[Serializable]
public class UserData
{
    public int totalQuestions; //Total no of questions for the session
    public int correctAnswers; //No of correctly answered questions
    public int wrongAnswers; //No of wrongly answered questions

    public double scorePercentage; //Percentage of correct / total questions
    public int remainingLives; //How many lives left
    public double totalTimeTaken; //Total game time used

    [Serializable]
    public class QuestionSummary
    {
        public string question;
        public string answer;
        public bool attempted;
        public string userResponse; //Empty if not attempted
        public string result; //CORRECT or WRONG, empty if not attempted
        public double responseTime; //Time taken to answer the question, 0 if not attempted
    }

    public List<QuestionSummary> questionsSummary;
}
