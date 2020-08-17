using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorIntEquality : IEqualityComparer<Vector3Int>
{
    public bool Equals(Vector3Int a, Vector3Int b){
        return (a.x==b.x && a.y==b.y && a.z==b.z);
    }
    public int GetHashCode(Vector3Int a){
        return a.x^a.y^a.z;
    }
}
