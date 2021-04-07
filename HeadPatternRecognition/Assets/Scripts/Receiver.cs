using System;
using System.Collections;
using System.Collections.Generic;
using extOSC;
using UnityEngine;

public class Receiver : MonoBehaviour
{
    public OscIn oscIn;
    public Record record;
    public Data data;
    public Write write;
    public string exportAddress;
    public string experimentAddress;
    public string resetAddress;
    public string markAddress;
    public string recenterAddress;
    public string sceneAddress;
    private void OnEnable()
    {        
        oscIn.MapFloat(exportAddress, OnReceiveExport);
        oscIn.MapFloat(experimentAddress, OnReceiveExperimentStart);
        oscIn.MapFloat(recenterAddress, OnReceiveRecenterCamera);
        oscIn.MapFloat(markAddress, OnReceiveMark);
        oscIn.MapFloat(resetAddress, OnReceiveReset);
        oscIn.MapFloat(sceneAddress, OnReceiveSceneTransition);
        oscIn.Open(7000);
    }

    public void OnReceiveExperimentStart(float value)
    {
        Debug.Log(value);
        if(value == 1)
        {
            record.IsStartExperiment = true;
            record.CameraRecenter();
        }
    }
    
    public void OnReceiveRecenterCamera(float value)
    {
        Debug.Log(value);
        if (value == 1)
            record.CameraRecenter();
    }

    public void OnReceiveReset(float value)
    {
        Debug.Log(value);
        if(value == 1)
            record.IsReset = true;
        //transmitter.Send(transmitter.ResetContents,value);
    }

    public void OnReceiveMark(float value)
    {
        Debug.Log(value);
        if(value == 1)
            record.IsMark = true;
        //transmitter.Send(transmitter.Mark, value);
    }

    public void OnReceiveSceneTransition(float value)
    {
        Debug.Log(value);
        if (value == 1)
            record.IsTransition = true;
    }

    public void OnReceiveExport(float value)
    {
        Debug.Log(value);
        if (value == 1)
        {
            record.IsExport = true;
            write.ExportTxt();
        }
    }
    
    public void OnReceiveSave(bool value)
    {
        if (value)
        {
            data.Save();
        }
    }
}
