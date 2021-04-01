using System.Collections;
using System.Collections.Generic;
using extOSC;
using UnityEngine;

public class Transmitter : MonoBehaviour
{
    public OscOut oscOut;
    [Header("XEuler")] public string XEuler;
    [Header("YEuler")] public string YEuler;
    [Header("ZEuler")] public string ZEuler;
    [Header("XPosition")] public string XPosition;
    [Header("YPosition")] public string YPosition;
    [Header("ZPosition")] public string ZPosition;
    [Header("Velocity")] public string Velocity;
    [Header("Acceleration")] public string Acceleration;
    [Header("Time")] public string Time;
    
    [Header("X Euler Mark")] public string MarkXEuler;
    [Header("Y Euler Mark")] public string MarkYEuler;
    [Header("Z Euler Mark")] public string MarkZEuler;
    [Header("X Position Mark")] public string MarkXPosition;
    [Header("Y Position Mark")] public string MarkYPosition;
    [Header("Z Position Mark")] public string MarkZPosition;


    private string[] dataAddresses;

    private string[] dataMarkAddress;

    [Header("Mark To Visualize Editor")] 
    public string Mark;
  
    [Header("Export To Visualize Editor")]
    public string Export;
    [Header("Reset Signal To Visualize Editor")]
    public string ResetContents;
    void Awake()
    {
        dataAddresses = new string[]
        {
            XEuler, YEuler, ZEuler, XPosition, YPosition, ZPosition, Velocity, Acceleration,
            Time
        };
    }

    public void Send(float[] value)
    {
        for (var i = 0; i < value.Length; i++)
        {
            Send(dataAddresses[i],value[i]);
        }
    }
    private void Send(string address, float value)
    {
        oscOut.Send(address,value);
    }
    
  
    public void Send(string address, bool value)
    {
        oscOut.Send(address, value);
    }
}
