using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;

public class LeaderboardManager : MonoBehaviour
{
    private static readonly string[] ProfanityList = new string[]
    {
        "abuse", "abusive", "asshole", "arsehole", "bastard", "bitch", "biatch",
        "bollocks", "bullshit", "bs", "crap", "crappy", "cunt", "damn",
        "dammit", "dick", "douche", "douchebag", "faggot", "fag", "fucker",
        "fuck", "fucking", "motherfucker", "mf", "nigga", "nigger",
        "piss", "pissed", "prick", "shit", "shitty", "slut", "twat", "whore"
    };

    private FMOD.Studio.EventInstance playSubmitConfirm;
    private FMOD.Studio.EventInstance playSubmitDeny;

    private const string submitScoreURL = "https://tsuneflip.keehar.net/submitScore";
    private const string getLeaderboardURL = "https://tsuneflip.keehar.net/getLeaderboard/";

    public GameObject leaderboardEntryTemplate;
    public Transform leaderboardContainer;
    private List<GameObject> leaderboardEntries;  // List to hold the persistent entries
    private bool alreadySubmitted = false;
    private string secretKey;

    private void Start()
    {
        leaderboardEntries = new List<GameObject>();
        LoadSecretKey();  // Load the secret key from config.txt

        playSubmitConfirm = FMODUnity.RuntimeManager.CreateInstance("event:/SubmitConfirm");
        playSubmitConfirm.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        playSubmitDeny = FMODUnity.RuntimeManager.CreateInstance("event:/SubmitDeny");
        playSubmitDeny.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
    }

    private void LoadSecretKey()
    {
        string configPath = Path.Combine(Application.streamingAssetsPath, "config.txt");
        if (File.Exists(configPath))
        {
            string[] configLines = File.ReadAllLines(configPath);
            foreach (var line in configLines)
            {
                if (line.StartsWith("TSUNE_SECRET_KEY="))
                {
                    secretKey = line.Substring("TSUNE_SECRET_KEY=".Length);
                    Debug.Log("Loaded TSUNE_SECRET_KEY from config file.");
                }
            }
        }
        else
        {
            Debug.LogError("Config file not found.");
        }
    }

    public IEnumerator SubmitScore(string levelId, string username, float rawTime, string formattedTime)
    {
        if (!alreadySubmitted)
        {
            alreadySubmitted = true;

            //filter profanity in usernames
            username = FilterProfanity(username);

            // Fetch the current leaderboard for the level
            UnityWebRequest www = UnityWebRequest.Get(getLeaderboardURL + levelId);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to get leaderboard: " + www.error);
                yield break;  // Stop here if there's an error retrieving the leaderboard
            }

            // Parse the leaderboard data
            string jsonResponse = www.downloadHandler.text;
            LeaderboardEntry[] leaderboardEntries = JsonHelper.FromJson<LeaderboardEntry>(jsonResponse);

            // Sort the leaderboard by raw_time and take the top 5
            var top5Entries = leaderboardEntries.OrderBy(entry => entry.raw_time).Take(5).ToArray();

            // Check if the new rawTime is better than any of the top 5
            bool shouldSubmit = top5Entries.Length < 5 || rawTime < top5Entries.Last().raw_time;

            if (!shouldSubmit)
            {
                playSubmitDeny.start();
                playSubmitDeny.release();
                Debug.Log("Score not submitted: Raw time is not in the top 5.");
                yield break;  // Stop if the score shouldn't be submitted
            }

            username = FilterProfanity(username);  // Apply profanity filter

            // Truncate the username if it's longer than 7 characters
            if (username.Length > 7)
            {
                username = username.Substring(0, 7);
            }

            // Create a hash of the score data
            string hash = GenerateHash(levelId, username, rawTime.ToString(), formattedTime);

            // Create a JSON object with the score data and the hash
            ScoreData scoreData = new ScoreData(levelId, username, rawTime, formattedTime, hash);
            string json = JsonUtility.ToJson(scoreData);

            // Submit the score
            www = new UnityWebRequest(submitScoreURL, "POST");
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
                playSubmitConfirm.start();
                playSubmitConfirm.release();
                Debug.Log("Score submitted successfully!");
            }

