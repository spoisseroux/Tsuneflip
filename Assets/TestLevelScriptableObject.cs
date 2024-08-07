using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level")]
public class TestLevelScriptableObject : ScriptableObject
{
    public string levelName;
    public string description;
}