using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator 
{
    protected static readonly Vector3 GenesisIntesity = new Vector3(2,4,16);
    protected static readonly Vector3 GenesisScale = new Vector3(1/2f,1/4f,1/8f);

    public static short CalculateHeight(float x, float y)
    {
        short height = 0;
        for (int i = 0; i < 3; i++)
        {
            height += (short)(Mathf.PerlinNoise(x / GenesisScale[i], y / GenesisScale[i]) * GenesisIntesity[i]);
        }
        return height;
    }

    public static Vector2 getRandomCoordinate(float range){
        return new Vector2(Random.Range(0,range),Random.Range(0,range));
    }
}
