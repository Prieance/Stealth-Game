using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSetter : MonoBehaviour
{
    public GameObject Real;
    public GameObject Astral;
    void Start()
    {
        Astral.transform.position = Real.transform.position;
    }
}
