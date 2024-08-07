using System;
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

// could we use Unity TileMap here?
//[RequireComponent(typeof(MeshRenderer))]
public class Tile : MonoBehaviour
{
    // positioning and location variables, should logic be split between physical GameObject info and backend Tile data?

    // Tile data
    [SerializeField]
    private TileData tile;

    // [SerializeField] BoxCollider tileBox; may not even need this if coordinate resolution system in GridManager is good enough

    // rendering code
    [SerializeField] MeshRenderer render;
    [SerializeField] List<Material> materials; // size of 3, 0 -> flipped texture 1 -> unflipped texture 2 -> empty texture
    // do we need another material for the transition shaders?

    // flipping event handler
    private delegate void OnFlip();
    private event OnFlip flip;

    public void Awake()
    {
        tile = new TileData();
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
    }

    public void FlipTriggered()
    {
        // for safety's sake
        if (tile.flipState == FlipCode.Empty)
            return;

        // do self-contained logic like flipping enum and changing the shader/material
        FlipEnum();
        //render.material = materials[(int)tile.flipState]; // hmmm

        // notify observers of flip event (sound system, etc)
        flip?.Invoke();
    }

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
        return;
    }
}
