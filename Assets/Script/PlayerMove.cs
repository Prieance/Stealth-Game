using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public event System.Action PlayerHasWon;
    public event System.Action PlayerIsCaught;

    public float MoveSpeed = 0;
    public float CrouchSpeed = 0;
    public float SprintSpeed = 0;
    public float smoothMoveTime = .1f;
    public float turnSpeed = 90;

    Rigidbody rb;

    Vector3 velocity;
    public Transform camA;
    public Transform camR;
    Transform cam;

    float angle;
    public float targetAngle;
    float smoothTurnVelocity;

    float horizontal = 0;
    float vertical = 0;

    bool Caught;
    bool Crouch;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

   
    private void Update()
    {
        Vector3 inputDirection = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.C))
        {
            Crouch = !Crouch;
        }

        if (camA.gameObject.activeSelf)
        {
            cam = camA;
        }
        else if (camR.gameObject.activeSelf)
        {
            cam = camR;
        }
        
        if (!Caught)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
        }
        else
        {
            horizontal = 0;
            vertical = 0;
        }
        inputDirection = new Vector3(horizontal, 0, vertical).normalized;
        float inputMagnitude = inputDirection.magnitude;
        

        targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude);

        if (inputDirection.magnitude >= 0.1f)
        {
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                velocity = moveDir * SprintSpeed;
                Crouch = false;
            }
            else if (Crouch)
            {
                velocity = moveDir * CrouchSpeed;
            }
            else
            {
                velocity = moveDir * MoveSpeed;
            }
        }
        else
        {
            velocity = Vector3.zero;
        }
    }

    void Disabled()
    {
        Caught = true;
    }

    private void FixedUpdate()
    {
        rb.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        rb.MovePosition(rb.position + velocity * Time.deltaTime);
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Goal"))
        {
            Disabled();
            if(PlayerHasWon != null)
            {
                PlayerHasWon();
            }
        }
    }

    void OnCollisionEnter(Collision collide)
    {
        if (collide.gameObject.CompareTag("Enemy"))
        {
            Disabled();
            if (PlayerIsCaught != null)
            {
                PlayerIsCaught();
            }
        }
    }
}
