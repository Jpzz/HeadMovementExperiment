using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Write : MonoBehaviour
{
   [Header("Read Text file name")] public string textFileName;
   public bool IsConvertCSV;
   
   
   public string personName;
   public string animationName;
   public Record record;

   public Text personNameText;
   public Text animationText;
   public Text ipText;
   public Text convertFileText;
   public Text isConvertText;
   public Text isExportText;
   public Text ExportText;
   
   private string date;
   private string path;
   private string settingPath;

   private bool isJsonLoad;
   private List<string[]> dataList = new List<string[]>();
   private void Awake()
   {
      date = System.DateTime.Now.ToString("yyyy-MM-dd");
   }

   private void Start()
   {
      LoadJson();
      
      if(IsConvertCSV && isJsonLoad)
         ConvertCSV();
   }
   
   private void LoadJson()
   {
      string hostname = Dns.GetHostName();
      IPAddress[] ipaddress = Dns.GetHostEntry(hostname).AddressList;
      foreach (IPAddress ip in ipaddress) {
         if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
            ipText.text = ip.ToString();
         }
      }
      
      settingPath = Application.streamingAssetsPath + "/Data/Setting.json";
      var data = File.ReadAllText(settingPath);
      Setting setting = JsonUtility.FromJson<Setting>(data);
      textFileName = setting.ConvertFile;
      convertFileText.text = "ConvertFile : " +textFileName;
      IsConvertCSV = setting.IsConvert;
      isConvertText.text = "IsConvert : "+IsConvertCSV.ToString();
      personName = setting.TestPersonName;
      personNameText.text = "Person : " +personName;
      animationName = setting.TestAnimation;
      animationText.text = "Animation : " +animationName;
      isJsonLoad = true;
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
      ExportText.text = fileName;
      isExportText.text = "TRUE";
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
         var array = new string[9];
         array[0] = splitStr[i];
         array[1] = splitStr[i + 1];
         array[2] = splitStr[i + 2];
         array[3] = splitStr[i + 3];
         array[4] = splitStr[i + 4];
         array[5] = splitStr[i + 5];
         array[6] = splitStr[i + 6];
         array[7] = splitStr[i + 7];

         if (i == 0)
         {
            array[8] = "Velocity";
         }
         else
         {
            float x, y, z;
            float x1, y1, z1;
            
            float.TryParse(splitStr[i], out x);
            float.TryParse(splitStr[i + 1], out y);
            float.TryParse(splitStr[i + 2], out z);
            float.TryParse(splitStr[i - 8], out x1);
            float.TryParse(splitStr[i + 1 - 8], out y1);
            float.TryParse(splitStr[i + 2 - 8], out z1);

            float newX = new float();
            float newY = new float();
            float newZ = new float();
            float newX1 = new float();
            float newY1 = new float();
            float newZ1 = new float();
            
            if (x > 360f)
               newX = x - 360f;
            else if (x < 0)
            {
               newX = 360 + (x%360);
            }
            
            if (y > 360f)
               newY = y - 360f;
            else if (y < 0)
               newY = 360 + (y%360);
            
            if (z > 360f)
               newZ = z - 360f;
            else if (z < 0)
               newZ = 360 + (z%360);
            
            if (x1 > 360f)
               newX1 = x1 - 360f;
            else if (x1 < 0)
               newX1 = 360 + (x1%360);
            
            if (y1 > 360f)
               newY1 = y1 - 360f;
            else if (y1 < 0)
               newY1 = 360 + (y1%360);
            
            if (z1 > 360f)
               newZ1 = z1 - 360f;
            else if (z1 < 0)
               newZ1 = 360 + (z1%360);
            
            var q = Quaternion.Euler(new Vector3(newX, newY, newZ));
            var q1 = Quaternion.Euler(new Vector3(newX1, newY1, newZ1));
            var velocity = Visualize.CalAllVelocity(q, q1, 0.1f);
            array[8] = velocity.ToString();
         }
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
