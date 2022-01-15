using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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

    [Range(1, 8)]
    public int generationRadius;
    [Range(1, 6)]
    public int renderRadius;
    public static int chunkSize = 16;
    
    public Dictionary<Vector3Int, GameObject> Chunks;
    // Start is called before the first frame update


    //public Dictionary<Vector3Int, GameObject> ActiveChunks;
    public uint generatingChunksCount = 0;

    
    
    override public void OnStartClient()
    {
        //super.OnStartClient();
        #region register prefab spawning for clients
        ClientScene.RegisterPrefab(chunkPrefab);
        ClientScene.RegisterPrefab(blockPrefab);
        ClientScene.RegisterPrefab(blockParticlePrefab);
        #endregion
    }
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
        return createChunk( getChunkCoords(location) );
        
    }
    /// <summary>
    /// Create chunk at real space location
    /// </summary>
    /// <param name="location">Exact Coordinate</param>
    /// <returns></returns>
    [Server]
    public GameObject createChunk(Vector3Int location){
        if (!Chunks.ContainsKey(location))
        {
            GameObject chunk = Instantiate(chunkPrefab, location, Quaternion.identity);
            NetworkServer.Spawn(chunk);
            try{
                Chunks.Add(location,chunk);
            }
            catch(ArgumentException){
                Debug.LogError("Could not add Chunk"+location.ToString());
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
    [Server]
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
        Vector3 playerchunka,generateat;
        for(;;)
        {
            //Get current chunk player is in
            playerchunka = ChunkManager.getChunkCoords( Vector3.zero);
            
            for(int x = -generationRadius; x <= generationRadius; x++)
            {
                for(int y = -generationRadius; y <= generationRadius; y++)
                {
                    generateat = playerchunka + new Vector3(x,0, y)*Chunk.ChunkSize;

                    createChunk(generateat);
                    yield return new WaitForEndOfFrame();
                }
            }
        }
    }

    //[Client,Obsolete]
    //IEnumerator MaintainActive(){
    //    for(;;){
    //        var playerLocation = Data.player.transform.position;
    //        Vector3Int playerChunkLocation = getChunkCoords(playerLocation);
    //        Dictionary<Vector3Int,GameObject> newActiveChunks = new Dictionary<Vector3Int, GameObject>();
    //        Vector3Int renderAt;
    //        for(int x = -generationRadius; x <= generationRadius; x++)
    //        {
    //            for(int y = -generationRadius; y <= generationRadius; y++)
    //            {
    //                renderAt = playerChunkLocation + new Vector3Int(x,0, y)*Chunk.ChunkSize;
    //                if(Chunks.ContainsKey(renderAt)){
    //                    newActiveChunks.Add(renderAt,Chunks[renderAt]);
    //                }

                    
    //            }

    //        }
    //        //Done for this frame
    //        yield return new WaitForEndOfFrame();
    //        //Now find all the old ones
    //        foreach(var pair in ActiveChunks){
    //            if(newActiveChunks.ContainsKey(pair.Key)){
    //                continue;
    //            }
    //            pair.Value.GetComponent<Chunk>().setState(false);
    //            // pair.Value.SendMessage("setState",value:false);
    //        }

    //        ActiveChunks = newActiveChunks;
    //        foreach(var pair in ActiveChunks){
    //            pair.Value.GetComponent<Chunk>().setState(true);
    //            // pair.Value.SendMessage("setState",value:true);
    //        }


    //        yield return new WaitForSeconds(.5f);
    //    }   
    //}

    /// <summary>
    /// Command for Clients to create a block
    /// </summary>
    /// <param name="placeBlockType"></param>
    /// <param name="block"></param>
    /// <param name="pos"></param>
    /// <param name="parent"></param>
    /// <param name="UpdateMesh"></param>
    [Command]
    public void CmdBlockInit(GenericBlock.PlaceBlockType placeBlockType, blocktypes block, Vector3 pos, bool UpdateMesh = true)
    {
        BlockInit(placeBlockType, block, pos, UpdateMesh);
    }

    [Server]
    public void BlockInit(GenericBlock.PlaceBlockType placeBlockType, blocktypes block, Vector3 pos, bool UpdateMesh = true)
    {
        GameObject prefab;
        if (placeBlockType == GenericBlock.PlaceBlockType.BLOCK) prefab = blockPrefab;
        else if (placeBlockType == GenericBlock.PlaceBlockType.TINYBLOCK) prefab = blockParticlePrefab;
        else prefab = blockPrefab;

        var parent = getChunk(pos)?.transform;
        GameObject returnable =  GenericBlock.Blockinit(prefab, block, pos, parent,availableMaterials, UpdateMesh);
        NetworkServer.Spawn(returnable);
        //NetworkServer.Spaw
        //return returnable;
    }
}
