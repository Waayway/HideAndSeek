using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleMaterialToObject : MonoBehaviour
{

    float SCALE = 2;

    void Start() { }

    void Update() { }
    void OnDrawGizmos()
    {
        float scale = 1 / SCALE;
        GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(this.gameObject.transform.lossyScale.x * scale, this.gameObject.transform.lossyScale.z * scale));
    }
}
