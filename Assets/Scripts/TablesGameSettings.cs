using System;
using System.Collections.Generic;

namespace Eduzo.Games.Tables.Data
{
    [Serializable]
    public class TablesGameSettings
    {
        public enum GameMode { PRACTICE, TEST };
        public GameMode gameMode; //Practice or test mode?

        public int tablesNo; //What tables are we learning?
        public List<int> questions = new List<int>();  //List of questions?

        public int totalTimeSeconds; //Time provided to player for the game session?

        public TablesGameSettings() { } //Default constructor

        public TablesGameSettings(TablesGameSettings other) //Copy constructor
        {
            gameMode = other.gameMode;
            tablesNo = other.tablesNo;
            questions = new List<int>(other.questions);
            totalTimeSeconds = other.totalTimeSeconds;
        }
    }
}