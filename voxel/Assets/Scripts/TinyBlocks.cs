using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinyBlocks : GenericBlock
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.LogWarning("Coll Warn"+ collision.collider.name);
        if (collision.transform == Data.player.transform)
        {
            //data.player.GetComponent<player>().ModifyInventory(getBlockType(), 1);

            Destroy(transform);
            //print( GetBlockType() );
        }
    }
    // Use this for initialization
    protected void Start() 
    {
        StartCoroutine("killMe");
    }

    IEnumerator killMe()
    {
        float randomValue = Random.value;
        //Debug.Log(randomValue);
        yield return new WaitForSeconds(randomValue * 200f);
        Destroy(gameObject);
    }
}
