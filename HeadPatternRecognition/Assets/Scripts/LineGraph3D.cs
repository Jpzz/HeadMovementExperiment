using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineGraph3D : MonoBehaviour
{
    private LineRenderer[] lineObjects;
    private int childCount;
    
    private float drawIntervalTime = 0f;
    private float _time = 0;
    private int index = 0;
    private Record _record;
    [Header("Axis Scale Value")]
    public int xScale;
    public int yScale;
    public GameObject circle;
    public Transform circleParent;

    private void Awake()
    {
        _record = FindObjectOfType<Record>();
    }

    void Start()
    {
        drawIntervalTime = _record.saveIntervalTime;
        childCount = transform.childCount;
        lineObjects = new LineRenderer[childCount];
        for (var i = 0; i < childCount; i++)
        {
            lineObjects[i] = transform.GetChild(i).GetComponent<LineRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;
        if (_time > drawIntervalTime)
        {
            _time = 0f;
            //Vector3 rotation = new Vector3(Record.dataSet[index][0], Record.dataSet[index][1], Record.dataSet[index][2]);
            //DrawGraph(lineObjects[0], Color.blue, rotation, Record.dataSet.Count);
            index++;
        }
    }

    private void DrawGraph(LineRenderer lineRenderer, Color color, Vector3 position, int positionCount)
    {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.widthMultiplier = 0.4f;
        lineRenderer.positionCount = positionCount;
        lineRenderer.SetPosition(index, position);
    }
    
}
