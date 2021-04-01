using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkingSection : MonoBehaviour
{
    public Material mat;
    public Color color;
    public void DrawBoundary(Vector3 first, Vector3 second, Vector3 third, Vector3 fourth, Transform parent,int layer)
    {
        var empty = new GameObject();
        var boundaryOBJ = Instantiate(empty);
        boundaryOBJ.layer = layer;
        boundaryOBJ.transform.SetParent(parent);
        boundaryOBJ.AddComponent<LineRenderer>();
        var lineRenderer = boundaryOBJ.GetComponent<LineRenderer>();
        lineRenderer.sharedMaterial = mat;
        lineRenderer.positionCount = 4;
        lineRenderer.widthMultiplier = 0.1f;
        lineRenderer.loop = true;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        Vector3[] position = {first, second, third, fourth};
        lineRenderer.SetPositions(position);
    }
}
