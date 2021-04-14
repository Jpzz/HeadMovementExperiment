using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataUtility
{
    public static string GetPath(string fileName, string fileExtenstion)
    {
        var filePath = Application.streamingAssetsPath + "/HeadMovementData" + "/"+ fileName + fileExtenstion;
        return filePath;
    }
}
