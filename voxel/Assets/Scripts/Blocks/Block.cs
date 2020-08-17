using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Block : GenericBlock
{
    protected MeshFilter myMeshFilter;
    /// <summary>
    /// Check if block is surrounded by solid. True if covered
    /// </summary>
    /// <returns></returns>
    public bool GetCave()
    {
        //bool endstate = true;
        //Debug.Log(Rays.Length);
        Ray[] ray = new Ray[6] {
            new Ray(transform.position, transform.up),
            new Ray(transform.position, -transform.up),
            new Ray(transform.position, transform.forward),
            new Ray(transform.position, -transform.forward),
            new Ray(transform.position, transform.right),
            new Ray(transform.position, -transform.right)
        };
        bool isHidden = Physics.Raycast(ray[0], 1f, Data.hardblocklayermask) && Physics.Raycast(ray[1], 1f, Data.hardblocklayermask) && Physics.Raycast(ray[2], 1f, Data.hardblocklayermask) && Physics.Raycast(ray[3], 1f, Data.hardblocklayermask) && Physics.Raycast(ray[4], 1f, Data.hardblocklayermask) && Physics.Raycast(ray[5], 1f, Data.hardblocklayermask);

        //print(this.Rays[0].direction);
        return isHidden;//return (Physics.Raycast(Rays[0], 1f, data.blocklayermask));
        //return true;
    }

    public void BlockDestroy()
    {
        // Create a tinyblock of the same type of the block to be destroyed'
        // TODO
        Transform RequiredChunk = Data.chunkManager.getChunk(transform.position)?.transform;//Chunk.IsChunk(Data.chunkManager.GetChunkSpace(transform.position)).transform;
        GenericBlock.Blockinit(Data.block_particle,BaseItem.Type, transform.position, RequiredChunk);

        // Tell parent chunk to update mesh

        // Destroy filter to disable rendering
        gameObject.GetComponent<MeshFilter>().mesh.Clear();
        // Update mesh
        transform.GetComponentInParent<Chunk>().StartCoroutine("DelayedUpdateMesh");
        Destroy(this.gameObject);
        //Sorcery
        
    }
}
