using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    public Animator an;
    public EnemyMove EMScript;
    public float test;
    void Start()
    {
        
    }

    
    void Update()
    {
        test = EMScript.movePosition.magnitude;
        an.SetFloat("Speed",test);
        an.SetBool("playerSpotted", EMScript.playerSpotted);
        an.SetBool("Stop", EMScript.stop);

    }
}
