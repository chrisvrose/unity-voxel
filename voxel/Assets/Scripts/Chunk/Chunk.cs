using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Chunk : MonoBehaviour {
    static readonly int[] GenesisIntesity = { 2,16 };
    static readonly float[] GenesisScale = { 8f, 16f };
    public static short ChunkSize = 16;
    public static readonly short seedf = 3567;
    public static readonly Vector2 GenesisDisplacement = new Vector2(seedf % 100, seedf / 100);

    public static readonly int generateRadius = 2;

    public bool chunkState = true;

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

        for (int x = 0; x < Chunk.ChunkSize; x++)
        {
            for (int z = 0; z < Chunk.ChunkSize; z++)
            {
                
                short height = Chunk.CalculateHeight(GenesisDisplacement.x + parent.position.x + x, GenesisDisplacement.y + parent.position.z + z);
                
                GenericBlock.Blockinit(Data.block,blocktypes.Grass, new Vector3(parent.position.x + x, height--, parent.position.z + z),parent,false);      //Build Grass and remove 1 from height
                
                for (int y = 0; y <= height; y++)
                {
                    // Chuck in a block
                    GenericBlock.Blockinit(Data.block,blocktypes.Dirt, new Vector3(parent.position.x + x, y, parent.position.z + z),parent,false);
                    // Increment numberOfInstances
                }
                numberOfInstances++;

                // If the number of instances per frame was met
                if (numberOfInstances == Data.gendegen_rate)
                {
                    // Reset numberOfInstances
                    numberOfInstances = 0;
                    // Wait for next frame
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        // Generation complete, commence a mesh update
        parent.SendMessage("UpdateMesh");
    }

    public void UpdateMesh()
    {
        List<Mesh> meshes = new List<Mesh>(6);

        // Unholy
        List<List<CombineInstance>> combineInstances = new List<List<CombineInstance>>(6);
        List<CombineInstance> finalCombine = new List<CombineInstance>();
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

        //Init lists - Per material
        for(int i = 0; i < 6; i++)
        {
            combineInstances.Add(new List<CombineInstance>());
        }


        //Bunchup all mesh filters into respective combineInstances
        foreach(MeshFilter i in meshFilters)
        {
            // Skip non-blocks and if no meshfilter
            if(i.gameObject.GetComponent<Block>() == null || i.gameObject.GetComponent<MeshFilter>()==null)
            {
                continue;
            } else if (i.gameObject.GetComponent<Block>().GetCave())
            {
                continue;
            }
            int index = (int)(i.gameObject.GetComponent<Block>().GetBlockType());
            combineInstances[index].Add(new CombineInstance { mesh = i.sharedMesh, transform = i.transform.localToWorldMatrix * Matrix4x4.Rotate(transform.rotation).inverse * Matrix4x4.Translate(transform.position).inverse });
            //lastElement.mesh = i.sharedMesh;
            // Transform to World Coordinates, but relative to the Chunk object by undoing self transformation and rotation
            //lastElement.transform = i.transform.localToWorldMatrix * Matrix4x4.Rotate(transform.rotation).inverse * Matrix4x4.Translate(transform.position).inverse;
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
        Debug.Log("Updated");
    }

    IEnumerator DelayedUpdateMesh()
    {
        yield return new WaitForEndOfFrame();
        UpdateMesh();
    }

}
