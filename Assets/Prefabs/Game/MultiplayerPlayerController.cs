using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerPlayerController : MonoBehaviour
{
    private PlayerColorManager colorManager;
    private Animator anim;
    private Rigidbody rb;

    private string playerID;
    private bool isSeeker;
    void Start() { }

    void Update()
    {

    }

    public void InitiatePrefab(string player, bool isSeekerBool, Color surfaceColor, Color seekerJointColor, Color hiderJointColor)
    {
        playerID = player;
        isSeeker = isSeekerBool;
        anim = GetComponent<Animator>();
        colorManager = GetComponent<PlayerColorManager>();
        rb = GetComponent<Rigidbody>();

        if (isSeeker)
        {
            colorManager.JointColor = seekerJointColor;
            colorManager.SurfaceColor = surfaceColor;
        }
        else
        {
            colorManager.JointColor = hiderJointColor;
            colorManager.SurfaceColor = surfaceColor;
        }
        colorManager.updateColors();
    }
    public void updateVelocity(VelocityData data) {
        transform.position = data.pos;
        transform.rotation = Quaternion.Euler(data.rot.x, data.rot.y, data.rot.z);
        rb.velocity = data.vel;
    }
}
