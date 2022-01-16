using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TinyBlocks : GenericBlock
{
    
    // Use this for initialization
    public new void Start() 
    {
        base.Start();
        if (!isServer) return;
        StartCoroutine(killMe());
    }


    /// <summary>
    /// commit toaster bath
    /// </summary>
    [Server]
    public void Terminate()
    {
        NetworkServer.Destroy(gameObject);
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
