using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineLegseffect : MonoBehaviour
{
    public Transform[] points;

    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = points.Length;
    }

    private void Update()
    {
        for (int i = 0; i < points.Length; i++)
        {
            lineRenderer.SetPosition(i, points[i].position);
        }
    }
}
