using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class UserDataManager
{
    static readonly string dirPath = Application.persistentDataPath + "/UserResults";
    static readonly string filePath = dirPath + "/results.json";

    [Serializable]
    public class UserGameHistory
    {
        public List<UserData> data = new List<UserData>();
    }

    public static void SaveUserData(UserData userData)
    {
        UserGameHistory userGameHistory;

        if(!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        if(File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            userGameHistory = JsonUtility.FromJson<UserGameHistory>(json);
        } else
        {
            userGameHistory = new UserGameHistory();
        }

        userGameHistory.data.Add(userData);

        string outputJson = JsonUtility.ToJson(userGameHistory, true);
        File.WriteAllText(filePath, outputJson);

        Debug.Log($"Saved results successfully to {filePath}");
    }
}
