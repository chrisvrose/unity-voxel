using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class environment : MonoBehaviour
{
    GameObject player;
    //GameObject defblock;

    [Range(1, 8)]
    public int generate_radius;

    short instancesPerFrame;

    [Range(-1, 9999)]
    public int seedf;

    Vector2 Genesis_Displacement;

    List<Vector2> generated_chunks;

    // Use this for initialization
    void Start()
    {

        if (PlayerPrefs.HasKey("seed") && seedf == -1)
        {
            data.seed = PlayerPrefs.GetInt("seed");
            //Debug.Log(seedf);
        }
        else
        {
            PlayerPrefs.SetInt("seed", Random.Range(0, 9999));
        }

        //Generate initial point from seed
        Genesis_Displacement = new Vector2(seedf % 100, seedf / 100);

        #region Load, set and print data
        data.player_prefab = Resources.Load("Prefab/Player") as GameObject;
        player = data.player_prefab;
        data.player = player;
        data.block = Resources.Load("Prefab/Block") as GameObject;
        //defblock = data.block;
        data.block_particle = Resources.Load("Prefab/particle_block") as GameObject;
        data.gendegen_rate = (short)(1f / (4 * Time.deltaTime));
        data.timeslots = (short)(data.gendegen_rate / 4);
        data.blocklayermask = 1 << 9 | 1<<10;
        data.hardblocklayermask = 1 << 9;
        Debug.Log("Rate:" + data.gendegen_rate);

        generated_chunks = new List<Vector2>();
        #endregion

        instancesPerFrame = data.gendegen_rate;
        //Start work
        StartCoroutine(generate(Vector2.zero));


        StartCoroutine(CycleTime());
        //StartCoroutine(generate(new Vector2(16, 0)));
    }

    // Update is called once per frame
    void Update()
    {

    }


    IEnumerator generate(Vector2 chunkpoint)
    {
        // Instantiate
        generated_chunks.Add(chunkpoint);
        short numberOfInstances = 0;

        for (int x = 0; x < chunkManager.ChunkSize; x++)
        {
            for (int z = 0; z < chunkManager.ChunkSize; z++)
            {
                // Compute a Wavy height. There is a /5f of intensity 5 and a /20f of intensity 20
                //short height = (short)(Mathf.PerlinNoise((chunkpoint.x + x) / 5f, (chunkpoint.y + z) / 5f) * 5 + Mathf.PerlinNoise(x / 20f, z / 20f) * 20);
                short height = chunkManager.calculateHeight(Genesis_Displacement.x +chunkpoint.x + x, Genesis_Displacement.y + chunkpoint.y + z);
                //print(height);
                Block.Blockinit(blocktypes.Grass, new Vector3(chunkpoint.x + x, height--, chunkpoint.y + z));      //Build Grass and remove 1 from height
                //float height = Random.Range(0, SizeY);
                for (int y = 0; y <= height; y++)
                {
                    // Chuck in a block
                    Block.Blockinit(blocktypes.Dirt, new Vector3(chunkpoint.x + x, y, chunkpoint.y + z));
                    // Increment numberOfInstances
                }
                numberOfInstances++;

                // If the number of instances per frame was met
                if (numberOfInstances == instancesPerFrame)
                {
                    // Reset numberOfInstances
                    numberOfInstances = 0;
                    // Wait for next frame
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        //First time generation
        if (chunkpoint == Vector2.zero)
        {
            data.player = Instantiate(player, new Vector3(chunkpoint.x, 35, chunkpoint.y), Quaternion.identity) as GameObject;
            StartCoroutine(GenerateNew());
            data.player_cam = data.player.GetComponentInChildren<Camera>();
        }
    }


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
    IEnumerator GenerateNew()
    {
        Vector2 playerchunka,generateat;
        while (true)
        {
            //Get current chunk player is in
            playerchunka = new Vector2((int)(data.player.transform.position.x / chunkManager.ChunkSize), (int)(data.player.transform.position.z / chunkManager.ChunkSize) );
            
            for(int x = -generate_radius; x <= generate_radius; x++)
            {
                for(int y = -generate_radius; y <= generate_radius; y++)
                {
                    generateat = (playerchunka + new Vector2(x, y))*chunkManager.ChunkSize;
                    
                    if (!generated_chunks.Contains(generateat)) StartCoroutine(generate(generateat));
                    yield return new WaitForSeconds(.05f);
                }
            }
            
            yield return new WaitForSeconds(.5f);
            
        }
    }
}
