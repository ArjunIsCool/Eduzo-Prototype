using System;
using System.Collections.Generic;

[Serializable]
public class GameSettings
{
    public enum GameMode { PRACTICE, TEST};
    public GameMode gameMode; //Practice or test mode?

    public int tablesNo; //What tables are we learning?
    public List<int> questions = new List<int>();  //List of questions?

    public int totalTimeSeconds; //Time provided to player for the game session?
}
