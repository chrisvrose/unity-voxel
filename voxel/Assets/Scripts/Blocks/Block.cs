using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Block : GenericBlock
{
    /// <summary>
    /// Setup transform rays
    /// </summary>
    public new void Start(){
        base.Start();
        
    }

    /// <summary>
    /// Check if block is surrounded by solid. True if covered
    /// </summary>
    /// <returns></returns>
    public bool isCovered()
    {
        Ray[] rays = new Ray[]{
            new Ray(transform.position, transform.up),
            new Ray(transform.position, -transform.up),
            new Ray(transform.position, transform.forward),
            new Ray(transform.position, -transform.forward),
            new Ray(transform.position, transform.right),
            new Ray(transform.position, -transform.right)
        };
        //bool endstate = true;
        //Debug.Log(Rays.Length);
        bool isHidden = Physics.Raycast(rays[0], 1f, Data.hardBlockLayerMask) && 
                        Physics.Raycast(rays[1], 1f, Data.hardBlockLayerMask) && 
                        Physics.Raycast(rays[2], 1f, Data.hardBlockLayerMask) && 
                        Physics.Raycast(rays[3], 1f, Data.hardBlockLayerMask) && 
                        Physics.Raycast(rays[4], 1f, Data.hardBlockLayerMask) && 
                        Physics.Raycast(rays[5], 1f, Data.hardBlockLayerMask);

        return isHidden;//return (Physics.Raycast(Rays[0], 1f, data.blocklayermask));        
    }

    public void BlockDestroy()
    {
        // Create a tinyblock of the same type of the block to be destroyed'
        Transform RequiredChunk = Data.chunkManager.getChunk(transform.position)?.transform;//Chunk.IsChunk(Data.chunkManager.GetChunkSpace(transform.position)).transform;

        GenericBlock.Blockinit(Data.blockParticlePrefab, BaseItem.Type, transform.position, RequiredChunk);
        // Tell parent chunk to update mesh

        // Destroy filter to disable rendering
        gameObject.GetComponent<MeshFilter>().mesh.Clear();

        // Update mesh
        transform.GetComponentInParent<Chunk>().StartCoroutine("DelayedUpdateMesh");
        Destroy(this.gameObject);
        //Sorcery   
    }
}
