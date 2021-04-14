using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class Write : MonoBehaviour
{
   [Header("Read Text file name")] public string textFileName;
   public bool IsConvertCSV;
   
   
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

   private void Start()
   {
      if(IsConvertCSV)
         ConvertCSV();
   }

   public void ExportTxt()
   {
      var fileName = personName + "_" + animationName + "_" + date;
      var filePath = DataUtility.GetPath(fileName,".txt");
      FileStream fileStream = new FileStream(filePath, FileMode.Create);
      StreamWriter streamWriter = new StreamWriter(fileStream);
      streamWriter.WriteLine(record.dataStr);
      
      streamWriter.Close();
      fileStream.Close();
   }
   
   private void ConvertCSV()
   {
      Debug.Log("Convert Start");
      var filePath = DataUtility.GetPath(textFileName,".txt");
      StreamReader streamReader = new StreamReader(filePath);
      var dataStr = streamReader.ReadToEnd();
      streamReader.Close();

      var splitStr = dataStr.Split(',');
      Debug.Log(splitStr.Length);
      var dataList = new List<string[]>();
      for (var i = 0; i < splitStr.Length; i+=8)
      {
         var array = new string[8];
         array[0] = splitStr[i];
         array[1] = splitStr[i + 1];
         array[2] = splitStr[i + 2];
         array[3] = splitStr[i + 3];
         array[4] = splitStr[i + 4];
         array[5] = splitStr[i + 5];
         array[6] = splitStr[i + 6];
         array[7] = splitStr[i + 7];
            
         dataList.Add(array);
      }
        
      var output = new string[dataList.Count][];

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

      var filePath02 = DataUtility.GetPath(textFileName,".csv");
      var outStream = System.IO.File.CreateText(filePath02);
      outStream.WriteLine(stringBuilder);
      outStream.Close();
        
      Debug.Log("Convert End");
   }
}
