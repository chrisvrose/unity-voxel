using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Environment : NetworkBehaviour
{
    /// time sensitivity
    [Range(.03125f, 8)]
    public float timeAdder;

    /// time represented as degrees
    float currentTime;


    [Range(0,90)]
    public float slant;

    Light myLightComponent;
    // Use this for initialization
    void Start()
    {
        if (!isServer) {
            return;
        }

        currentTime = 0;
        myLightComponent = GetComponent<Light>();
        #region Load Prefabs and set generation timeslot intervals



        //On 30 fps that's two instantiate only
        //Data.timeslots = (short)(Data.gendegen_rate / 4);
        #endregion
        //Start work
        //Data.player = Instantiate(Data.blockParticlePrefab, new Vector3(0, 35 * (1 + Data.timeslots), 0), Quaternion.identity);
        //NetworkServer.Spawn(Data.player);   
        //Instantiate(Data.playerPrefab, new Vector3(0, 35*(1+Data.timeslots), 0), Quaternion.identity) as GameObject;
        // StartCoroutine(Generation());
        StartCoroutine(CycleTime());
        // Managing Chunk states
        //StartCoroutine(ChangeChunkState());
    }


    private void Update()
    {
        if (!isServer) return;
        var newTime = currentTime + timeAdder * Time.deltaTime;
        //transform.Rotate(Vector3.right * timeMultiplier);
        if ((newTime) > 360) currentTime = 0;
        else
        currentTime =newTime;

        transform.rotation = Quaternion.Euler(currentTime, slant * Mathf.Sin(currentTime * Mathf.Deg2Rad), 0);
        //Debug.Log(transform.eulerAngles.x);
        bool night = transform.eulerAngles.x > 180;
        myLightComponent.intensity = !night ? 1 : 0;
        RenderSettings.ambientIntensity = !night ? 0.5f : 0;
    }


    /// <summary>
    /// Cycle through day and night cycles. Change light intensities and ambience to make it look better
    /// </summary>
    /// <returns></returns>
    [Server]
    IEnumerator CycleTime()
    {
        while (true)
        {
            
            yield return new WaitForSeconds(.25f);
        }   
    }
    

}
