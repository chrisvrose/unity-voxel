using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
[DisallowMultipleComponent]
public class DistanceInterestException : NetworkBehaviour
{
    /// <summary>
    /// Whether the gameobject will always be visible or not
    /// </summary>
    [Tooltip("Whether we will always be visible or not")]
    public bool isException= true;
}
