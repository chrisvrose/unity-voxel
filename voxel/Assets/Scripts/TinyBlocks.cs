using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinyBlocks : GenericBlock
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.LogWarning("Coll Warn"+ collision.collider.name);
        if (collision.transform == data.player.transform)
        {
            data.player.GetComponent<player>().modifyInventory(getBlockType(), 1);

            Destroy(transform);
            //print( getBlockType() );
        }
    }
    // Use this for initialization
    protected void Start() 
    {
        StartCoroutine(killMe());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator killMe()
    {
        float randomValue = Random.value;
        //Debug.Log(randomValue);
        yield return new WaitForSeconds(randomValue * 200f);
        Destroy(gameObject);
    }

    public blocktypes getBlockType()
    {
        Debug.Log(GetComponent<Renderer>().material.name);
        int a;
        if (int.TryParse(GetComponent<Renderer>().material.name, out a))
        {
            return (blocktypes)a;
        }
        else {
            return (blocktypes)1;
        }
        //return (blocktypes)int.Parse(GetComponent<Renderer>().material.name);
    }
}
