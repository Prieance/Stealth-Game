using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SwitchingWorld : MonoBehaviour
{
    public static event System.Action SwitchWorld;

    bool Switched = false;

    public GameObject Real;
    public GameObject Astral;

    void Start()
    {
        Astral.SetActive(false);
        UI.ForceBack += ForceIntoReal;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!Switched)
            {
                Switched = true;
                Setlevel(Astral, Real);
                Debug.Log("Flip");
            }
            else if (Switched)
            {
                Switched = false;
                Setlevel(Real, Astral);
                Debug.Log("Flop");
            }

            if (SwitchWorld != null)
            {
                SwitchWorld();
            }
        }
    }

    void Setlevel(GameObject activate, GameObject deactivate)
    {
        activate.SetActive(true);
        deactivate.SetActive(false);
    }
    
    void ForceIntoReal()
    {
        Switched = false;
        Setlevel(Real, Astral);
        Debug.Log("ForcedFlop");
    }
}
