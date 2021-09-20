using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RegenArcRenderer : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public float velocity;
    public float angle;
    public int resolution;

    private float g;
    private float radianAngle;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        g = Mathf.Abs(Physics2D.gravity.y);

        RenderArc();
    }

    private void RenderArc()
    {
        lineRenderer.positionCount = resolution + 1;
        lineRenderer.SetPositions(CalculateArcArray());
    }

    private Vector3[] CalculateArcArray()
    {
        Vector3[] arcArray = new Vector3[resolution + 1];

        radianAngle = Mathf.Deg2Rad * angle;
        float maxDistance = (velocity * velocity * Mathf.Sin(2 * radianAngle)) / g;

        for (int i = 0; i <= resolution; i++)
        {
            float t = (float)i / (float)resolution;
            arcArray[i] = CalculateArcPoint(t, maxDistance);
        }

        return arcArray;
    }

    private Vector3 CalculateArcPoint(float t, float maxDistance)
    {
        float x = t * maxDistance;
        float y = x * Mathf.Tan(radianAngle) - ((g * x * x) / (2 * velocity * velocity * Mathf.Cos(radianAngle) * Mathf.Cos(radianAngle)));
        return new Vector3(x, y);
    }
}
