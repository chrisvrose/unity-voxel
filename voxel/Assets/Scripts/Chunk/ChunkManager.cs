using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    static readonly int[] GenesisIntesity = { 2, 16 };
    static readonly float[] GenesisScale = { 8f, 16f };
    public readonly short seedf = 3567;
    [Range(1, 8)]
    public int generationRadius;
    public static int chunkSize = 16;
    public Dictionary<Vector3Int, GameObject> Chunks;
    // Start is called before the first frame update


    /// <summary>
    /// Initialize chunk data location
    /// </summary>
    void Start()
    {
        Chunks = new Dictionary<Vector3Int, GameObject>(new VectorIntEquality());
        //Loaded, now start the generation
        StartCoroutine(Generation());
    }


    /// <summary>
    /// Create chunk at real space location
    /// </summary>
    /// <param name="location">Approximate Coordinate(will be wrapped to nearest chunk)</param>
    /// <returns></returns>
    public GameObject createChunk(Vector3 location)
    {
        return createChunk( getChunkCoords(location) );
        
    }
    /// <summary>
    /// Create chunk at real space location
    /// </summary>
    /// <param name="location">Exact Coordinate</param>
    /// <returns></returns>
    public GameObject createChunk(Vector3Int location){
        if (!Chunks.ContainsKey(location))
        {
            GameObject chunk = Instantiate(Data.chunkPrefab, location, Quaternion.identity);
            try{
                Chunks.Add(location,chunk);
            }
            catch(ArgumentException){
                Debug.Log("Could not add Chunk"+location.ToString());
            }
            return chunk;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Get GameObject of realspace Chunk
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    public GameObject getChunk(Vector3 location)
    {
        Vector3Int locationInteger = getChunkCoords(location);
        if (Chunks.ContainsKey(locationInteger))
        {
            return Chunks[locationInteger];
        }
        else
        {
            return null;
        }
    }


    /// <summary>
    /// Find chunk with any coordinate given in it
    /// </summary>
    /// <param name="realSpace">Real world Coordinate</param>
    /// <returns></returns>
    public static Vector3Int getChunkCoords(Vector3 realSpace)
    {
        Vector3Int chunkSpace = new Vector3Int((int)(realSpace.x / chunkSize), 0, (int)(realSpace.z / chunkSize));
        return chunkSpace * chunkSize;
    }


    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator abc()
    {
        yield return new WaitForSeconds(10);
        Debug.Log("REEEEEE");
    }



    /// <summary>
    /// Check if generation required, call required functions then
    /// </summary>
    /// <returns></returns>
    IEnumerator Generation()
    {
        Vector3 playerchunka,generateat;
        while (true)
        {
            //Get current chunk player is in
            playerchunka = ChunkManager.getChunkCoords(Data.player.transform.position);
            
            for(int x = -generationRadius; x <= generationRadius; x++)
            {
                for(int y = -generationRadius; y <= generationRadius; y++)
                {
                    generateat = playerchunka + new Vector3(x,0, y)*Chunk.ChunkSize;

                    Data.chunkManager.createChunk(generateat);
                    yield return new WaitForSeconds(.05f);
                }
            }
            
            yield return new WaitForSeconds(.5f);
            
        }
    }



}
