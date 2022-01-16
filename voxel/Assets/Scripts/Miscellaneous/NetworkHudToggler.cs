using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class NetworkHudToggler : MonoBehaviour
{
    // Start is called before the first frame update
    public NetworkManagerHUD hud;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            // toggle n/w manager hud
            hud.enabled = !(hud.enabled);
        }
    }
}
