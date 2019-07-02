using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Block : GenericBlock
{
    //public Ray[] ray;

    public void BlockDestroy()
    {
        // Create a tinyblock of the same type of the block to be destroyed
        GenericBlock.Blockinit(data.block_particle,BaseItem.Type, transform.position, ChunkManager.IsChunk(ChunkManager.GetChunkSpace(transform.position)).transform);

        // Tell parent chunk to update mesh

        // Destroy filter to disable rendering
        Destroy(gameObject.GetComponent<MeshFilter>());
        // Update mesh
        gameObject.GetComponentInParent<ChunkManager>().UpdateMesh();

        Destroy(this.gameObject);
    }


    void Start()
    {
        // shruggie
    }
}
