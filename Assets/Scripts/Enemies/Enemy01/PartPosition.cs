using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartPosition : MonoBehaviour
{
    public Vector3 lastPosition;
    public Quaternion lastRotation;

    public bool travelFinish;

    private void Update()
    {
        if (transform.localPosition != lastPosition && transform.localRotation != lastRotation)
        {
            travelFinish = false;
        }
        else
        {
            travelFinish = true;
        }
    }

    public void UpdateLastPosition()
    {
        lastPosition = transform.localPosition;
        lastRotation = transform.localRotation;
    }

    public void ResetPosition()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, lastPosition, Time.deltaTime * 4f);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, lastRotation, Time.deltaTime * 4f);
    }
}
