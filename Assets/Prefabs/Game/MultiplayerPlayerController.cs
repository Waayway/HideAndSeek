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

    private VelocityData new_vel;
    void Start() { }

    void Update()
    {
        if (new_vel != null)
        {
            rb.position = new_vel.pos;
            rb.rotation = Quaternion.Euler(new_vel.rot.x, new_vel.rot.y, new_vel.rot.z);
            rb.velocity = new_vel.vel;
            if (anim != null)
            {
                //Debug.Log("anim: left-right: " + new_vel.anim.left_right + " walking: " + new_vel.anim.walking + " Running: " + new_vel.anim.running);
                anim.SetFloat("left-right", new_vel.anim.left_right);
                anim.SetBool("walking", new_vel.anim.walking);
                anim.SetBool("running", new_vel.anim.running);
            }
            new_vel = null;
        }
    }

    public void InitiatePrefab(string player, bool isSeekerBool, Color surfaceColor, Color seekerJointColor, Color hiderJointColor)
    {
        playerID = player;
        isSeeker = isSeekerBool;
        anim = GetComponent<Animator>();
        colorManager = GetComponent<PlayerColorManager>();
        rb = GetComponent<Rigidbody>();
        colorManager.updateColors(isSeeker ? seekerJointColor : hiderJointColor, surfaceColor);
    }

    public void updateVelocity(VelocityData data) {
        new_vel = data;
    }
    public string getId()
    {
        return this.playerID;
    }
}
