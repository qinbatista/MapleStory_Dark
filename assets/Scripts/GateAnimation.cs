using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateAnimation : MonoBehaviour
{

    Animator anim;
    int openID;
    public bool isOpen;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        openID = Animator.StringToHash("Open");
        GameManager.RegisterGate(this);
    }

    // Update is called once per frame
    public void Open()
    {
        isOpen = true;
        anim.SetTrigger(openID);
        AudioManger.PlayOpenDoorAudio();
    }
}
