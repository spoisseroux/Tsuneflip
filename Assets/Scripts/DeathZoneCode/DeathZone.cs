using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class for generating a DeathZone, dynamically sized based on the size of the given level's Grid
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DeathZone : MonoBehaviour, IDealDamage
{
    [SerializeField] LevelData level; // for creating a dynamically sized death zone
    private int width, height;

    [SerializeField] private Vector3[] vertices;
    private Mesh mesh;
    private BoxCollider deathBox;
    private float yPlane;
    private float scalingConstant;

    public int hitDamage { get => hitDamage; set => hitDamage = value; }

    // do we screen for trigger here or in player?

    // all damage is done in 1 life increments!!! no value for damage

    // player.Respawn(Vector3 startTransform, Vector3 startRotation, Space.World); IF PLAYER DIES TO THIS
    // player.Respawn(Vector3 currentTransform, Vector3 currentRotation, Space.World) IF PLAYER DIES TO ENEMY

    // Generate and toggle death zone below Grid
    void Awake()
    {
        // level = LevelMenuManager.loaded; // static variable from level select
        yPlane = -5.0f;
        scalingConstant = 100.0f;
        width = level.rows;
        height = level.columns;
        GenerateZone();
        deathBox.isTrigger = true;

        this.gameObject.transform.position = new Vector3(0f, yPlane, 0f);
    }

    #region Player Interaction

    private void OnTriggerEnter(Collider other)
    {
        ITakeDamage damageableObject = other.GetComponent<ITakeDamage>();
        damageableObject?.TakeDamage(this);
    }

    #endregion

    #region Zone Generation
    private void GenerateZone()
    {
        GenerateVertices();
        GenerateTriangles();
        GenerateCollider();
        mesh.RecalculateNormals();
    }

    private void GenerateVertices()
    {
        // create and set vertices array
        vertices = new Vector3[4];
        vertices[0] = new Vector3(width * scalingConstant * -1, yPlane, height * scalingConstant * -1); // Bottom-left
        vertices[1] = new Vector3(width * scalingConstant, yPlane, height * scalingConstant * -1); // Bottom-right
        vertices[2] = new Vector3(width * scalingConstant, yPlane, height * scalingConstant); // Top-right
        vertices[3] = new Vector3(width * scalingConstant * -1, yPlane, height * scalingConstant); // Top-left

        // create uv coordinates
        Vector2[] uv = new Vector2[4];
        uv[0] = Vector2.zero;
        uv[1] = Vector2.right;
        uv[2] = Vector2.up;
        uv[3] = Vector2.one;

        // create Mesh instance
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";

        // set Mesh
        mesh.vertices = vertices;
        //mesh.uv = uv;
    }

    private void GenerateTriangles()
    {
        // clockwise == rendered face up
        int[] triangles = new int[6];
        triangles[0] = 0;
        triangles[4] = triangles[1] = 3;
        triangles[3] = triangles[2] = 1;
        triangles[5] = 2;

        // set triangles
        mesh.triangles = triangles;
    }

    private void GenerateCollider()
    {
        BoxCollider deathBox = gameObject.AddComponent<BoxCollider>();
        deathBox.size = new Vector3(width * scalingConstant * 2.0f, 1, height * scalingConstant * 2.0f);
        deathBox.isTrigger = true;
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        if (vertices == null)
            return;

        Gizmos.color = Color.magenta;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 1f);
        }
    }
    #endregion
}
