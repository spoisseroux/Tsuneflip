using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

public class LeaderboardManager : MonoBehaviour
{
    private const string submitScoreURL = "https://tsuneflip.keehar.net/submitScore";
    private const string getLeaderboardURL = "https://tsuneflip.keehar.net/getLeaderboard/";

    public IEnumerator SubmitScore(string levelId, string username, float rawTime, string formattedTime)
    {
        username = FilterProfanity(username);  // Apply profanity filter

        // Create a JSON object with the score data
        string json = JsonUtility.ToJson(new ScoreData(levelId, username, rawTime, formattedTime));

        UnityWebRequest www = new UnityWebRequest(submitScoreURL, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to submit score: " + www.error);
        }
        else
        {
            Debug.Log("Score submitted successfully!");
        }
    }

    public IEnumerator GetLeaderboard(string levelId)
    {
        UnityWebRequest www = UnityWebRequest.Get(getLeaderboardURL + levelId);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to get leaderboard: " + www.error);
        }
        else
        {
            Debug.Log("Leaderboard: " + www.downloadHandler.text);
        }
    }

    private string FilterProfanity(string input)
    {
        string[] bannedWords = { "badword1", "badword2" };  // Add more words as needed
        foreach (string word in bannedWords)
        {
            input = Regex.Replace(input, word, "****", RegexOptions.IgnoreCase);
        }
        return input;
    }

    // Class to hold the score data
    [System.Serializable]
    public class ScoreData
    {
        public string level_id;
        public string username;
        public float raw_time;
        public string formatted_time;

        public ScoreData(string levelId, string username, float rawTime, string formattedTime)
        {
            this.level_id = levelId;
            this.username = username;
            this.raw_time = rawTime;
            this.formatted_time = formattedTime;
        }
    }
}