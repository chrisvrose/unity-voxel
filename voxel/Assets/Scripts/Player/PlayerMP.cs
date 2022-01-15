using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Handle multiplayer related state
/// </summary>
public class PlayerMP : NetworkBehaviour
{
    public Behaviour[] toDisableBehaviours;
    public CharacterController characterController;

    /// <summary>
    /// Meshes of the player.
    /// Will be shifted to another layer during runtime.
    /// </summary>
    public Transform[] playerMeshes;
    // Start is called before the first frame update
    void Start()
    {
        if (!isLocalPlayer)
        {
            foreach (var behaviour in toDisableBehaviours)
            {
                behaviour.enabled = false;
            }
            
            characterController.enabled = false;
            foreach (var playerMesh in playerMeshes)
                playerMesh.gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
