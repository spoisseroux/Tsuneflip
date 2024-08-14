using UnityEngine;

[CreateAssetMenu(fileName = "WorldData", menuName = "WorldData", order = 1)]
public class WorldData : ScriptableObject
{
    public string worldName;
    public string worldFolderName;
    public Cubemap worldCubemap;
    public Color worldCubemapColor;
    [SerializeField] public Color tileColorTop;
    [SerializeField] public Color tileColorBottom;
}
