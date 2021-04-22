using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualize : MonoBehaviour
{
    public Read read;
    public GameObject[] visualPrefabs;
    public GameObject markPrefabs;
    public GameObject heatPrefabs;
    public GameObject linePrefabs;
    
    #region Private Variables

    private Transform transform;
    private GameObject visualP;
    private GameObject heatmapP;
    private bool loop;
    //원 각도
    private float degree;
    //비주얼라이즈 원 크기
    private float radius = 30;
    //움직임에 따른 가중치
    private float yWeight = 0.05f;

    private int prefabsIndex = 0;

    private LineRenderer lineRenderer;
    #endregion
    
    private void Start()
    {
        transform = GetComponent<Transform>();
        visualP = GameObject.FindWithTag("Visualize");
        heatmapP = GameObject.FindWithTag("Heatmap");
        lineRenderer = linePrefabs.GetComponent<LineRenderer>();
        
    }

    private void Update()
    {
        if (read.isRead && !loop)
        {
            loop = true;
            StartCoroutine(CoVisualize());
        }
    }

    
    private IEnumerator CoVisualize()
    {
        lineRenderer.positionCount = read.headData.Count - 1;
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

                GameObject go = null;
                var mark = false;
                
                if (read.headData[i][7] == 0)
                {
                    go = Instantiate(visualPrefabs[prefabsIndex],visualP.transform);
                }
                else if (read.headData[i][7] == 1)
                {
                    go = Instantiate(markPrefabs, visualP.transform);
                    go.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.red;
                    go.name = "marking visualize";
                    mark = true;
                }
                else if (read.headData[i][7] == 2)
                {
                    go = Instantiate(markPrefabs, visualP.transform);
                    go.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.green;
                    go.name = "transition visualize";
                    prefabsIndex++;
                    mark = true;
                }
                
                go.transform.position = new Vector3(Mathf.Sin(radian) * radius, 0f, Mathf.Cos(radian) * radius);
                ///다음 위치에 따른 Loot at
                go.transform.LookAt(new Vector3(Mathf.Sin(nextRadian) * (radius-0.005f), 0f, Mathf.Cos(nextRadian) * (radius-0.005f)));
                ///속도 비율 줄이기
                var convertVel = velocity / 100;

                if (mark)
                {
                    go.transform.localScale = new Vector3(1f, 1f, 1f);
                }
                else
                {
                    if(convertVel > 0.04f)
                        go.transform.localScale = new Vector3(velocity/100, yWeight, 0.4f);
                    else
                        go.transform.localScale = new Vector3(0.04f, yWeight, 0.4f);  
                }
            }
            /// 어몽어스 캐릭터 시뮬레이션
            var rotation = new Vector3(read.headData[i][0] - read.headData[0][0], read.headData[i][1]-read.headData[0][1], read.headData[i][2]-read.headData[0][2]);
            transform.rotation = Quaternion.Euler(rotation);
            /// 히트맵 생성
            var heatMap = Instantiate(heatPrefabs, heatmapP.transform);
            heatMap.transform.position = transform.forward * 10f;
            heatMap.transform.LookAt(transform);
            
            RaycastHeatMap(transform);
            yield return new WaitForSeconds(0.1f);
            ///변수 갱신
            degree+=1.5f;
            radius -= 0.006f;
        }
    }


    private void RaycastHeatMap(Transform tr)
    {
        RaycastHit hit;

        if (Physics.Raycast(tr.position,tr.forward, out hit,Mathf.Infinity))
        {
            Debug.Log(hit.transform.name);
            Debug.DrawRay(tr.position, tr.forward, Color.red);
        }
        else
        {
            Debug.DrawRay(tr.position, tr.forward, Color.blue);
        }
    }
    private float CalAllVelocity(Quaternion qot, Quaternion qot2, float time)
    {
        var angle = Quaternion.Angle(qot, qot2);
        var velocity = angle / time;

        return velocity;
    }
}
