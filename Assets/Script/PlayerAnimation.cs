using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    Animator an;
    Rigidbody rb;
    float speed;
    void Start()
    {
        an = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");
        Vector3 inputDirection = new Vector3(hor, 0, ver).normalized;
        speed = inputDirection.magnitude;
        if(speed >= 0.2f)
        {
            an.SetFloat("Speed", speed);
        }
        else
        {
            speed = 0;
            an.SetFloat("Speed", speed);
        }
        
    }
}
