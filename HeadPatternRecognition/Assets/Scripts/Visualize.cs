using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Visualize : MonoBehaviour
{
    #region Public Variables
    public Read read;
    public GameObject[] visualPrefabs;
    public GameObject markPrefabs;
    public GameObject circleMarkPrefabs;
    public GameObject heatPrefabs;
    // public GameObject linePrefabs;
    // public GameObject mapPlanePrefabs;
    // public float mapWidth;
    // public GameObject raycastSphere;
    #endregion
    #region Private Variables
    private Transform transform;
    private GameObject visualP;
    private GameObject heatmapP;
    //원 각도
    private float degree;
    //비주얼라이즈 원 크기
    private float radius = 30;
    //머리 움직임에 따른 가중치
    private float yWeight = 0.05f;
    private int prefabsIndex = 0;
    private LineRenderer lineRenderer;
    private float weightStopTime = 0f;
    
    
    //히트맵 합치기, 펼치기 변수
    private int curLineVertexCount = 0;
    private Vector3[] heatPosition;
    private GameObject[] mapPlane;


    private Dictionary<float, int> degreeDict = new Dictionary<float, int>();
    #endregion
    
    private void Start()
    {
        transform = GetComponent<Transform>();
        visualP = GameObject.FindWithTag("Visualize");
        heatmapP = GameObject.FindWithTag("Heatmap");
        heatmapP.gameObject.SetActive(false);
        //lineRenderer = linePrefabs.GetComponent<LineRenderer>();
        heatPosition = new Vector3[read.headData.Count];
        mapPlane = new GameObject[read.headData.Count];
        Sort();
        StartCoroutine(CoVisualize());
    }

    private IEnumerator CoVisualize()
    {
        while (!read.isRead)
        {
            yield return null;
        }
        
        for (var i = 0; i < read.headData.Count; i++)
        {
            if (i > 0)
            {
                ///Quaternion 계산
                //첫번째 Quaternion
                var qot = Quaternion.Euler(new Vector3(read.headData[i - 1][0], read.headData[i - 1][1],
                    read.headData[i - 1][2]));
                //두번쨰 Quaternion
                var qot2 = Quaternion.Euler(new Vector3(read.headData[i][0], read.headData[i][1], read.headData[i][2]));
                ///속도 계산
                var velocity = CalAllVelocity(qot, qot2, 0.1f);
                
                ///속도에 따른 가중치 계산
                if (velocity < 8)
                    yWeight += 0.005f;
                else
                    yWeight = 0.1f;
                
                /// Degree 계산
                if (degree >= 360)
                    degree = 0;
                
                ///Degree에 따른 Radian 계산
                var radian = degree * Mathf.PI / 180;
                ///생성 오브젝트 Look at을 위한 다음 위치 계산
                var nextRadian = (degree + 1.5f) * Mathf.PI / 180;
                
                if (read.headData[i][7] == 0)
                {
                    var go = Instantiate(visualPrefabs[prefabsIndex],visualP.transform);
                    
                    go.transform.position = new Vector3(Mathf.Sin(radian) * radius, 0f, Mathf.Cos(radian) * radius);
                    ///다음 위치에 따른 Loot at
                    go.transform.LookAt(new Vector3(Mathf.Sin(nextRadian) * (radius - 0.005f), 0f,
                        Mathf.Cos(nextRadian) * (radius - 0.005f)));
                    
                    if(velocity / 100 > 0.04f)
                        go.transform.localScale = new Vector3(velocity/100, yWeight, 0.4f);
                    else
                    {
                        go.transform.localScale = new Vector3(0.04f, yWeight, 0.4f);
                        weightStopTime += 0.1f;
                    }
                }
                else
                {
                    var go = Instantiate(markPrefabs, visualP.transform);
                    go.transform.position = new Vector3(Mathf.Sin(radian) * radius, 0f, Mathf.Cos(radian) * radius);
                    go.transform.localScale = new Vector3(1f, 1f, 1f);

                    if (read.headData[i][7] == 1)
                    {
                        go.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.red;
                        go.name = "marking visualize";
                    }
                    else if (read.headData[i][7] == 2)
                    {
                        go.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.green;
                        go.name = "transition visualize";
                        prefabsIndex++;
                    }
                }
            }
            /// 어몽어스 캐릭터 시뮬레이션
            var rotation = new Vector3(read.headData[i][0] - read.headData[0][0], read.headData[i][1]-read.headData[0][1], read.headData[i][2]-read.headData[0][2]);

            var yRotation = new Vector3(0f, read.headData[i][1] - read.headData[0][1], 0f);
            transform.rotation = Quaternion.Euler(yRotation);
            
            /// 히트맵 생성
            
            // var heatMap = Instantiate(heatPrefabs, heatmapP.transform);
            //
            // heatMap.transform.position = transform.forward * (i *0.01f);
            // heatMap.transform.position = new Vector3(heatMap.transform.position.x, 0f, heatMap.transform.position.z);
            // heatMap.transform.LookAt(transform);
            
            /// 스피어 히트맵 평면으로 맵핑 
            //RaycastHeatMap(transform,30f,i);
            yield return null;
            ///변수 갱신
            degree+=0.08f;
            //radius -= 0.006f;
        }

        var totalTime = read.headData.Count * 0.1f;
        var stopProportion = weightStopTime / totalTime;
        // raycastSphere.SetActive(false);
        Debug.Log(stopProportion.ToString("N2"));
    }

    private void Sort()
    {
        foreach (var t in read.headData)
        {
            var temp = Mathf.Round(t[1] - read.headData[0][1]);
            if(!degreeDict.ContainsKey(temp))
                degreeDict.Add(temp,1);
            else
            {
                degreeDict[temp] += 1;
            }
        }

        foreach (var item in degreeDict)
        {
            var yRotation = new Vector3(0f, item.Key, 0f);
            transform.rotation = Quaternion.Euler(yRotation);
            var heatMap = Instantiate(heatPrefabs, heatmapP.transform);
            heatMap.transform.position = transform.forward * 20f;
            heatMap.transform.localScale = new Vector3(0.1f, 0.1f, item.Value * 0.1f);
            heatMap.transform.LookAt(transform);
        }
    }
    
    /// 콜리더 내부에서 레이 캐스트를 쏘기 때문에 역으로 계산해줘야함
    // private void RaycastHeatMap(Transform tr, float distance, int index)
    // {
    //     var outPosition = tr.forward * distance;
    //     var direction = (tr.position- outPosition).normalized;
    //     var ray = new Ray(outPosition, direction);
    //     var uv = new Vector2();
    //
    //     if (!Physics.Raycast(ray, out var hit, Mathf.Infinity)) 
    //         return;
    //     uv = hit.textureCoord;
    //     var xPos = uv.x * mapWidth - mapWidth/2;
    //     var yPos = uv.y * mapWidth / 2;
    //     var zPos = 50f + index * 0.5f;
    //     var position = new Vector3(xPos, yPos, zPos);
    //     SetLineRenderer(index,position);
    //     
    //     if (read.headData[index][7] == 0)
    //         return;
    //     
    //     Debug.Log("Marking");
    //     
    //     var go = Instantiate(markPrefabs, position, Quaternion.identity, visualP.transform);
    //     var go02 = Instantiate(circleMarkPrefabs, new Vector3(position.x, position.y, 49f), Quaternion.identity, visualP.transform);
    //     go.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
    //     go02.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
    //     go.transform.name = "Line Marker";
    //     if (read.headData[index][7] == 1)
    //     {
    //         go.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.red;
    //         go02.transform.GetComponent<Renderer>().material.color = Color.red;
    //     }
    //     else
    //     {
    //         go.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.green;
    //         go02.transform.GetComponent<Renderer>().material.color = Color.green;
    //     }
    //     Debug.DrawRay(outPosition, direction * 6, Color.red);
    // }

    // public void MergeHeatMap()
    // {
    //     for (var i = 0; i < mapPlane.Length; i++)
    //     {
    //         mapPlane[i].SetActive(false);
    //     }
    //     mapPlane[0].SetActive(true);
    //
    //     mapPlane[0].transform.GetComponent<Renderer>().sharedMaterial.color = new Color(1f, 1f, 1f, 1f);
    //
    //     for (var i = 0; i < curLineVertexCount; i++)
    //     {
    //         var position = lineRenderer.GetPosition(i);
    //         var position02 = new Vector3(position.x, position.y, 50f);
    //         lineRenderer.SetPosition(i, position02);
    //     }
    //
    //     lineRenderer.widthMultiplier = 0.1f;
    // }
    //
    // public void UnMergeHeatMap()
    // {
    //     for (var i = 0; i < mapPlane.Length; i++)
    //     {
    //         mapPlane[i].SetActive(true);
    //     }
    //     mapPlane[0].transform.GetComponent<Renderer>().sharedMaterial.color = new Color(1f, 1f, 1f, 0.03f);
    //     
    //     for (var i = 0; i < curLineVertexCount; i++)
    //     {
    //         var position = heatPosition[i];
    //         lineRenderer.SetPosition(i, position);
    //     }
    //     
    //     lineRenderer.widthMultiplier = 0.3f;
    // }
    //
    // private void SetLineRenderer(int index, Vector3 position)
    // {
    //     lineRenderer.positionCount = index + 1;
    //     curLineVertexCount++;
    //     lineRenderer.SetPosition(index, position);
    //     var quad = Instantiate(this.mapPlanePrefabs, new Vector3(mapWidth/2 - mapWidth/2, mapWidth/4, 50f+index*0.5f), Quaternion.identity);
    //     quad.transform.SetParent(visualP.transform);
    //     quad.name = "box" + index;
    //     mapPlane[index] = quad;
    //     heatPosition[index] = position;
    // }
    public static float CalAllVelocity(Quaternion qot, Quaternion qot2, float time)
    {
        var angle = Quaternion.Angle(qot, qot2);
        var velocity = angle / time;
        return velocity;
    }
    
}
