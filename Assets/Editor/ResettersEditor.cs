using UnityEditor;
using UnityEngine;

public class ResettersEditor : EditorWindow
{
    [MenuItem("Tools/Resetters")]
    public static void ShowWindow()
    {
        // Opens the window and makes it dockable
        ResettersEditor window = GetWindow<ResettersEditor>("Resetters");
        window.minSize = new Vector2(300, 200); // Set a minimum size for better docking experience
    }

    private void OnGUI()
    {
        GUILayout.Label("Reset Options", EditorStyles.boldLabel);

        if (GUILayout.Button("Reset All PlayerPrefs"))
        {
            ResetPlayerPrefs();
        }

        if (GUILayout.Button("Reset All Best Times"))
        {
            ResetAllLevelDataBestTimes();
        }
    }

    private void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save(); // Save changes
        Debug.Log("All PlayerPrefs have been reset.");
        EditorUtility.DisplayDialog("Reset PlayerPrefs", "All PlayerPrefs have been reset.", "OK");
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
        EditorUtility.DisplayDialog("Reset Best Times", "Reset best times for LevelData objects.", "OK");
    }
}