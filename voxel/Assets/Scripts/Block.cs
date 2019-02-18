using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Block : MonoBehaviour
{
    public blocktypes type;
    public Ray[] ray;

    public static GameObject Blockinit(blocktypes block, Vector3 pos, Transform parent)
    {
        GameObject sblock = Instantiate(data.block, pos, Quaternion.identity, parent);
        sblock.GetComponent<Block>().setBlockType(block);
        return sblock;
    }


    public static void BlockDestroy(GameObject block_to_del)
    {

        // Create a tinyblock of the same type of the block to be destroyed
        tiny_blocks.Blockinit(block_to_del.GetComponent<Block>().type, block_to_del.GetComponent<Transform>().position);

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
        Destroy(block_to_del);
    }

    /// <summary>
    /// Upon generation, call function to set the block type.
    /// </summary>
    /// <param name="block"></param>
    private void setBlockType(blocktypes block)
    {
        type = block;
        Material mat = Resources.Load("Materials/" + (int)block) as Material;
        GetComponent<Renderer>().material = mat;
        if (mat.HasProperty("_Metallic"))
        {
            //Debug.Log(mat.GetFloat("_Metallic"));
            GetComponent<ReflectionProbe>().enabled = true;
        }
        if (mat.GetColor("_EmissionColor") != (new Color(0, 0, 0)))
        {
            GetComponent<Light>().color = mat.GetColor("_EmissionColor");
            GetComponent<Light>().enabled = true;
        }
        // If transparent move to another layer
        if (mat.color.a != 1f) gameObject.layer = 10;
        //print(block);
        //renderer.

    }


    public bool changeStateIfCave(bool overr = false)
    {

        int layer = data.hardblocklayermask;
        
        bool covered;
        if (GetComponentInParent<chunkManager>().chunkState)
        {
            // Do Raycast only if required, to save time
            covered = Physics.Raycast(ray[0], 1f, layer) && Physics.Raycast(ray[1], 1f, layer) && Physics.Raycast(ray[2], 1f, layer) && Physics.Raycast(ray[3], 1f, layer) && Physics.Raycast(ray[4], 1f, layer) && Physics.Raycast(ray[5], 1f, layer);
            GetComponent<Renderer>().enabled = (!covered || overr);
        }
        else
        {
            GetComponent<Renderer>().enabled = covered = false;
        }
        

        // Kickstart the hiding protocol
        if (overr) { StartCoroutine(FallAsleep()); }

        return covered;
        //Debug.Log(stat);
    }

    public blocktypes getBlockType()
    {
        return (blocktypes)int.Parse(GetComponent<Renderer>().material.name);
    }

    void Start()
    {
        // As block cannot change position we make it part of the data
        ray = new Ray[6] {
            new Ray(transform.position, transform.up),
            new Ray(transform.position, -transform.up),
            new Ray(transform.position, transform.forward),
            new Ray(transform.position, -transform.forward),
            new Ray(transform.position, transform.right),
            new Ray(transform.position, -transform.right)
        };

        StartCoroutine(FallAsleep());
        //gameObject.SetActive(false);
    }

    /// <summary>
    /// Check if surrounded, and sleep
    /// </summary>
    /// <returns></returns>
    IEnumerator FallAsleep()
    {
        //yield return new WaitForEndOfFrame();
        while (true)
        {
            yield return new WaitForSeconds(data.timeslots);
            if (changeStateIfCave())
            {
                break;
            }
        }
    }


}
