using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class Player : NetworkBehaviour
{
    Texture2D crosshairTexture;
    
    Vector2 crossHairBounds;

    //Rect positionCrossLeft, positionCrossRight;


    public Camera myCamera;
    public Transform myCameraHolder;
    public CharacterController character;


    ChunkManager chunkManager;

    /// <summary>
    /// Server controlled list of players
    /// </summary>
    public static readonly List<Player> playersList = new List<Player>();



    public const float interactDistance = 10f;
    public float cameraSensitivity = 1f;
    public const float movementSensitivity = 2.5f;
    public const float jumpSensitivity = 4.6f;
    public const float gravity = 9.81f;
    
    public short selected;
    bool[] hasPressed;


    private Vector3 try_to_move;
    private float camera_rotation = 0f;


    [ServerCallback]
    public override void OnStartServer()
    {
        //base.OnStartServer();
        playersList.Add(this);
    }
    [ServerCallback]
    public override void OnStopServer()
    {
        //base.OnStopServer();

        playersList.Remove(this);
    }
    /// <summary>
    /// Universal usable variables, setup
    /// </summary>
    void Start()
    {
        #region Crosshair
        Cursor.lockState = CursorLockMode.Locked;
        crosshairTexture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
        var offset = myCamera.stereoSeparation / 2;
        var p = new Vector3(offset, 0);
        var camoffset = myCamera.WorldToScreenPoint(p).x;
        crossHairBounds = new Vector2(crosshairTexture.width, crosshairTexture.height);
        //Debug.Log(camoffset);
        //position = new Rect((Screen.width - crosshairTexture.width) / 2, (Screen.height - crosshairTexture.height) / 2, crosshairTexture.width, crosshairTexture.height);
        //positionCrossLeft = new Rect((Screen.width - crosshairTexture.width+ camoffset) / 4, (Screen.height - crosshairTexture.height) / 2, crosshairTexture.width, crosshairTexture.height);
        //positionCrossRight = new Rect(3*(Screen.width - crosshairTexture.width- camoffset) / 4, (Screen.height - crosshairTexture.height) / 2, crosshairTexture.width, crosshairTexture.height);
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                crosshairTexture.SetPixel(i, j, Color.white);
            }

        }
        crosshairTexture.Apply();
        #endregion
        #region Inventory initialization
        //inventory = new short[5];
        selected = 1;
        //for (int i = 0; i > 5; i++) inventory[i] = 5;
        #endregion
        try_to_move = new Vector3(0, 0, 0);
        chunkManager = GameObject.FindGameObjectWithTag("WorldLight").GetComponent<ChunkManager>();

        character = GetComponent<CharacterController>();
        hasPressed = new bool[7];
        StartCoroutine(PlaceStuff());
        
    }

    /// <summary>
    /// GUI crosshair
    /// </summary>
    void OnGUI()
    {
        Ray ray = myCamera.ScreenPointToRay(new Vector3(myCamera.pixelWidth / 2, myCamera.pixelHeight / 2, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, Data.blocklayermask))
        {
            var hitPosition = hit.point;
            var leftPoint = myCamera.WorldToScreenPoint(hitPosition,Camera.MonoOrStereoscopicEye.Left);
            var rightPoint = myCamera.WorldToScreenPoint(hitPosition,Camera.MonoOrStereoscopicEye.Right);
            //Debug.Log(leftPoint + " " + rightPoint+ " "+ myCamera.pixelWidth, this);
            var leftPos = new Vector2(leftPoint.x/myCamera.pixelWidth* (Screen.width / 2f), leftPoint.y / myCamera.pixelHeight * (Screen.height));
            var rightPos = new Vector2(rightPoint.x/myCamera.pixelWidth* (Screen.width / 2f)+Screen.width/2f, rightPoint.y / myCamera.pixelHeight * (Screen.height));
            
            var leftRect = new Rect(leftPos, crossHairBounds);
            var rightRect = new Rect(rightPos, crossHairBounds);
            GUI.DrawTexture(leftRect, crosshairTexture);
            GUI.DrawTexture(rightRect, crosshairTexture);
            //Debug.Log(":" + myCamera.pixelWidth + " " + Screen.width);
        }
        //GUI.DrawTexture(positionCrossLeft, crosshairTexture);
        //GUI.DrawTexture(positionCrossRight, crosshairTexture);
    }

    /// <summary>
    /// Change Rotation of camera and apply a fix for double placement of blocks
    /// </summary>
    void Update()
    {
        #region movement
        // Realistic method
        transform.Rotate(0, Input.GetAxis("Mouse X") * cameraSensitivity, 0);
        camera_rotation -= Input.GetAxis("Mouse Y") * cameraSensitivity;
        
        camera_rotation = Mathf.Clamp(camera_rotation, -89.8f, 89.8f);
        myCameraHolder.transform.localRotation = Quaternion.Euler(camera_rotation,0,0);
        

        // init velocity
        
        float buff = try_to_move.y;
        try_to_move.Set(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        try_to_move = myCameraHolder.transform.rotation * (try_to_move * movementSensitivity);
        
        // Move the spherical coordinates into the x-z plane
        try_to_move = Vector3.ProjectOnPlane(try_to_move, Vector3.up).normalized * try_to_move.magnitude;
        // v = u-gt
        try_to_move.y = buff -  gravity * Time.deltaTime;
        //Debug.Log(try_to_move.y);

        
        //Do jump stuff and reset y velocity
        if (character.isGrounded) {
            try_to_move.y = 0+Input.GetAxis("Jump")*jumpSensitivity;            
        }
        // dS = vdt
        character.Move(try_to_move * Time.deltaTime);
        if (transform.position.y < -256)
        {
            //clearly fell. lift up and reset velocity
            transform.position += 512 * Vector3.up;
            try_to_move.y = 0;
        }
        #endregion

        #region action queueing
        // This pass helps remove double interactions, when the fps is low
        hasPressed[0] |= Input.GetMouseButtonDown(0);
        hasPressed[1] |= Input.GetMouseButtonDown(1);
        hasPressed[2] |= Input.GetKey("1");
        hasPressed[3] |= Input.GetKey("2");
        hasPressed[4] |= Input.GetKey("3");
        hasPressed[5] |= Input.GetKey("4");
        hasPressed[6] |= Input.GetKey("5");
        #endregion

    }



    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Debug.Log("HITC:" + hit.transform.name);
        if (isClient && hit.gameObject.TryGetComponent(out TinyBlocks tinyblock))
        {
            //hit.gameObject.GetComponent<TinyBlocks>()
            blocktypes blockTypeHere = tinyblock.GetBlockType();
            uint netid = tinyblock.netId;
            //Debug.Log(blockTypeHere.ToString());
            CmdTinyBlockCollided(netid);
            
            //TODO inventory

        }
    }
    [Command]
    private void CmdTinyBlockCollided(uint netid)
    {
        if(NetworkServer.spawned.TryGetValue(netid, out NetworkIdentity obj) && obj.TryGetComponent(out TinyBlocks tb))
        {
            tb.Terminate();
        }
    }

    /// <summary>
    /// Every now and again this enumerator will trigger and do the placements and stuff. 
    /// </summary>
    /// <returns></returns>
    IEnumerator PlaceStuff()
    {
        for(; ; )
        {
            if( hasPressed[2]|| hasPressed[3] || hasPressed[4] || hasPressed[5] || hasPressed[6] )
            {
                selected = (short) ( hasPressed[2] ? 1 : (hasPressed[3] ? 2 :(hasPressed[4] ? 3 : (hasPressed[5] ? 4 : (hasPressed[6] ? 5 : 1 )))) );
                
            }
            if (hasPressed[0] || hasPressed[1])
            {
                //Debug.Log("Recorded " + hasPressed[0]);
                //int button = hasPressed[0] ? 0 : 1;
                Ray ray = myCamera.ScreenPointToRay(new Vector3(myCamera.pixelWidth / 2, myCamera.pixelHeight / 2, 0));
                if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, Data.blocklayermask))
                {
                    Transform hit_object = hit.transform;
                    //Debug.Log("hit");
                    //Debug.Log(hit.normal);
                    Vector3 place_pos = hit_object.position + hit.normal;
                    if (!hasPressed[0])
                    {
                        // only place blocks if the hit was a Block
                        if (hit.transform.TryGetComponent(out Block component))
                            CmdBlockInit(GenericBlock.PlaceBlockType.BLOCK, (blocktypes)selected, place_pos);
                    }
                    else
                    {
                        //Debug.Log("Called to Destroy");
                        if(hit.transform.TryGetComponent(out Block component))
                        {
                            var netid = component.netId;
                            //Debug.Log("Block found with netid:"+ netid);

                            CmdBlockDestroy(netid);
                        }
                        else
                        {
                            Debug.Log("No Block on hit component", hit.transform);
                        }
                        
                    }
                }
                else
                {
                    Debug.Log("Not hit");
                }
                
            }
            
            // Reset the request
            hasPressed = new bool[hasPressed.Length];
            
            yield return new WaitForFixedUpdate();//WaitForSeconds(0.25f);
        }
    }

    [Command]
    public void CmdBlockInit(GenericBlock.PlaceBlockType placeBlockType, blocktypes block, Vector3 pos)
    {
        chunkManager.BlockInit(placeBlockType, block, pos);
    }
    /// <summary>
    /// Delete a block
    /// </summary>
    /// <param name="netid">netid of the block</param>
    [Command]
    public void CmdBlockDestroy(uint netid)
    {
        if(NetworkServer.spawned.TryGetValue(netid, out NetworkIdentity obj) && obj.TryGetComponent(out Block block)){ 
            block.BlockDestroy();
        }
    }


}
