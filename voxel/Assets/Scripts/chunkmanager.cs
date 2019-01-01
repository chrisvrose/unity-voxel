using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class chunkManager : MonoBehaviour {
    static readonly int[] GenesisIntesity = { 2,16 };
    static readonly float[] GenesisScale = { 8f, 16f };
    public static short ChunkSize = 16;


    private static List<GameObject> chunks = new List<GameObject>();
    private static List<Vector2> chunks_location = new List<Vector2>();
    void Start()
    {
        
    }
    void Update()
    {
        
    }

    public static short calculateHeight(float x, float y)
    {
        short height = 0;
        for (int i = 0; i < GenesisScale.Length; i++)
        {
            height += (short)(Mathf.PerlinNoise(x / GenesisScale[i], y / GenesisScale[i]) * GenesisIntesity[i]);
        }
        return height;
    }
}
