using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Data : MonoBehaviour
{
       [Header("File Name")] public string fileName;
       [Header("Animation Genre")] public string Genre;
       [Header("Record")] private Record record;
       private string fileDirectory;
       private string Date;
    
       void Start()
       {
           fileDirectory = "Assets/HeadMovementData/" + fileName +".txt";
       }
        
       private void DataToText(string filePath)
       {
           var DataSize = record.dataSet.Count;
           var NumberOfDataType = record.dataSet[0].Length;
           var writer = new StreamWriter(filePath,false);
           writer.WriteLine(Genre);
           
           for (var i = 0; i < DataSize; i++)
           {
               var dataTime = record.dataSet[i][8];
               writer.Write("Time :" + dataTime + " ## ");
               for (var j = 0; j < NumberOfDataType-1; j++)
               {
                   var data = record.dataSet[i][j].ToString("N2");
                   var tag = record.tags[j];
                   
                   writer.Write(tag +" :"+ data + " ## ");
               }
               writer.WriteLine(); 
           }
           writer.Flush();
           writer.Close();
           
           AssetDatabase.ImportAsset(filePath);
       }
        
       public void Save()
       {
           DataToText(fileDirectory);
       }
}
