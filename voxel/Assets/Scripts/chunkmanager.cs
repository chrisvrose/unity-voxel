using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChunkManager : MonoBehaviour {
    static readonly int[] GenesisIntesity = { 2,16 };
    static readonly float[] GenesisScale = { 8f, 16f };
    public static short ChunkSize = 16;
    public static readonly short seedf = 3567;
    public static readonly Vector2 GenesisDisplacement = new Vector2(seedf % 100, seedf / 100);

    public static readonly int generateRadius = 2;

    public bool chunkState = true;//false;

    private static List<GameObject> Chunks = new List<GameObject>();
    private static List<Vector3> ChunksLocation = new List<Vector3>();

    
    /// <summary>
    /// Convert real space into chunk space
    /// </summary>
    /// <param name="realSpace"></param>
    /// <returns>Vector of the Chunk space coordinates of the chunk(if/not exists)</returns>
    public static Vector3 GetChunkSpace(Vector3 realSpace)
    {
        return new Vector3((int)(realSpace.x / ChunkSize), 0, (int)(realSpace.z / ChunkSize));
    }


    /// <summary>
    /// Convert real space into chunk space, and report it in realspace
    /// </summary>
    /// <param name="realSpace"></param>
    /// <returns></returns>
    public static Vector3 GetChunkRealSpace(Vector3 realSpace)
    {
        return GetChunkSpace(realSpace)*ChunkSize;
    }

    /// <summary>
    /// Create chunk at real space location
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    public static GameObject CreateChunk(Vector3 location)
    {
        if (!ChunksLocation.Contains(location))
        {
            GameObject chunk =  Instantiate(data.chunkPrefab, location,Quaternion.identity);
            Chunks.Add(chunk);
            ChunksLocation.Add(location);
            
            return chunk;
        } else {
            return null;
        }
        
        
    }

    /// <summary>
    /// Get GameObject of realspace Chunk
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    public static GameObject IsChunk(Vector3 location)
    {
        if (ChunkManager.ChunksLocation.Contains(location))
        {
            return ChunkManager.Chunks.Find(o => location == o.GetComponent<Transform>().position);
        }
        else
        {
            return null;
        }
    }

    public static short CalculateHeight(float x, float y)
    {
        short height = 0;
        for (int i = 0; i < GenesisScale.Length; i++)
        {
            height += (short)(Mathf.PerlinNoise(x / GenesisScale[i], y / GenesisScale[i]) * GenesisIntesity[i]);
        }
        return height;
    }

    public bool changeState(bool state)
    {
        foreach (Transform c in GetComponentsInChildren<Transform>())
            if (!c.name.Contains("Chunk"))
            {
                //c.enabled = state;
                if (!state) c.GetComponent<Renderer>().enabled = false;
                //else c.GetComponent<Block>().ChangeStateIfCave();
                c.GetComponent<Collider>().enabled = state;
                chunkState = state;

            }
        return true;
    }

    void Start()
    {
        // Well, we have created a new chunk. Time to generate it.
        StartCoroutine(Generate(transform));
    }

    /// <summary>
    /// Generate all the blocks inside the chunk. Called after creation of chunk.
    /// At the end, to update Chunk Mesh
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    IEnumerator Generate(Transform parent){
        short numberOfInstances = 0;

        for (int x = 0; x < ChunkManager.ChunkSize; x++)
        {
            for (int z = 0; z < ChunkManager.ChunkSize; z++)
            {
                
                short height = ChunkManager.CalculateHeight(GenesisDisplacement.x + parent.position.x + x, GenesisDisplacement.y + parent.position.z + z);
                
                GenericBlock.Blockinit(data.block,blocktypes.Grass, new Vector3(parent.position.x + x, height--, parent.position.z + z),parent);      //Build Grass and remove 1 from height
                
                for (int y = 0; y <= height; y++)
                {
                    // Chuck in a block
                    GenericBlock.Blockinit(data.block,blocktypes.Dirt, new Vector3(parent.position.x + x, y, parent.position.z + z),parent);
                    // Increment numberOfInstances
                }
                numberOfInstances++;

                // If the number of instances per frame was met
                if (numberOfInstances == data.gendegen_rate)
                {
                    // Reset numberOfInstances
                    numberOfInstances = 0;
                    // Wait for next frame
                    yield return new WaitForEndOfFrame();
                }
            }
        }
        
    }


}
