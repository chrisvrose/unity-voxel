using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ChunkManager : NetworkBehaviour
{
    static readonly int[] GenesisIntesity = { 2, 16 };
    static readonly float[] GenesisScale = { 8f, 16f };
    public readonly short seedf = 3567;

    [SerializeField]
    public Material[] availableMaterials;

    public GameObject chunkPrefab;
    public GameObject blockPrefab;
    public GameObject blockParticlePrefab;

    [Range(0, 8)]
    public int generationRadius;
    [Range(0, 6)]
    public int renderRadius;
    public static int chunkSize = 16;
    
    public Dictionary<Vector3Int, GameObject> Chunks;
    // Start is called before the first frame update


    //public Dictionary<Vector3Int, GameObject> ActiveChunks;
    public uint generatingChunksCount = 0;

    /// <summary>
    /// Initialize chunk data location
    /// </summary>
    void Start()
    {
        // do all this stuff for server only
        if (!isServer) { return; }
        renderRadius = renderRadius < generationRadius ? renderRadius : generationRadius;
        Chunks = new Dictionary<Vector3Int, GameObject>(new Vector3IntEquality());
        //ActiveChunks = new Dictionary<Vector3Int, GameObject>(new Vector3IntEquality());
        //Loaded, now start the generation

        StartCoroutine(worldGeneration());
        //StartCoroutine(MaintainActive());
    }

    /// <summary>
    /// Create chunk at real space location
    /// </summary>
    /// <param name="location">Approximate Coordinate(will be wrapped to nearest chunk)</param>
    /// <returns></returns>
    [Server]
    public GameObject createChunk(Vector3 location)
    {
        return CreateChunkIfNotExists( getChunkCoords(location) );
        
    }
    /// <summary>
    /// Create chunk at real space location
    /// </summary>
    /// <param name="location">Exact Coordinate</param>
    /// <returns></returns>
    [Server]
    public GameObject CreateChunkIfNotExists(Vector3Int location){
        if (!Chunks.ContainsKey(location))
        {
            GameObject chunk = Instantiate(chunkPrefab, location, Quaternion.identity);
            NetworkServer.Spawn(chunk);
            try{
                Chunks.Add(location,chunk);
            }
            catch(ArgumentException){
                Debug.LogError("Could not add Chunk:"+location.ToString());
            }
            return chunk;
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
        Vector3Int chunkSpace = new Vector3Int(Mathf.FloorToInt(realSpace.x / chunkSize), 0, Mathf.FloorToInt(realSpace.z / chunkSize));
        return chunkSpace * chunkSize;
    }


    /// <summary>
    /// Check if generation required, call required functions then
    /// </summary>
    /// <returns></returns>
    [Server]
    IEnumerator worldGeneration()
    {
        // a container for the chunks that will be required to be created
        HashSet<Vector3Int> requiredChunksToBeLoaded;
        for(;;)
        {
            /*
             * Two step process - 
             * 1. Using playerList create a set of required creations. Hashset should dedup them
             * 2. Schedule these for Creation
             * 3. Repeat everything as you need
             */
            // step 1
            requiredChunksToBeLoaded = new HashSet<Vector3Int>();
            foreach(var player in Player.playersList)
            {
                //get players position
                var playerChunk = ChunkManager.getChunkCoords( player.transform.position);
                for (int x = -generationRadius; x <= generationRadius; x++)
                {
                    for (int y = -generationRadius; y <= generationRadius; y++)
                    {
                        requiredChunksToBeLoaded.Add(playerChunk + new Vector3Int(x, 0, y) * Generator.chunkSize);
                    }
                }
             }

            //step 2
            foreach(var chunkLocations in requiredChunksToBeLoaded)
            {
                createChunk(chunkLocations);
                // Delayed each instance to save some processor
                yield return new WaitForEndOfFrame();
            }
            //step 3
            yield return new WaitForEndOfFrame();

            
        }
    }


    /// <summary>
    /// Command for Clients to create a block
    /// </summary>
    /// <param name="placeBlockType"></param>
    /// <param name="block"></param>
    /// <param name="pos"></param>
    /// <param name="parent"></param>
    /// <param name="UpdateMesh"></param>
    [Command]
    public void CmdBlockInit(GenericBlock.PlaceBlockType placeBlockType, blocktypes block, Vector3 pos)
    {
        BlockInit(placeBlockType, block, pos);
    }

    [Server]
    public void BlockInit(GenericBlock.PlaceBlockType placeBlockType, blocktypes block, Vector3 pos)
    {
        GameObject prefab;
        if (placeBlockType == GenericBlock.PlaceBlockType.BLOCK) prefab = blockPrefab;
        else if (placeBlockType == GenericBlock.PlaceBlockType.TINYBLOCK) prefab = blockParticlePrefab;
        else prefab = blockPrefab;

        GenericBlock.Blockinit(prefab, block, pos);
       
    }
}
