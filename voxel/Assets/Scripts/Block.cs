using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Block : GenericBlock
{
    public Ray[] ray;


    public static void BlockDestroy(GameObject block_to_del)
    {
        // Create a tinyblock of the same type of the block to be destroyed
        TinyBlocks.Blockinit(data.block_particle,block_to_del.GetComponent<Block>().type, block_to_del.GetComponent<Transform>().position, ChunkManager.IsChunk(ChunkManager.GetChunkSpace(block_to_del.GetComponent<Transform>().position)).transform);

        Block willdie = block_to_del.GetComponent<Block>();
        Ray[] r = { willdie.ray[0], willdie.ray[1], willdie.ray[2], willdie.ray[3], willdie.ray[4], willdie.ray[5] };
        int layer = data.blocklayermask;

        // Tell every other block around to unhide
        for (int i = 0; i < 6; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(r[i], out hit, 1f, layer))
            {
                hit.transform.GetComponent<Block>().changeStateIfCave(true);
            }
        }

        //Debug.Log(block_to_del.transform.position);
        Destroy(block_to_del);
    }


    public bool changeStateIfCave(bool overr = false)
    {

        int layer = data.hardblocklayermask;
        
        bool IsHidden;
        if (GetComponentInParent<ChunkManager>().chunkState)
        {
            // Do Raycast only if required, to save time
            //Debug.Log(ray[0]);
            //IsHidden = false;
            IsHidden = Physics.Raycast(ray[0], 1f, layer) && Physics.Raycast(ray[1], 1f, layer) && Physics.Raycast(ray[2], 1f, layer) && Physics.Raycast(ray[3], 1f, layer) && Physics.Raycast(ray[4], 1f, layer) && Physics.Raycast(ray[5], 1f, layer);
            //Debug.Log(IsHidden);
            GetComponent<Renderer>().enabled = (!IsHidden || overr);
        }
        else
        {
            GetComponent<Renderer>().enabled = IsHidden = false;
        }
        

        // Kickstart the hiding protocol
        if (overr) { StartCoroutine(FallAsleep()); }

        return IsHidden;
        //Debug.Log(stat);
    }

    public blocktypes getBlockType()
    {
        return (blocktypes)int.Parse(GetComponent<Renderer>().material.name);
    }

    void Start()
    {
        // As blocks cannot change position we make it part of the data
        ray = new Ray[6] {
            new Ray(transform.position, transform.up),
            new Ray(transform.position, -transform.up),
            new Ray(transform.position, transform.forward),
            new Ray(transform.position, -transform.forward),
            new Ray(transform.position, transform.right),
            new Ray(transform.position, -transform.right)
        };

        // Commented - Do not fall asleep again
        //StartCoroutine(FallAsleep());
        //gameObject.SetActive(false);
    }

    /// <summary>
    /// Check if surrounded, and sleep
    /// </summary>
    /// <returns></returns>
    IEnumerator FallAsleep()
    {
        //yield return new WaitForSeconds(.2f);
        while (true)
        {
            yield return new WaitForSeconds(data.timeslots*5);
            if (changeStateIfCave())
            {
                break;
            }
        }
    }


}
