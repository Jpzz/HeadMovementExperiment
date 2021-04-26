using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Camera[] heatCameras;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.DownArrow))
        {
            for (var i = 0; i < heatCameras.Length; i++)
            {
                heatCameras[i].transform.position += new Vector3(0f, 0f, -3f) * Time.deltaTime;
            }   
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            for (var i = 0; i < heatCameras.Length; i++)
            {
                heatCameras[i].transform.position += new Vector3(0f, 0f, 3f) * Time.deltaTime;
            }
        }
    }
    
   
}
