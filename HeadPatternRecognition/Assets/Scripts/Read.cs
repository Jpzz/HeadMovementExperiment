using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Read : MonoBehaviour
{
    public string csvFileName;

    public List<float[]> headData = new List<float[]>();

    public bool isRead;
    void Start()
    {
        ReadCSV();
    }
    private void ReadCSV()
    {
        var filePath = DataUtility.GetPath(csvFileName, ".csv");
        StreamReader streamReader = new StreamReader(filePath);
        var dataStr = streamReader.ReadToEnd();
        streamReader.Close();
        var splitStr = dataStr.Split(',','\n');
        
        for (var i = 0; i < 34344; i+=8)
        {
            if(i  == 0)
                continue;
            var tempData = new float[8];
            
            float.TryParse(splitStr[i], out tempData[0]);   
            float.TryParse(splitStr[i+1], out tempData[1]);
            float.TryParse(splitStr[i+2], out tempData[2]);
            float.TryParse(splitStr[i+3], out tempData[3]);
            float.TryParse(splitStr[i+4], out tempData[4]);
            float.TryParse(splitStr[i+5], out tempData[5]);
            float.TryParse(splitStr[i+6], out tempData[6]);
            float.TryParse(splitStr[i+7], out tempData[7]);

            for (var j = 0; j < tempData.Length; j++)
            {
                if (!(tempData[j] < 0)) continue;
                
                var value = 360 - Mathf.Abs(tempData[j]);
                tempData[j] = value;
            }
            
            headData.Add(tempData);
        }
        
        //Debug.Log(headData.Count);
        isRead = true;

    }
    
}
