using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 move = Vector3.zero;
    public float speed;
    private float speedX;
    private float speedZ;
    public float dashSpeed;
    private float dashSpeedSmooth;
    public float gravity;

    public Animator anim;
    private float xSmooth;
    private float zSmooth;

    private RotationPlayer rotationPlayer;

    private Vector3 targetViewNormalized;

    public GameObject dashParticle;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        rotationPlayer = GetComponentInChildren<RotationPlayer>();
    }

    private void Update()
    {
        Dash();
        MovementsSet();
        AnimSet();
    }

    private void Dash()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            dashSpeedSmooth = Mathf.SmoothStep(1, dashSpeed, 1);
            GameObject particleDashGO = Instantiate(dashParticle);
            particleDashGO.transform.parent = transform;
            particleDashGO.transform.position = transform.position;
        }
        else
        {
            dashSpeedSmooth = Mathf.SmoothStep(dashSpeedSmooth, 1, 1);
        }
    }

    private void MovementsSet()
    {
        //Setup speed
        //Vertical
        if (Input.GetKey(KeyCode.Z))
        {
            speedX = Mathf.Lerp(0, speed, 1);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            speedX = Mathf.Lerp(0, -speed, 1);
        }
        else
        {
            speedX = Mathf.Lerp(speedX, 0, 1);
        }
        //Horizontal
        if (Input.GetKey(KeyCode.Q))
        {
            speedZ = Mathf.Lerp(0, speed, 1);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            speedZ = Mathf.Lerp(0, -speed, 1);
        }
        else
        {
            speedZ = Mathf.Lerp(speedZ, 0, 1);
        }

        move = new Vector3(speedX * dashSpeedSmooth, move.y, speedZ * dashSpeedSmooth);

        //Gravity
        move.y = move.y - (gravity * Time.deltaTime);

        //Application direction move
        controller.Move(move * Time.deltaTime);
    }

    private void AnimSet()
    {
        targetViewNormalized = rotationPlayer.sphereTargetView.transform.position - transform.position;
        targetViewNormalized = targetViewNormalized.normalized;

        #region Vertical preset anim
        if (targetViewNormalized.x > 0.7f && targetViewNormalized.z < 0.7f || targetViewNormalized.x < -0.7f && targetViewNormalized.z > -0.7f)
        {
            if (targetViewNormalized.x > 0) //Up velocity
            {
                //Vertical anim
                if (Input.GetKey(KeyCode.Z))
                {
                    xSmooth = Mathf.Lerp(xSmooth, 1, 1);
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    xSmooth = Mathf.Lerp(xSmooth, -1, 1);
                }
                else
                {
                    xSmooth = Mathf.Lerp(xSmooth, 0, 1);
                }

                //Horizontal anim
                if (Input.GetKey(KeyCode.Q))
                {
                    zSmooth = Mathf.Lerp(zSmooth, -1, 1);
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    zSmooth = Mathf.Lerp(zSmooth, 1, 1);
                }
                else
                {
                    zSmooth = Mathf.Lerp(zSmooth, 0, 1);
                }
            }
            else if (targetViewNormalized.x < 0) //Down velocity
            {
                //Vertical anim
                if (Input.GetKey(KeyCode.Z))
                {
                    xSmooth = Mathf.Lerp(xSmooth, -1, 1);
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    xSmooth = Mathf.Lerp(xSmooth, 1, 1);
                }
                else
                {
                    xSmooth = Mathf.Lerp(xSmooth, 0, 1);
                }

                //Horizontal anim
                if (Input.GetKey(KeyCode.Q))
                {
                    zSmooth = Mathf.Lerp(zSmooth, 1, 1);
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    zSmooth = Mathf.Lerp(zSmooth, -1, 1);
                }
                else
                {
                    zSmooth = Mathf.Lerp(zSmooth, 0, 1);
                }
            }
        }
        #endregion

        #region Horizontal preset anim
        else if (targetViewNormalized.x < 0.7f && targetViewNormalized.x > -0.7f)
        {
            if (targetViewNormalized.z < 0) //Left velocity
            {
                //Vertical anim
                if (Input.GetKey(KeyCode.Z))
                {
                    zSmooth = Mathf.Lerp(zSmooth, -1, 1);
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    zSmooth = Mathf.Lerp(zSmooth, 1, 1);
                }
                else
                {
                    zSmooth = Mathf.Lerp(zSmooth, 0, 1);
                }

                //Horizontal anim
                if (Input.GetKey(KeyCode.Q))
                {
                    xSmooth = Mathf.Lerp(xSmooth, -1, 1);
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    xSmooth = Mathf.Lerp(xSmooth, 1, 1);
                }
                else
                {
                    xSmooth = Mathf.Lerp(xSmooth, 0, 1);
                }
            }
            else if (targetViewNormalized.z > 0) //Right velocity
            {
                //Vertical anim
                if (Input.GetKey(KeyCode.Z))
                {
                    zSmooth = Mathf.Lerp(zSmooth, 1, 1);
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    zSmooth = Mathf.Lerp(zSmooth, -1, 1);
                }
                else
                {
                    zSmooth = Mathf.Lerp(zSmooth, 0, 1);
                }

                //Horizontal anim
                if (Input.GetKey(KeyCode.Q))
                {
                    xSmooth = Mathf.Lerp(xSmooth, 1, 1);
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    xSmooth = Mathf.Lerp(xSmooth, -1, 1);
                }
                else
                {
                    xSmooth = Mathf.Lerp(xSmooth, 0, 1);
                }
            }
        }
        #endregion

        anim.SetFloat("X", xSmooth);
        anim.SetFloat("Z", zSmooth);

        if (controller.velocity.magnitude >= 1f)
        {
            anim.SetBool("Move", true);
        }
        else
        {
            anim.SetBool("Move", false);
        }
    }
}
