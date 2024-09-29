using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelX", menuName = "LevelData")]
public class LevelData : ScriptableObject
{
    [SerializeField] public string levelName;
    [SerializeField] public string levelId = "test";  // Unique ID for the level
    [SerializeField] public int tileSize, rows, columns;
    [SerializeField] public FlipCode2DArray goalDataArray;
    [SerializeField] public WorldData associatedWorldData;
    [SerializeField] public float optimalTimeS = 10.0f;
    [HideInInspector] public float bestTime = float.MaxValue;  // Default to max value
    [HideInInspector] public Cubemap cubemap;
    [HideInInspector] public Color cubemapColor, tileColorTop, tileColorBottom;

    // Serializable class for saving and loading data
    [System.Serializable]
    public class LevelDataSave
    {
        public string levelId;  // Unique ID of the level
        public float bestTime;  // Best time to save/load
    }

    // Save bestTime to JSON file
    public void SaveBestTimeToJson()
    {
        LevelDataSave data = new LevelDataSave();
        data.levelId = levelId;
        data.bestTime = bestTime;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(GetFilePath(), json);
        Debug.Log($"Best time saved for {levelId} at path: {GetFilePath()}");
    }

    // Load bestTime from JSON file
    public void LoadBestTimeFromJson()
    {
        string filePath = GetFilePath();
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            LevelDataSave data = JsonUtility.FromJson<LevelDataSave>(json);
            bestTime = data.bestTime;
            Debug.Log($"Best time loaded for {levelId}: {bestTime}");
        }
        else
        {
            bestTime = float.MaxValue;  // Default value if no file exists
            Debug.LogWarning($"No save file found for {levelId}, default bestTime set.");
        }
    }

    // Generate the file path for saving/loading the JSON data
    private string GetFilePath()
    {
        return Application.persistentDataPath + "/" + levelId + "_bestTime.json";
    }

    // Example method to reset the bestTime and save it
    public void ResetBestTime()
    {
        bestTime = float.MaxValue;
        SaveBestTimeToJson();
    }

    private void OnValidate()
    {
        // Ensure WorldData is set correctly
        if (associatedWorldData != null)
        {
            cubemap = associatedWorldData.worldCubemap;
            cubemapColor = associatedWorldData.worldCubemapColor;
            tileColorTop = associatedWorldData.tileColorTop;
            tileColorBottom = associatedWorldData.tileColorBottom;
        }
        else
        {
            Debug.LogWarning($"Associated WorldData is not set for LevelData '{levelName}'");
        }

        // Ensure goalDataArray is initialized correctly
        if (goalDataArray == null || goalDataArray.array == null || goalDataArray.rows != rows || goalDataArray.columns != columns)
        {
            goalDataArray = new FlipCode2DArray(rows, columns);
        }
        else if (goalDataArray.rows != rows || goalDataArray.columns != columns)
        {
            goalDataArray.rows = rows;
            goalDataArray.columns = columns;
            goalDataArray.array = new FlipCode[rows * columns];
        }
    }

    // Example method to complete a level and update best time if needed
    public void CompleteLevel(float completionTime)
    {
        if (completionTime < bestTime)
        {
            bestTime = completionTime;
            SaveBestTimeToJson();
            Debug.Log($"New best time saved: {bestTime}");
        }
    }
}