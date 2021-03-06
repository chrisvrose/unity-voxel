﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Chunk : MonoBehaviour {
    static readonly int[] GenesisIntesity = { 2,16 };
    static readonly float[] GenesisScale = { 8f, 16f };
    public static short ChunkSize = 16;
    public static readonly short seedf = 3567;
    public static readonly Vector2 GenesisDisplacement = new Vector2(seedf % 100, seedf / 100);

    public static readonly int generateRadius = 2;


    // TOD Replace
    public static short CalculateHeight(float x, float y)
    {
        short height = 0;
        for (int i = 0; i < GenesisScale.Length; i++)
        {
            height += (short)(Mathf.PerlinNoise(x / GenesisScale[i], y / GenesisScale[i]) * GenesisIntesity[i]);
        }
        return height;
    }

    public bool setState(bool state){
        gameObject.SetActive(state);
        return state;
    }

    void Start()
    {
        // Well, we have created a new chunk. Time to generate it.
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
    public IEnumerator Generate(){
        short numberOfInstances = 0;
        Data.chunkManager.generatingChunksCount++;
        uint approximateLimit = Data.gendegen_rate / Data.chunkManager.generatingChunksCount;
        // Debug.Log(approximateLimit);
        if (approximateLimit<1) approximateLimit = 1;
        for (int x = 0; x < Chunk.ChunkSize; x++)
        {
            for (int z = 0; z < Chunk.ChunkSize; z++)
            {
                
                short height = Chunk.CalculateHeight(GenesisDisplacement.x + transform.position.x + x, GenesisDisplacement.y + transform.position.z + z);
                
                GenericBlock.Blockinit(Data.block,blocktypes.Grass, new Vector3(transform.position.x + x, height--, transform.position.z + z),transform,false);      //Build Grass and remove 1 from height
                
                for (int y = 0; y <= height; y++)
                {
                    // Chuck in a block
                    GenericBlock.Blockinit(Data.block,blocktypes.Dirt, new Vector3(transform.position.x + x, y, transform.position.z + z),transform,false);
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
        // Generation complete, commence a mesh update
        Data.chunkManager.generatingChunksCount--;
        transform.SendMessage("UpdateMesh");
        setState(false);
    }

    public void UpdateMesh()
    {
        List<Mesh> meshes = new List<Mesh>(6);

        // Unholy
        List<List<CombineInstance>> combineInstances = new List<List<CombineInstance>>(6);
        List<CombineInstance> finalCombine = new List<CombineInstance>();
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>(includeInactive:false);

        //Init lists - Per material
        for(int i = 0; i < 6; i++)
        {
            combineInstances.Add(new List<CombineInstance>());
        }


        //Bunchup all mesh filters into respective combineInstances
        foreach(MeshFilter i in meshFilters)
        {
            var blockComponent = i.gameObject.GetComponent<Block>();
            // Skip non-blocks and if no meshfilter
            if(blockComponent == null || i.gameObject.GetComponent<MeshFilter>()==null)
            {
                continue;
            } else if (blockComponent.isCovered())
            {
                continue;
            }
            int index = (int)(blockComponent.GetBlockType());
            combineInstances[index].Add(new CombineInstance {
                    mesh = i.sharedMesh, 
                    transform = i.transform.localToWorldMatrix * 
                        Matrix4x4.Rotate(transform.rotation).inverse * 
                        Matrix4x4.Translate(transform.position).inverse 
                });
        }

        List<Material> materials = new List<Material>();
        for(int i = 0; i < 6; i++)
        {
            // Preparing materials for the mesh renderer
            if (combineInstances[i].Count > 0)
            {
                materials.Add(Data.materials[i]);
            }
            meshes.Add(new Mesh());
            meshes[meshes.Count - 1].CombineMeshes(combineInstances[meshes.Count - 1].ToArray());
            finalCombine.Add(new CombineInstance { mesh = meshes[meshes.Count - 1] });
        }

        
        Mesh finalMesh = new Mesh();
        finalMesh.CombineMeshes(finalCombine.ToArray(), false, false);
        transform.GetComponent<MeshFilter>().sharedMesh = finalMesh;
        transform.GetComponent<MeshRenderer>().materials = materials.ToArray();
        // Debug.Log("Updated");
    }

    public IEnumerator DelayedUpdateMesh()
    {
        yield return new WaitForEndOfFrame();
        UpdateMesh();
    }

}