            // Reload the leaderboard to show updated scores
            StartCoroutine(GetLeaderboard(levelId));
        }
        else
        {
            Debug.Log("Already submitted");
        }
    }

    private string GenerateHash(string levelId, string username, string rawTime, string formattedTime)
    {
        // Format rawTime to match the server-side precision
        string formattedRawTime = float.Parse(rawTime).ToString("F6"); // 6 decimal places

        string dataToHash = levelId + username + formattedRawTime + formattedTime + secretKey;
        Debug.Log("Data to hash (client): " + dataToHash);

        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(dataToHash));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            string hash = builder.ToString();
            Debug.Log("Generated hash (client): " + hash);
            return hash;
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
            string jsonResponse = www.downloadHandler.text;
            LeaderboardEntry[] leaderboardEntries = JsonHelper.FromJson<LeaderboardEntry>(jsonResponse);
            PopulateLeaderboard(leaderboardEntries);
        }
    }

    private void PopulateLeaderboard(LeaderboardEntry[] entries)
    {
        // Clear existing leaderboard entries
        foreach (var entryObject in leaderboardEntries)
        {
            Destroy(entryObject);
        }
        leaderboardEntries.Clear();

        // Sort the entries by raw_time in ascending order
        var sortedEntries = entries.OrderBy(entry => entry.raw_time).Take(5).ToArray();
        bool getFirst = false;

        // Populate the leaderboard with the top entries
        foreach (var entry in sortedEntries)
        {
            GameObject newEntry = Instantiate(leaderboardEntryTemplate, leaderboardContainer);
            newEntry.SetActive(true);

            TMP_Text[] texts = newEntry.GetComponentsInChildren<TMP_Text>(true);

            if (texts.Length >= 2)
            {
                texts[0].text = entry.username;
                texts[1].text = entry.formatted_time;  // Display formatted time

                // Rainbow effect for best entry
                if (getFirst == false)
                {
                    texts[0].GetComponent<RainbowTextEffect>().enabled = true;
                    texts[1].GetComponent<RainbowTextEffect>().enabled = true;
                    getFirst = true;
                }

                // Make sure the texts are enabled
                foreach (var text in texts)
                {
                    text.enabled = true;
                }
            }
            else
            {
                Debug.LogError("LeaderboardEntryTemplate must have at least two TMP_Text components.");
            }

            // Add the new entry to the list
            leaderboardEntries.Add(newEntry);
        }

        // Ensure we have 5 entries, even if some are empty
        for (int i = leaderboardEntries.Count; i < 5; i++)
        {
            GameObject newEntry = Instantiate(leaderboardEntryTemplate, leaderboardContainer);
            newEntry.SetActive(true);

            TMP_Text[] texts = newEntry.GetComponentsInChildren<TMP_Text>(true);

            if (texts.Length >= 2)
            {
                texts[0].text = "---";  // Placeholder for empty entries
                texts[1].text = "---";
            }

            leaderboardEntries.Add(newEntry);
        }
    }

    public static string FilterProfanity(string input)
    {
        foreach (var word in ProfanityList)
        {
            string pattern = @"\b" + Regex.Escape(word) + @"\b";
            input = Regex.Replace(input, pattern, "****", RegexOptions.IgnoreCase);
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
        public string hash;  // Added hash field

        public ScoreData(string levelId, string username, float rawTime, string formattedTime, string hash)
        {
            this.level_id = levelId;
            this.username = username;
            this.raw_time = rawTime;
            this.formatted_time = formattedTime;
            this.hash = hash;
        }
    }

    [System.Serializable]
    public class LeaderboardEntry
    {
        public string username;
        public float raw_time;
        public string formatted_time;
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