using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Block : GenericBlock
{
    protected Ray[] Rays;

    public void Start()
    {
        Rays = new Ray[6] {
            new Ray(transform.position, transform.up),
            new Ray(transform.position, -transform.up),
            new Ray(transform.position, transform.forward),
            new Ray(transform.position, -transform.forward),
            new Ray(transform.position, transform.right),
            new Ray(transform.position, -transform.right)
        };
    }

    /// <summary>
    /// Check if block is surrounded by solid
    /// </summary>
    /// <returns></returns>
    public bool GetCave()
    {
        bool endstate = true;
        Debug.Log(Rays.Length);
        foreach(Ray r in Rays)
        {
            RaycastHit hit;
            if(Physics.Raycast(r,out hit, 1f, data.blocklayermask))
            {
                endstate &= false;
            }
        }
        return endstate;
    }

    public void BlockDestroy()
    {
        // Create a tinyblock of the same type of the block to be destroyed
        GenericBlock.Blockinit(data.block_particle,BaseItem.Type, transform.position, ChunkManager.IsChunk(ChunkManager.GetChunkSpace(transform.position)).transform);

        // Tell parent chunk to update mesh

        // Destroy filter to disable rendering
        Destroy(gameObject.GetComponent<MeshFilter>());
        // Update mesh
        //gameObject.GetComponentInParent<ChunkManager>().UpdateMesh();

        Destroy(this.gameObject);
    }
}
