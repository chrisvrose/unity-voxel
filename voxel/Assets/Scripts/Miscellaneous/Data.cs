using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum blocktypes { Invalid,Grass, Dirt, Stone, Glass }
public static class Data
{
    //Resources
    public static GameObject chunkPrefab;     //Assigned in Environment.cs
    public static GameObject block;     //Assigned in Environment.cs
    public static GameObject block_particle;        //Assigned in Environment.cs
    public static GameObject player_prefab;     //Assigned in Environment.cs
    //Runtime stuff
    public static GameObject player;        //Assigned in Environment.cs, access camera through myCamera
    public static ChunkManager chunkManager;        //Assigned in Environment
    public static List<Material> materials = new List<Material>();
    //Generated stuff
    public static int seed;             //Assigned in Environment.cs, access camera through myCamera
    public static uint gendegen_rate;
    public static short timeslots;
    public static int hardblocklayermask = 1 << 9;
    public static int blocklayermask = 1 << 9 | 1 << 10;
}