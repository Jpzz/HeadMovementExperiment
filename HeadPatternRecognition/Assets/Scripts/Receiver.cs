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
    
    private void OnEnable()
    {        
        oscIn.MapFloat(exportAddress, OnReceiveExport);
        oscIn.MapFloat(experimentAddress, OnReceiveExperimentStart);
        oscIn.MapFloat(recenterAddress, OnReceiveRecenterCamera);
        oscIn.MapFloat(markAddress, OnReceiveMark);
        oscIn.MapFloat(resetAddress, OnReceiveReset);
        oscIn.Open(7000);
    }

    public void OnReceiveExperimentStart(float value)
    {
        Debug.Log(value);
        if(value == 1)
            record.IsStartExperiment = true;
    }
    
    public void OnReceiveRecenterCamera(float value)
    {
        Debug.Log(value);
        if(value == 1)
            record.IsCenter = true;
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

    public void OnReceiveExport(float value)
    {
        Debug.Log(value);
        if (value == 1)
        {
            record.IsExport = true;
            write.ExportCSV();
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
