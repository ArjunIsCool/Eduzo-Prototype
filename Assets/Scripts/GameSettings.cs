using System.Collections.Generic;

public class GameSettings
{
    public enum GameMode { PRACTICE, TEST};
    public GameMode gameMode;

    public int tablesNo;
    public List<int> questions = new List<int>();
}
