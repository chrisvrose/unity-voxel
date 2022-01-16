using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TinyBlocks : GenericBlock
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.LogWarning("Coll Warn"+ collision.collider.name);
        //if (collision.transform == Data.player.transform)
        if(collision.transform.TryGetComponent( out Player p))
        {
            //data.player.GetComponent<player>().ModifyInventory(getBlockType(), 1);
            //Destroy(transform);
            //print( GetBlockType() );
        }
    }
    // Use this for initialization
    public new void Start() 
    {
        base.Start();
        if (!isServer) return;
        StartCoroutine(killMe());
    }

    IEnumerator killMe()
    {
        float randomValue = Random.value;
        //Debug.Log(randomValue);
        yield return new WaitForSeconds(randomValue * 200f);
        NetworkServer.Destroy(gameObject);
        //Destroy(gameObject);
    }
}
