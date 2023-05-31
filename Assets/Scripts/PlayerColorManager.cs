using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColorManager : MonoBehaviour
{    
    void Start(){
    }   
    void Update(){}


    public void updateColors(Color JointColor, Color SurfaceColor)
    {
        transform.Find("Beta_Joints").GetComponent<Renderer>().material.color = JointColor;
        transform.Find("Beta_Surface").GetComponent<Renderer>().material.color = SurfaceColor;
    }
}
