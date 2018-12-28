﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    Texture2D crosshairTexture;
    Rect position;

    public Camera myCamera;
    public CharacterController character;


    public const float interact_disance = 10f;
    public float camera_sensitivity;
    public float movement_sensitivity;
    public float jump_sensitivity;
    public float gravity;

    public short[] inventory;
    public short selected;
    bool[] hasPressed;

    private Vector3 rotate_vector;      //Used by Update to ch
    private Vector3 movement_vector;
    private Vector3 try_to_move;

    // Use this for initialization
    void Start()
    {
        #region Crosshair
        Cursor.lockState = CursorLockMode.Locked;
        crosshairTexture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
        position = new Rect((Screen.width - crosshairTexture.width) / 2, (Screen.height - crosshairTexture.height) / 2, crosshairTexture.width, crosshairTexture.height);
        //Debug.Log("NAEEE");
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                //Debug.Log("NAEEE");
                crosshairTexture.SetPixel(i, j, Color.white);
            }

        }
        crosshairTexture.Apply();
        #endregion
        #region Inventory initialization
        inventory = new short[5];
        selected = 1;
        for (int i = 0; i > 5; i++) inventory[i] = 5;
        #endregion
        try_to_move = new Vector3(0, 0, 0);
        character = GetComponent<CharacterController>();
        hasPressed = new bool[7];
        StartCoroutine(placeStuff());
        StartCoroutine(UpdateFace());
    }

    /// <summary>
    /// Change the face
    /// </summary>
    IEnumerator UpdateFace()
    {
        Transform a = transform.GetChild(0);
        Transform b = transform.GetChild(1).GetChild(0);
        //Debug.Log(b);
        while (true)
        {
            b.localRotation = a.localRotation;
            yield return new WaitForSeconds(.25f);
        }
    }

    /// <summary>
    /// Change Rotation of camera and apply a fix for double placement of blocks
    /// </summary>
    void Update()
    {
        //Do a small check first? is going more down or up allowed?
        rotate_vector.Set(-Input.GetAxis("Mouse Y"),Input.GetAxis("Mouse X"),0);
        //myCamera.transform.rotation = myCamera.transform.rotation * Quaternion.Euler(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) ;
        myCamera.transform.rotation = Quaternion.Euler(myCamera.transform.rotation.eulerAngles + rotate_vector*movement_sensitivity);

        //if (myCamera.transform.rotation.eulerAngles.x > 85)
        //    myCamera.transform.rotation = Quaternion.Euler(85, myCamera.transform.rotation.eulerAngles.y, myCamera.transform.rotation.eulerAngles.z);


        //myCamera.transform.Rotate(rotate_vector);
        //      force_vector.Set(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // init velocity
        
        float buff = try_to_move.y;
        try_to_move.Set(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        try_to_move = myCamera.transform.rotation * (try_to_move * movement_sensitivity);
        
        // Move the spherical coordinates into the x-z plane
        try_to_move = Vector3.ProjectOnPlane(try_to_move, Vector3.up).normalized * try_to_move.magnitude;
        // v = u-gt
        try_to_move.y = buff -  gravity * Time.deltaTime;
        Debug.Log(try_to_move.y);

        //Do jump stuff and reset y velocity
        if (character.isGrounded) {
            try_to_move.y = 0+Input.GetAxis("Jump")*jump_sensitivity;
            
        }
        character.Move(try_to_move * Time.deltaTime);




        // This pass helps remove double interactions, when the fps is low
        hasPressed[0] = hasPressed[0] || Input.GetMouseButtonDown(0);
        hasPressed[1] = hasPressed[1] || Input.GetMouseButtonDown(1);
        hasPressed[2] = hasPressed[2] || Input.GetKey("1");
        hasPressed[3] = hasPressed[3] || Input.GetKey("2");
        hasPressed[4] = hasPressed[4] || Input.GetKey("3");
        hasPressed[5] = hasPressed[5] || Input.GetKey("4");
        hasPressed[6] = hasPressed[6] || Input.GetKey("5");
        //Debug.Log(inventory);
    }


    /// <summary>
    /// Add and remove contents of the inventory
    /// </summary>
    /// <param name="blocktype">Block enum val</param>
    /// <param name="num_mod">How much of a change</param>
    /// <returns>bool of whether it was possible</returns>
    public bool modifyInventory(blocktypes blocktype, short num_mod)
    {
        inventory[(int)blocktype - 1] += num_mod;
        // Add methods to update block list GUI
        return true;
    }

    /// <summary>
    /// Every now and again this enumerator will trigger and do the placements and stuff. 
    /// </summary>
    /// <returns></returns>
    IEnumerator placeStuff()
    {
        for(; ; )
        {
            if(hasPressed[2]|| hasPressed[3] || hasPressed[4] || hasPressed[5] || hasPressed[6])
            {
                selected = (short) ( hasPressed[2] ? 1 : (hasPressed[3]?2:(hasPressed[4]?3:(hasPressed[5]?4:(hasPressed[6]?5:1)))) );
                
            }
            if (hasPressed[0] || hasPressed[1])
            {
                //Debug.Log("Recorded " + hasPressed[0]);
                //int button = hasPressed[0] ? 0 : 1;
                RaycastHit hit;
                Ray ray = data.player_cam.ScreenPointToRay(new Vector3(data.player_cam.pixelWidth / 2, data.player_cam.pixelHeight / 2, 0));
                if (Physics.Raycast(ray, out hit, interact_disance, data.blocklayermask))
                {
                    Transform hit_object = hit.transform;
                    //Debug.Log(hit.normal);
                    Vector3 place_pos = hit_object.position + hit.normal;
                    if (!hasPressed[0])
                        Block.Blockinit((blocktypes) selected, place_pos);
                    else
                        Block.BlockDestroy(hit.transform.gameObject);
                }
                
            }
            // Reset the request
            for(int i=0;i<hasPressed.Length;i++) hasPressed[i] = false;

            yield return new WaitForSeconds(0.25f);
        }
    }
    void OnGUI()
    {
        GUI.DrawTexture(position, crosshairTexture);
    }
}
