using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    GameObject player;

    

    [Range(1, 8)]
    public float timeMultiplier;

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
        Data.player_prefab = Resources.Load("Prefab/Player") as GameObject;
        Data.block = Resources.Load("Prefab/Block") as GameObject;
        Data.block_particle = Resources.Load("Prefab/particle_block") as GameObject;
        Data.chunkManager = GetComponent<ChunkManager>();

        for(int i = 0; i < 6; i++)
        {
            Data.materials.Add(Resources.Load("Materials/" + i) as Material);
        }


        Data.gendegen_rate = (short)(1f / (4 * Time.deltaTime));
        Data.timeslots = (short)(Data.gendegen_rate / 4);
        #endregion

        //Start work
        Data.player = Instantiate(Data.player_prefab, new Vector3(0, 35*(1+Data.timeslots), 0), Quaternion.identity) as GameObject;
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

    

    /// <summary>
    /// If player is too far off, disable chunks, and reenable them accordingly.
    /// </summary>
    /// <returns></returns>
    IEnumerator ChangeChunkState()
    {
        int GenerationRadius = Data.chunkManager.generationRadius;
        //yield return new WaitForEndOfFrame();
        while (true)
        {
            //Debug.Log(ChunkManager.GetChunkSpace(data.player.transform.position));
            for (int x = -GenerationRadius; x <= GenerationRadius; x++)
            {
                for (int y = -GenerationRadius; y <= GenerationRadius; y++)
                {
                    //get chunk around player chunks
                    GameObject i = Data.chunkManager.getChunk(Data.player.transform.position + new Vector3(x,0,y) * ChunkManager.chunkSize);
                    if (i)
                    {
                        ActiveChunks.Add(i);
                    }
                }

            }

            // Actively gimp
            foreach(GameObject x in ActiveChunks)
            {
                if (OldChunks.Contains(x))
                    OldChunks.Remove(x);
            }
            

            //Debug.Log(OldChunks.Count);

            foreach (GameObject c in ActiveChunks)
                c.GetComponent<Chunk>().changeState(true);

            foreach (GameObject c in OldChunks)
                c.GetComponent<Chunk>().changeState(false);

            OldChunks.Clear();
            OldChunks.AddRange(ActiveChunks);
            ActiveChunks.Clear();
            yield return new WaitForEndOfFrame();
        }
    }

}
