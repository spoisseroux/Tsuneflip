using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class DebugLevelManager : MonoBehaviour
{
    // Level objects
    [SerializeField] GridManager grid;
    [SerializeField] DeathZone deathZone;

    // for respawn debug
    [SerializeField] Vector3 respawnPosition;
    [SerializeField] Vector3 respawnRotation;

    // NavMesh
    public NavMeshSurface surface;

    private void Awake()
    {
        // read this from leveldata later i guess
        respawnPosition = new Vector3(0.5f, 1.5f, 0.5f);
        respawnRotation = new Vector3(0.5f, 0f, 0.5f);

        // find Grid, get components
        grid = GameObject.Find("Grid").GetComponent<GridManager>();
        grid.InitializeGrid();

        // generate navmesh
        surface.BuildNavMesh();
    }
}
