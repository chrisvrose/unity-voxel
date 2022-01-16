using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum blocktypes { Invalid,Grass, Dirt, Stone, Glass,Metal }
public static class Data
{
    
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