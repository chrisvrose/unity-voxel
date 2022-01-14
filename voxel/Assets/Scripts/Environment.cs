using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    [Range(.03125f, 8)]
    public float timeMultiplier;
    /// <summary>
    /// Seed for world generation
    /// </summary>
    [Range(-1, 9999)]
    public int seedf;

    // Use this for initialization
    void Start()
    {

        #region Check for seed, and generate one if none
        if (PlayerPrefs.HasKey("seed") && seedf == -1)
        {
            Data.seed = PlayerPrefs.GetInt("seed");
            //Debug.Log(seedf);
        }
        else
        {
            PlayerPrefs.SetInt("seed", Random.Range(0, 9999));
        }
        #endregion

        #region Load Prefabs and set generation timeslot intervals
        Data.chunkPrefab = Resources.Load("Prefab/Chunk") as GameObject;
        Data.playerPrefab = Resources.Load("Prefab/Player") as GameObject;
        Data.blockPrefab = Resources.Load("Prefab/Block") as GameObject;
        Data.blockParticlePrefab = Resources.Load("Prefab/particle_block") as GameObject;
        Data.chunkManager = GetComponent<ChunkManager>();

        for(var i = 0; i < 6; i++)
        {
            Data.materials.Add(Resources.Load("Materials/" + i) as Material);
        }

        //On 30 fps that's two instantiate only
        Data.gendegen_rate = (uint)(1f / (15 * Time.deltaTime));
        Data.timeslots = (short)(Data.gendegen_rate / 4);
        #endregion

        //Start work
        Data.player = Instantiate(Data.playerPrefab, new Vector3(0, 35*(1+Data.timeslots), 0), Quaternion.identity) as GameObject;
        // StartCoroutine(Generation());
        StartCoroutine(CycleTime());
        // Managing Chunk states
        //StartCoroutine(ChangeChunkState());
    }
    

    /// <summary>
    /// Cycle through day and night cycles. Change light intensities and ambience to make it look better
    /// </summary>
    /// <returns></returns>
    IEnumerator CycleTime()
    {
        while (true)
        {
            transform.Rotate(Vector3.right * timeMultiplier);
            //Debug.Log(transform.eulerAngles.x);
            bool night = transform.eulerAngles.x > 180;
            GetComponent<Light>().intensity = !night ? 1:0;
            RenderSettings.ambientIntensity = !night ? 0.5f : 0;
            yield return new WaitForSeconds(.1f);
        }
    }

}
