using UnityEditor;
using UnityEngine;

public class LevelDataResetterEditor : EditorWindow
{
    [MenuItem("Tools/Reset All LevelData Best Times")]
    public static void ShowWindow()
    {
        GetWindow<LevelDataResetterEditor>("Reset LevelData Best Times");
    }

    private void OnGUI()
    {
        GUILayout.Label("Reset Best Times for All LevelData", EditorStyles.boldLabel);

        if (GUILayout.Button("Reset All Best Times"))
        {
            ResetAllLevelDataBestTimes();
        }
    }

    private void ResetAllLevelDataBestTimes()
    {
        // Find all LevelData assets in the project
        string[] guids = AssetDatabase.FindAssets("t:LevelData");
        int count = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            LevelData levelData = AssetDatabase.LoadAssetAtPath<LevelData>(path);

            if (levelData != null)
            {
                levelData.ResetBestTime();
                EditorUtility.SetDirty(levelData); // Mark the object as dirty to save the changes
                count++;
            }
        }

        AssetDatabase.SaveAssets(); // Save all the changes
        Debug.Log($"Reset best time for {count} LevelData objects.");
    }
}