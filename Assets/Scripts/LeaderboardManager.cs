using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using TMPro; 
using UnityEngine.UI;
using System;
using System.Linq;

public class LeaderboardManager : MonoBehaviour
{
    private const string submitScoreURL = "https://tsuneflip.keehar.net/submitScore";
    private const string getLeaderboardURL = "https://tsuneflip.keehar.net/getLeaderboard/";

    public GameObject leaderboardEntryTemplate;
    public Transform leaderboardContainer;

    public IEnumerator SubmitScore(string levelId, string username, float rawTime, string formattedTime)
    {
        username = FilterProfanity(username);  // Apply profanity filter

        // Truncate the username if it's longer than 7 characters
        if (username.Length > 7)
        {
            username = username.Substring(0, 7);
        }

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
            // Parse JSON response and populate leaderboard UI
            string jsonResponse = www.downloadHandler.text;
            LeaderboardEntry[] leaderboardEntries = JsonHelper.FromJson<LeaderboardEntry>(jsonResponse);
            PopulateLeaderboard(leaderboardEntries);
        }
    }

    private void PopulateLeaderboard(LeaderboardEntry[] entries)
    {
        // Clear existing leaderboard entries
        foreach (Transform child in leaderboardContainer)
        {
            Destroy(child.gameObject);
        }

        // Sort the entries by raw_time in ascending order
        var sortedEntries = entries.OrderBy(entry => entry.raw_time).Take(5).ToArray();

        // Populate the leaderboard with the top 5 entries
        foreach (var entry in sortedEntries)
        {
            GameObject newEntry = Instantiate(leaderboardEntryTemplate, leaderboardContainer);
            newEntry.SetActive(true);

            TMP_Text[] texts = newEntry.GetComponentsInChildren<TMP_Text>(true);

            if (texts.Length >= 2)
            {
                texts[0].text = entry.username;
                texts[1].text = entry.formatted_time;  // Display formatted time

                foreach (var text in texts)
                {
                    text.enabled = true;
                }
            }
            else
            {
                Debug.LogError("LeaderboardEntryTemplate must have at least two TMP_Text components.");
            }
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

    [System.Serializable]
    public class LeaderboardEntry
    {
        public string username;
        public float raw_time;  // Assuming this exists
        public string formatted_time;  // Add this property if it doesn't exist
    }

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            string wrappedJson = "{\"Items\":" + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrappedJson);
            return wrapper.Items;
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
}