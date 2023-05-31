using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class podiumScript : MonoBehaviour
{
    Animator anim;
    GameObject xbot;
    TMPro.TextMeshPro nameObj;
    PlayerColorManager pcm;
    public string id;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() { }

    public void updateNameAndReady(string name, bool ready)
    {
        if (!string.IsNullOrEmpty(name) && nameObj != null)
        {
            nameObj.SetText(name);
        }

        if (ready)
        {
            anim.SetTrigger("RaiseHand");
            nameObj.color = Color.blue;
        } else {
            nameObj.color = Color.black;
        }


    }

    public void initiatePrefab(string idString)
    {
        anim = GetComponentInChildren<Animator>();
        nameObj = GetComponentInChildren<TMPro.TextMeshPro>();
        pcm = GetComponentInChildren<PlayerColorManager>();
        pcm.updateColors(Color.cyan, Color.black);
        id = idString;
    }

}
