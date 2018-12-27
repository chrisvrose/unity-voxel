using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tiny_blocks : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform == data.player.transform)
        {
            data.player.GetComponent<player>().modifyInventory(getBlockType(), 1);

            Destroy(GetComponent<GameObject>());
            //print( getBlockType() );
        }
    }
    // Use this for initialization
    void Start()
    {
        StartCoroutine(killme());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator killme()
    {
        yield return new WaitForSeconds(Random.value * 200f);
        Destroy(gameObject);
    }


    public static GameObject Blockinit(blocktypes block, Vector3 pos)
    {
        //nblock = Resources.Load("Prefab/Block") as GameObject;

        GameObject sblock = Instantiate(data.block_particle, pos + Random.insideUnitSphere / 2f, Quaternion.Euler(Quaternion.identity.eulerAngles + Random.insideUnitSphere * 60f));
        sblock.GetComponent<tiny_blocks>().setBlockType(block);

        Material mat = sblock.GetComponent<Renderer>().material;
        if (mat.GetColor("_EmissionColor") != (new Color(0, 0, 0)))
        {
            sblock.GetComponent<Light>().enabled = true;
            sblock.GetComponent<Light>().color = mat.GetColor("_EmissionColor");
        }
        //Debug.Log(mat.color);

        return sblock;
    }
    private void setBlockType(blocktypes block)
    {
        Material mat = Resources.Load("Materials/" + (int)block) as Material;
        GetComponent<Renderer>().material = mat;


    }
    public blocktypes getBlockType()
    {
        Debug.Log(GetComponent<Renderer>().material.name);
        int a;
        if (int.TryParse(GetComponent<Renderer>().material.name, out a))
            return (blocktypes)a;
        else return (blocktypes)1;
        //return (blocktypes)int.Parse(GetComponent<Renderer>().material.name);
    }
}
