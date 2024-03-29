﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// A variant of the distance based measurements with 2 differences:
/// 1. Exception: Tag objects to skip this distance measuring
/// 2. sqrMagnitude
/// </summary>
public class DistanceBasedInterestManagementWithExceptions : InterestManagement
{
    [Tooltip("The maximum range that objects will be visible at. Add DistanceInterestManagementCustomRange onto NetworkIdentities for custom ranges.")]
    public int visRange = 10;

    [Tooltip("Rebuild all every 'rebuildInterval' seconds.")]
    public float rebuildInterval = 1;
    double lastRebuildTime;

    Vector3Int mask = Vector3Int.one - Vector3Int.up;

    // helper function to get vis range for a given object, or default.
    int GetVisRange(NetworkIdentity identity)
    {
        DistanceInterestManagementCustomRange custom = identity.GetComponent<DistanceInterestManagementCustomRange>();
        return custom != null ? custom.visRange : visRange;
    }


    bool GetIsException(NetworkIdentity identity)
    {
        if(identity.TryGetComponent(out DistanceInterestException obj)){
            return obj.isException;
        }
        return false;
    }

    [ServerCallback]
    public override void Reset()
    {
        lastRebuildTime = 0D;
    }

    public override bool OnCheckObserver(NetworkIdentity identity, NetworkConnection newObserver)
    {
        bool isException = GetIsException(identity);
        // just skip if the object is a network transform
        if (isException) return true;
        int range = GetVisRange(identity);
        
        var distvec = Vector3.Scale(identity.transform.position - newObserver.identity.transform.position,mask);
        return distvec.sqrMagnitude < range;
    }

    public override void OnRebuildObservers(NetworkIdentity identity, HashSet<NetworkConnection> newObservers, bool initialize)
    {
        // cache range and .transform because both call GetComponent.
        bool isException = GetIsException(identity);

        int range = GetVisRange(identity);
        Vector3 position = identity.transform.position;

        // brute force distance check
        // -> only player connections can be observers, so it's enough if we
        //    go through all connections instead of all spawned identities.
        // -> compared to UNET's sphere cast checking, this one is orders of
        //    magnitude faster. if we have 10k monsters and run a sphere
        //    cast 10k times, we will see a noticeable lag even with physics
        //    layers. but checking to every connection is fast.
        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            // authenticated and joined world with a player?
            if (conn != null && conn.isAuthenticated && conn.identity != null)
            {
                // check distance, skip check if exception
                if (isException||(Vector3.Scale(conn.identity.transform.position- position,mask).sqrMagnitude < range))
                {
                    newObservers.Add(conn);
                }
            }
        }
    }

    // internal so we can update from tests
    [ServerCallback]
    internal void Update()
    {
        // rebuild all spawned NetworkIdentity's observers every interval
        if (NetworkTime.localTime >= lastRebuildTime + rebuildInterval)
        {
            RebuildAll();
            lastRebuildTime = NetworkTime.localTime;
        }
    }
}
