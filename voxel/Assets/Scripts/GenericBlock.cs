using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericBlock : MonoBehaviour {
    public blocktypes type;
    public static GameObject selfObject = null;
    
    /// <summary>
    /// Take care of block generation
    /// </summary>
    /// <param name="prefab">What Prefab to use</param>
    /// <param name="block">What form of block to use</param>
    /// <param name="pos">Position</param>
    /// <param name="parent">Parent chunk</param>
    /// <returns></returns>
    public static GameObject Blockinit(GameObject prefab,blocktypes block, Vector3 pos, Transform parent)
    {
        //if (prefab == null) prefab = data.block;
        GameObject sblock = Instantiate(prefab, pos, Quaternion.identity, parent);
        sblock.GetComponent<GenericBlock>().setBlockType(block);
        Material mat = sblock.GetComponent<Renderer>().material;
        sblock.name = block.ToString()+ " Block";
        if (mat.GetColor("_EmissionColor") != (new Color(0, 0, 0)))
        {
            sblock.GetComponent<Light>().enabled = true;
            sblock.GetComponent<Light>().color = mat.GetColor("_EmissionColor");
        }
        
        return sblock;
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
    
}
