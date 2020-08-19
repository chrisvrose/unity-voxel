using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericBlock : MonoBehaviour {
    protected Item BaseItem;
    public Item item {
        get
        {
            return BaseItem;
        }
        set{
            BaseItem = item;
            //update the item
            StartCoroutine(GetComponentInParent<Chunk>().DelayedUpdateMesh());
        }
    }

    public void Start(){
        
    }
    
    /// <summary>
    /// Create a block at a given position, within a parent chunk
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="block"></param>
    /// <param name="pos"></param>
    /// <param name="parent"></param>
    /// <param name="UpdateMesh"></param>
    /// <returns></returns>
    public static GameObject Blockinit(GameObject prefab,blocktypes block, Vector3 pos, Transform parent,bool UpdateMesh=true)
    {
        //if (prefab == null) prefab = data.block;
        GameObject sblock = Instantiate(prefab, pos, Quaternion.identity, parent);
        
        sblock.GetComponent<GenericBlock>().BaseItem = new Item(block);
        //Do some additional setup
        sblock.GetComponent<GenericBlock>().setBlockType(block);
        sblock.name = block.ToString()+ " Block";

        if (UpdateMesh)
        {
            Transform p = parent;
            p.GetComponent<Chunk>().UpdateMesh();
        }
        return sblock;
    }


    /// <summary>
    /// Get type of block used
    /// </summary>
    /// <returns>Type of block</returns>
    public blocktypes GetBlockType()
    {
        return BaseItem.Type;
    }
    


    /// <summary>
    /// Upon generation, call function to set the block type.
    /// </summary>
    /// <param name="block"></param>
    private void setBlockType(blocktypes block)
    {
        BaseItem.Type = block;
        Material mat = Data.materials[(int)block];
        // Skip this
        //GetComponent<Renderer>().material = mat;
        // TODO Check if already done
        if (mat.HasProperty("_Metallic"))
        {
            ReflectionProbe rfProbe = gameObject.AddComponent(typeof(ReflectionProbe)) as ReflectionProbe;
            rfProbe.enabled = true;
        }
        if (mat.GetColor("_EmissionColor") != (new Color(0, 0, 0)))
        {
            Light objectLight = gameObject.AddComponent(typeof(Light)) as Light;
            objectLight.enabled = true;
            objectLight.color = mat.GetColor("_EmissionColor");
        }
        // If transparent move to another layer
        if (mat.color.a != 1f) gameObject.layer = 10;

    }
    
}
