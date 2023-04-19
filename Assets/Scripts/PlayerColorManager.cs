using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColorManager : MonoBehaviour
{
    public GameObject Joints;
    public GameObject Surfaces;

    public Color JointColor;
    public Color SurfaceColor;
    void Start(){
        updateColors();
    }   
    void Update(){}

    public void updateColors() {
        Joints.GetComponent<Renderer>().sharedMaterial.color = JointColor;
        Surfaces.GetComponent<Renderer>().sharedMaterial.color = SurfaceColor;
    }
}
