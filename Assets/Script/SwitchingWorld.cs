using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SwitchingWorld : MonoBehaviour
{
    public event System.Action SwitchAstral;
    public event System.Action SwitchReal;

    bool Switched = false;
    public Camera RealCam;
    public Camera AstralCam;

    void Start()
    {
        AstralCam.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!Switched)
            {
                Switched = true;
                if(SwitchAstral != null)
                {
                    SwitchAstral();
                    SwitchReal = null;
                }

                SetCam(AstralCam,RealCam);
                Debug.Log("Flip");
            }
            else if (Switched)
            {
                Switched = false;
                if(SwitchReal != null)
                {
                    SwitchReal();
                    SwitchAstral = null;
                }
                
                
                SetCam(RealCam,AstralCam);
                Debug.Log("Flop");
            }
        }
    }

    void SetCam(Camera activate,Camera deactivate)
    {
        activate.enabled = true;
        deactivate.enabled = false;
    }
}
