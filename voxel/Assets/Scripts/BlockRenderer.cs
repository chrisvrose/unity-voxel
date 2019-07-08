using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent( typeof(MeshFilter) )]
public class BlockRenderer : MonoBehaviour
{
    public GameObject[] CombineMeshes;
    private void Start()
    {
        /*for(int i = 0; i < 6; i++)
        {
            CombineMeshes.Add(new GameObject());
        }
        */
    }

    public void UpdateMesh(Item item)
    {
        GameObject[] collection = GameObject.FindGameObjectsWithTag("Chunk");
        List<CombineInstance> combines = new List<CombineInstance>();
        foreach(GameObject chunk in collection)
        {
            MeshFilter[] filters = chunk.GetComponentsInChildren<MeshFilter>();
            foreach(MeshFilter m in filters)
            {
                if (m.GetComponent<Block>() == null)
                {
                    continue;
                }
                if (item.Type == m.GetComponent<Block>().GetBlockType() && m.GetComponent<Block>().GetCave() )
                {
                    combines.Add(new CombineInstance { mesh = m.sharedMesh, transform = m.transform.localToWorldMatrix });
                }
            }
        }
        (CombineMeshes[(int)item.Type].GetComponent<MeshFilter>().sharedMesh = new Mesh()).CombineMeshes(combines.ToArray());
    }
}
