﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class environment : MonoBehaviour
{
    GameObject player;
    //GameObject defblock;

    [Range(1, 8)]
    public int generate_radius;


    [Range(-1, 9999)]
    public int seedf;

    List<GameObject> OldChunks = new List<GameObject>();
    List<GameObject> ActiveChunks = new List<GameObject>();
    // Use this for initialization
    void Start()
    {

        #region Check for seed, and generate one if none
        if (PlayerPrefs.HasKey("seed") && seedf == -1)
        {
            data.seed = PlayerPrefs.GetInt("seed");
            //Debug.Log(seedf);
        }
        else
        {
            PlayerPrefs.SetInt("seed", Random.Range(0, 9999));
        }
        #endregion

        #region Load Prefabs and set generation timeslot intervals
        data.chunkPrefab = Resources.Load("Prefab/Chunk") as GameObject;
        data.player_prefab = Resources.Load("Prefab/Player") as GameObject;
        data.block = Resources.Load("Prefab/Block") as GameObject;
        data.block_particle = Resources.Load("Prefab/particle_block") as GameObject;

        data.gendegen_rate = (short)(1f / (4 * Time.deltaTime));
        data.timeslots = (short)(data.gendegen_rate / 4);
        #endregion

        //Start work
        data.player = Instantiate(data.player_prefab, new Vector3(0, 35, 0), Quaternion.identity) as GameObject;
        StartCoroutine(Generation());
        StartCoroutine(CycleTime());
        StartCoroutine(ChangeChunkState());
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Cycle through day and night cycles. Change light intensities and ambience to make it look better
    /// </summary>
    /// <returns></returns>
    IEnumerator CycleTime()
    {
        while (true)
        {
            
            transform.Rotate(new Vector3(.1f, 0, 0));
            //Debug.Log(transform.eulerAngles.x);
            bool night = transform.eulerAngles.x > 180;
            GetComponent<Light>().intensity = !night ? 1:0;
            RenderSettings.ambientIntensity = !night ? 0.5f : 0;
            yield return new WaitForSeconds(.1f);
        }
    }

    /// <summary>
    /// Check if generation required, call required functions then
    /// </summary>
    /// <returns></returns>
    IEnumerator Generation()
    {
        Vector3 playerchunka,generateat;
        while (true)
        {
            //Get current chunk player is in
            playerchunka = chunkManager.GetChunkSpace(data.player.transform.position);
            
            for(int x = -generate_radius; x <= generate_radius; x++)
            {
                for(int y = -generate_radius; y <= generate_radius; y++)
                {
                    generateat = (playerchunka + new Vector3(x,0, y))*chunkManager.ChunkSize;

                    chunkManager.CreateChunk(generateat);//StartCoroutine(generate(generateat));
                    yield return new WaitForSeconds(.05f);
                }
            }
            
            yield return new WaitForSeconds(.5f);
            
        }
    }

    /// <summary>
    /// If player is too far off, disable chunks, and reenable them accordingly.
    /// </summary>
    /// <returns></returns>
    IEnumerator ChangeChunkState()
    {

        while (true)
        {
            //Debug.Log(chunkManager.GetChunkSpace(data.player.transform.position));
            for (int x = -generate_radius; x <= generate_radius; x++)
            {
                for (int y = -generate_radius; y <= generate_radius; y++)
                {
                    GameObject i = chunkManager.IsChunk(chunkManager.GetChunkRealSpace(data.player.transform.position) + new Vector3(x, 0, y) * chunkManager.ChunkSize);
                    if (i)
                    {
                        
                        ActiveChunks.Add(i);
                    }
                }

            }

            //Actively gimp
            foreach(GameObject x in ActiveChunks)
            {
                if (OldChunks.Contains(x))
                    OldChunks.Remove(x);
            }
            

            //Debug.Log(OldChunks.Count);

            foreach (GameObject c in ActiveChunks)
                c.GetComponent<chunkManager>().changeState(true);

            foreach (GameObject c in OldChunks)
                c.GetComponent<chunkManager>().changeState(false);

            OldChunks.Clear();
            //Debug.Log(OldChunks.Count);
            OldChunks.AddRange(ActiveChunks);
            ActiveChunks.Clear();
            yield return new WaitForEndOfFrame();
        }
    }
}
