using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Generic block implementation. On creation server sets the blocktype which gets caught by the updateHook and run
/// </summary>
public abstract class GenericBlock : NetworkBehaviour {
    public enum PlaceBlockType { BLOCK, TINYBLOCK };

    //public Material[] availableMaterials;
    protected ChunkManager chunkManager;

    [SyncVar(hook ="updateItemSet")]
    public blocktypes blockType;
    

    public void Start(){

        chunkManager = GameObject.FindGameObjectWithTag("WorldLight").GetComponent<ChunkManager>();
        
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        //GameObject parentObject = NetworkServer.FindLocalObject(parentNetId);
        //transform.SetParent(parentObject.transform);
        //transform.position += parentPosition;
    }
    
    
    /// <summary>
    /// Create a block at a given position, within a parent chunk
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="block"></param>
    /// <param name="pos"></param>
    /// <param name="UpdateMesh"></param>
    /// <returns></returns>
    [Server]
    public static void Blockinit(GameObject prefab,blocktypes block, Vector3 pos)
    {
        //GameObject sblock = Instantiate(prefab, pos, Quaternion.identity, parent);
        GameObject sblock = Instantiate(prefab, pos, Quaternion.identity);
        
        NetworkServer.Spawn(sblock);
        
        var gb = sblock.GetComponent<GenericBlock>();

        // dirty this, and update the mesh
        gb.blockType = block;

    }


    /// <summary>
    /// Get type of block used
    /// </summary>
    /// <returns>Type of block</returns>
    public blocktypes GetBlockType()
    {
        return this.blockType;
    }
    


    ///// <summary>
    ///// Upon generation, call function to set the block type.
    ///// </summary>
    ///// <param name="block"></param>
    //[ClientRpc]
    //private void setBlockTypeClientRpc(blocktypes block)
    //{
    //    setBlockTypeClient(block);
    //}

    public void updateItemSet(blocktypes oldBlockType,blocktypes newBlockType)
    {
        //Debug.Log("[dirty] update");
        setBlockMaterialClient(newBlockType);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="block"></param>
    [Client]
    private void setBlockMaterialClient(blocktypes block)
    {
        var availableMaterials = GameObject.FindGameObjectWithTag("WorldLight").GetComponent<ChunkManager>().availableMaterials;
        //Debug.Log("Created " + block.ToString());
        
        Material mat = availableMaterials[(int)block];
        // Skip this
        if (TryGetComponent<Renderer>(out Renderer R))
            R.material = mat;
        // TODO Check if already done
        if (mat.HasProperty("_Metallic"))
        {
            ReflectionProbe rfProbe = gameObject.AddComponent(typeof(ReflectionProbe)) as ReflectionProbe;
            rfProbe.mode = UnityEngine.Rendering.ReflectionProbeMode.Realtime;
            rfProbe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.OnAwake;
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
