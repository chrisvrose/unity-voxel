﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Texture2D crosshairTexture;
    Rect position;

    public Camera myCamera;
    public CharacterController character;


    public const float interact_disance = 10f;
    public float camera_sensitivity = 1f;
    public float movement_sensitivity = 2f;
    public float jump_sensitivity = 10f;
    public const float gravity = 9.81f;
    
    public short selected;
    bool[] hasPressed;

    private Vector3 rotate_vector;
    private Vector3 movement_vector;
    private Vector3 try_to_move;
    private float camera_rotation = 0f;

    /// <summary>
    /// Universal usable variables, setup
    /// </summary>
    void Start()
    {
        #region Crosshair
        Cursor.lockState = CursorLockMode.Locked;
        crosshairTexture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
        position = new Rect((Screen.width - crosshairTexture.width) / 2, (Screen.height - crosshairTexture.height) / 2, crosshairTexture.width, crosshairTexture.height);
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
        character = GetComponent<CharacterController>();
        hasPressed = new bool[7];
        StartCoroutine(PlaceStuff());
        
    }

    /// <summary>
    /// GUI crosshair
    /// </summary>
    void OnGUI()
    {
        GUI.DrawTexture(position, crosshairTexture);
    }

    /// <summary>
    /// Change Rotation of camera and apply a fix for double placement of blocks
    /// </summary>
    void Update()
    {
        // Realistic method
        transform.Rotate(0, Input.GetAxis("Mouse X") * camera_sensitivity, 0);
        camera_rotation -= Input.GetAxis("Mouse Y") * camera_sensitivity;
        
        camera_rotation = Mathf.Clamp(camera_rotation, -89.8f, 89.8f);
        myCamera.transform.localRotation = Quaternion.Euler(camera_rotation,0,0);
        

        // init velocity
        
        float buff = try_to_move.y;
        try_to_move.Set(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        try_to_move = myCamera.transform.rotation * (try_to_move * movement_sensitivity);
        
        // Move the spherical coordinates into the x-z plane
        try_to_move = Vector3.ProjectOnPlane(try_to_move, Vector3.up).normalized * try_to_move.magnitude;
        // v = u-gt
        try_to_move.y = buff -  gravity * Time.deltaTime;
        //Debug.Log(try_to_move.y);

        
        //Do jump stuff and reset y velocity
        if (character.isGrounded) {
            try_to_move.y = 0+Input.GetAxis("Jump")*jump_sensitivity;            
        }
        // dS = vdt
        character.Move(try_to_move * Time.deltaTime);


        // This pass helps remove double interactions, when the fps is low
        hasPressed[0] = hasPressed[0] || Input.GetMouseButtonDown(0);
        hasPressed[1] = hasPressed[1] || Input.GetMouseButtonDown(1);
        hasPressed[2] = hasPressed[2] || Input.GetKey("1");
        hasPressed[3] = hasPressed[3] || Input.GetKey("2");
        hasPressed[4] = hasPressed[4] || Input.GetKey("3");
        hasPressed[5] = hasPressed[5] || Input.GetKey("4");
        hasPressed[6] = hasPressed[6] || Input.GetKey("5");

    }



    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Debug.Log("HITC:" + hit.transform.name);
        if(hit.gameObject.GetComponent<TinyBlocks>() is TinyBlocks)
        {
            blocktypes blockTypeHere = hit.gameObject.GetComponent<TinyBlocks>().GetBlockType();
            Destroy(hit.gameObject);
            //inventory

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
                RaycastHit hit;
                Ray ray = myCamera.ScreenPointToRay(new Vector3(myCamera.pixelWidth / 2, myCamera.pixelHeight / 2, 0));
                if (Physics.Raycast(ray, out hit, interact_disance, Data.blocklayermask))
                {
                    Transform hit_object = hit.transform;
                    //Debug.Log(hit.normal);
                    Vector3 place_pos = hit_object.position + hit.normal;
                    if (!hasPressed[0])
                    {
                        //Debug.Log("Asked to spawn");
                        // Note this convolution here is if not using a block already
                        Block.Blockinit(Data.block,(blocktypes)selected, place_pos, Data.chunkManager.getChunk(place_pos).transform);
                        // Workaround to lots of math, just get parent of hit
                        //Block.Blockinit(data.block, (blocktypes)selected, place_pos, hit.transform.GetComponentInParent<Transform>());
                    }
                    else
                    {
                        hit.transform.GetComponent<Block>().BlockDestroy();

                    }
                }
                
            }
            // Reset the request
            hasPressed = new bool[hasPressed.Length];

            yield return new WaitForFixedUpdate();//WaitForSeconds(0.25f);
        }
    }

    
}
