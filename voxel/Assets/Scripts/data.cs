using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum blocktypes { Grass = 1, Dirt, Stone, Glass }
public static class data
{
    //Resources
    public static GameObject chunkPrefab;     //Assigned in environment.cs
    public static GameObject block;     //Assigned in environment.cs
    public static GameObject block_particle;        //Assigned in environment.cs
    public static GameObject player_prefab;     //Assigned in environment.cs
    //Runtime stuff
    public static GameObject player;        //Assigned in environment.cs, access camera through myCamera
    //Generated stuff
    public static int seed;
    public static short gendegen_rate;
    public static short timeslots;
    public static int hardblocklayermask;
    public static int blocklayermask;
}