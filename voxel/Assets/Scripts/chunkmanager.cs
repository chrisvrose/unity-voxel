using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class chunkManager : MonoBehaviour {
    static readonly int[] GenesisIntesity = { 2,16 };
    static readonly float[] GenesisScale = { 8f, 16f };
    public static short ChunkSize = 16;
    public static readonly short seedf = 567;
    public static readonly Vector2 GenesisDisplacement = new Vector2(seedf % 100, seedf / 100);

    private static List<GameObject> Chunks = new List<GameObject>();
    private static List<Vector2> ChunksLocation = new List<Vector2>();

    public static int CreateChunk(Vector2 location)
    {
        GameObject chunk = new GameObject("Chunk " + location);
        
        //StartCoroutine(Generate(chunk.transform));
        //Instantiate(chunk);
        return 1;
    }
    void Start()
    {
        // Well, we have created a new chunk. Time to generate it.
        StartCoroutine(Generate(transform));
        // Welp, all done
    }
    void Update()
    {
        
    }

    IEnumerator Generate(Transform parent)
    {
        // Instantiate
        //ChunksLocation.Add(location);
        short numberOfInstances = 0;

        for (int x = 0; x < chunkManager.ChunkSize; x++)
        {
            for (int z = 0; z < chunkManager.ChunkSize; z++)
            {
                // Compute a Wavy height. There is a /5f of intensity 5 and a /20f of intensity 20
                //short height = (short)(Mathf.PerlinNoise((chunkpoint.x + x) / 5f, (chunkpoint.y + z) / 5f) * 5 + Mathf.PerlinNoise(x / 20f, z / 20f) * 20);
                short height = chunkManager.CalculateHeight(GenesisDisplacement.x + parent.position.x + x, GenesisDisplacement.y + parent.position.y + z);
                //print(height);
                Block.Blockinit(blocktypes.Grass, new Vector3(x, height--,z),parent);      //Build Grass and remove 1 from height
                //float height = Random.Range(0, SizeY);
                for (int y = 0; y <= height; y++)
                {
                    // Chuck in a block
                    Block.Blockinit(blocktypes.Dirt, new Vector3(x, y, z),parent);
                    // Increment numberOfInstances
                }
                numberOfInstances++;

                // If the number of instances per frame was met
                if (numberOfInstances == data.gendegen_rate)
                {
                    // Reset numberOfInstances
                    numberOfInstances = 0;
                    // Wait for next frame
                    yield return new WaitForEndOfFrame();
                }
            }
        }
    }


    public static short CalculateHeight(float x, float y)
    {
        short height = 0;
        for (int i = 0; i < GenesisScale.Length; i++)
        {
            height += (short)(Mathf.PerlinNoise(x / GenesisScale[i], y / GenesisScale[i]) * GenesisIntesity[i]);
        }
        return height;
    }
}
