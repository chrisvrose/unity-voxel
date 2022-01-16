using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class Chunk : NetworkBehaviour {
    const short chunkSize = Generator.chunkSize;
    /// <summary>
    /// Displacement seed - Used to move the perlin noise sines out of the origin
    /// </summary>
    public static readonly short seedf = 3567;
    public static readonly Vector2 GenesisDisplacement = new Vector2(seedf % 100, seedf / 100);

    /// <summary>
    /// A chunks generation rate. This is set by the current deltaTimeFrame.
    /// Smooths out lag spikes
    /// </summary>
    uint generationRate;

    // parent references
    ChunkManager chunkManager;
    

    void Start()
    {
        if (!isServer) return;
        // Well, we have created a new chunk. Time to generate it.
        chunkManager = GameObject.FindGameObjectWithTag("WorldLight").GetComponent<ChunkManager>();

        generationRate = (uint)(1f / (15 * Time.deltaTime));
        if (!isServer) { 
            return; 
        }
        StartCoroutine(Generate());
        // Be off by default
        // setState(false);
    }

    /// <summary>
    /// Generate all the blocks inside the chunk. Called after creation of chunk.
    /// At the end, to update Chunk Mesh
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    [Server]
    public IEnumerator Generate(){
        short numberOfInstances = 0;
        chunkManager.generatingChunksCount++;
        // how many chunks are generating right now? split by N to prevent this garbage
        uint approximateLimit = generationRate / chunkManager.generatingChunksCount;
        // Debug.Log(approximateLimit);
        if (approximateLimit<1) approximateLimit = 1;
        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                
                short height = Generator.CalculateHeight(GenesisDisplacement.x + transform.position.x + x, GenesisDisplacement.y + transform.position.z + z);
                
                chunkManager.BlockInit(GenericBlock.PlaceBlockType.BLOCK,blocktypes.Grass, new Vector3(transform.position.x + x, height--, transform.position.z + z));      //Build Grass and remove 1 from height
                
                for (int y = 0; y <= height; y++)
                {
                    chunkManager.BlockInit(GenericBlock.PlaceBlockType.BLOCK, blocktypes.Dirt, new Vector3(transform.position.x + x, y, transform.position.z + z));
                    // Chuck in a block
                    // Increment numberOfInstances
                }
                numberOfInstances++;

                // If the number of instances per frame was met
                if (numberOfInstances == approximateLimit)
                {
                    // Reset numberOfInstances
                    numberOfInstances = 0;
                    // Wait for next frame
                    yield return new WaitForEndOfFrame();
                }
            }
        }
        chunkManager.generatingChunksCount--;
        
    }
    

}
