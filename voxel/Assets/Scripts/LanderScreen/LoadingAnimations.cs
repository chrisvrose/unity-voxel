using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingAnimations : MonoBehaviour
{
    [Range(.1f,100f)]
    public float dayLengthMinutes;

    float dayLengthMinutesByRotation;
    // Start is called before the first frame update
    void Start()
    {
        dayLengthMinutesByRotation = dayLengthMinutes*60 / 360;
    }

    void Update()
    {
        bool night = transform.eulerAngles.x > 180;
        GetComponent<Light>().intensity = !night ? 1 : 0;
        RenderSettings.ambientIntensity = !night ? 0.5f : 0;
        transform.Rotate(Vector3.left * dayLengthMinutesByRotation*Time.deltaTime);
    }

   

}
