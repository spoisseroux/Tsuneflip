using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FlipCode
{
    Unflipped,
    Flipped,
    Empty
}

[System.Serializable]
public class TileData
{
    [SerializeField]
    public FlipCode flipState;

    public Dictionary<FlipCode, int> enumMap = new Dictionary<FlipCode, int>{
        {FlipCode.Unflipped, 0},
        {FlipCode.Flipped, 1},
        {FlipCode.Empty, 2}
    };
}

public class Tile : MonoBehaviour
{
    private RotateTile rotateTileScript;
    [SerializeField]
    private TileData tile;
    //[SerializeField] MeshRenderer render;
    //[SerializeField] List<Material> materials; // size of 3, 0 -> flipped texture 1 -> unflipped texture 2 -> empty texture

    private delegate void OnFlip();
    private event OnFlip flip;

    public void Awake()
    {
        tile = new TileData();
        rotateTileScript = GetComponent<RotateTile>();
        if (rotateTileScript == null)
        {
            Debug.LogError("RotateTile script not found!");
        }
    }


    public void Update()
    {
        //TODO: Remove debug testing
        if (Input.GetKeyDown(KeyCode.R))
        {
            FlipTile();   
        }
    }

    // CALL THIS WHEN U WANNA FLIP TILE
    public void FlipTile(){
        StartCoroutine(FlipTriggered());
    }

    public void OnTriggerEnter(Collider other)
    {
        // if player, play tile enter sound
    }

    public void InitializeTile(FlipCode flip)
    {
        // set flip state
        if (flip == FlipCode.Empty)
        {
            tile.flipState = FlipCode.Empty;
        }
        else
        {
            tile.flipState = FlipCode.Unflipped;
        }
        Debug.Log("Initialized Tile Flip State: " + tile.flipState);
    }


    private IEnumerator FlipTriggered()
    {
        // for safety's sake
        if (tile.flipState == FlipCode.Empty)
        {
            yield break;
        }

        // rotate tile if not already rotating
        if (!rotateTileScript.IsRotating)
        {
            Debug.Log("Starting Rotation...");
            yield return StartCoroutine(rotateTileScript.Rotate());

            // once rotation is complete, execute FlipEnum and invoke flip event
            FlipEnum();
            Debug.Log("Flip State after rotation: " + tile.flipState);
            //render.material = materials[(int)tile.flipState];
            flip?.Invoke();
        }
        else
        {
            Debug.Log("Tile is already rotating.");
        }
    }

    //TODO: Not entering enum???
    private void FlipEnum()
    {
        switch (tile.flipState)
        {
            case FlipCode.Empty:
                break;

            case FlipCode.Flipped:
                tile.flipState = FlipCode.Unflipped;
                break;

            case FlipCode.Unflipped:
                tile.flipState = FlipCode.Flipped;
                break;
        }
        Debug.Log("New Flip State: " + tile.flipState);
    }
}