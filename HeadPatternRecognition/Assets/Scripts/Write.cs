using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class Write : MonoBehaviour
{
   public string personName;
   public string animationName;
   public Record record;
   
   private string date;
   private string path;

   private List<string[]> dataList = new List<string[]>();
   private void Awake()
   {
      date = System.DateTime.Now.ToString("yyyy-MM-dd");
   }

   public void ExportTxt()
   {
      var filePath = GetPath(".txt");
      FileStream fileStream = new FileStream(filePath, FileMode.Create);
      StreamWriter streamWriter = new StreamWriter(fileStream);
      streamWriter.WriteLine(record.dataStr);
      
      streamWriter.Close();
      fileStream.Close();
   }
   public void ExportCSV()
   {
      Debug.Log("EXPORT");
      var rowDataTemp = new string[8];
      var dataSizes = record.dataSet.Count;
      Debug.Log("DataSizes : "+dataSizes);
      for (var i = 0; i < rowDataTemp.Length; i++)
      {
         rowDataTemp[i] = record.tags[i];
      }
      
      dataList.Add(rowDataTemp);
      
      for (var i = 0; i < dataSizes; i++)
      {
         var tempData = new string[8];
         for (int j = 0; j < 8; j++)
         {
            tempData[j] = record.dataSet[i][j].ToString();
            Debug.Log(record.dataSet[i][6]);
         }
         dataList.Add(tempData);
      }
      
      var output = new string[dataSizes + 1][];

      for (var i = 0; i < output.Length; i++)
      {
         output[i] = dataList[i];
      }

      var length = output.GetLength(0);
      var delimiter = ",";
      var stringBuilder = new StringBuilder();

      for (var i = 0; i < length; i++)
      {
         stringBuilder.AppendLine(string.Join(delimiter, output[i]));
      }

      var filePath = GetPath(".csv");
      var outStream = System.IO.File.CreateText(filePath);
      outStream.WriteLine(stringBuilder);
      outStream.Close();
   }
   private string GetPath(string fileExtenstion)
   {
      return Application.streamingAssetsPath + "/HeadMovementData" + "/" + personName + "_" + animationName + "_" +
             date + fileExtenstion;
   }
}
