using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour
{
    // Start is called before the first frame update

    Animator anim;
    int fadeID;
    void Start()
    {
        anim = GetComponent<Animator>();
        fadeID = Animator.StringToHash("Fade");
        GameManager.RegisterFader(this);
    }

    // Update is called once per frame
    public void FadeOut() {
        anim.SetTrigger(fadeID);
    }
}
