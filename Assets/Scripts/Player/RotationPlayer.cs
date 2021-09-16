using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationPlayer : MonoBehaviour
{
    public Camera cameraPlayer;

    public LayerMask layerMask;

    private Ray ray;
    private RaycastHit hit;

    public GameObject sphereTargetView;

    private void Update()
    {
        ray = cameraPlayer.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, layerMask))
        {
            sphereTargetView.transform.position = hit.point;
            transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
        }
    }
}
