using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusCamera : MonoBehaviour
{
    public GameObject player;
    public Camera cameraPlayer;
    public float heightFocusPoint;

    private void Update()
    {
        //Focus point
        transform.position = Vector3.Lerp(transform.position, player.transform.position, 8f);
        transform.position = new Vector3(transform.position.x, transform.position.y + heightFocusPoint, transform.position.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, player.transform.rotation, 0.05f);

        //Camera
        cameraPlayer.transform.LookAt(transform.position);
    }
}
