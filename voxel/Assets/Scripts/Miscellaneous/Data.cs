using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum blocktypes { Invalid,Grass, Dirt, Stone, Glass,Metal }
public static class Data
{
    //Resources
    //public static GameObject chunkPrefab;     //Assigned in Environment.cs

    public static GameObject blockPrefab;     //Assigned in Environment.cs
    public static GameObject blockParticlePrefab;        //Assigned in Environment.cs
    // <summary>
    /// Player prefab
    /// </summary>
    //public static GameObject playerPrefab;     //Assigned in Environment.cs


    //Runtime stuff
    //public static GameObject player;        //Assigned in Environment.cs, access camera through myCamera
    //public static ChunkManager chunkManager;        //Assigned in Environment
    /// <summary>
    /// List of blocktypes that are available for use.
    /// Assigned in Environment.
    /// </summary>
    //public static List<Material> materials = new List<Material>();


    //Generated stuff
    //public static int seed;             //Assigned in Environment.cs, access camera through myCamera
    //public static uint gendegen_rate;

    //public static short timeslots;
    /// <summary>
    /// Non transparent layer mask
    /// </summary>
    public static int hardBlockLayerMask = 1 << 9;
    public static int transparentLayerMask = 1 << 10;
    /// <summary>
    /// All blocks layer mask
    /// </summary>
    public static int blocklayermask = hardBlockLayerMask|transparentLayerMask;
}