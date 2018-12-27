using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    Texture2D crosshairTexture;
    Rect position;
    public short[] inventory;
    public short selected;
    bool[] hasPressed;
    public const float interact_disance = 10f; 

    // Use this for initialization
    void Start()
    {
        #region Crosshair
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
        hasPressed = new bool[7];
        StartCoroutine(placeStuff());
        StartCoroutine(UpdateFace());
    }

    // Change direction of your face
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

    // Update is called once per frame
    void Update()
    {
        hasPressed[0] = hasPressed[0] || Input.GetMouseButtonDown(0);
        hasPressed[1] = hasPressed[1] || Input.GetMouseButtonDown(1);
        hasPressed[2] = hasPressed[2] || Input.GetKey("1");
        hasPressed[3] = hasPressed[3] || Input.GetKey("2");
        hasPressed[4] = hasPressed[4] || Input.GetKey("3");
        hasPressed[5] = hasPressed[5] || Input.GetKey("4");
        hasPressed[6] = hasPressed[6] || Input.GetKey("5");
        //Debug.Log(inventory);
    }
    public int modifyInventory(blocktypes blocktype, short num_mod)
    {
        inventory[(int)blocktype - 1] += num_mod;
        // Add methods to update block list GUI
        return 1;
    }
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
