using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shrineCollecte : MonoBehaviour
{
    // Start is called before the first frame update

    int playerLayer;
    public GameObject ShrineVFXPerfabs;
    void Start()
    {
        playerLayer = LayerMask.NameToLayer("LayerPlayer");
        GameManager.RegisterOrb(this);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == playerLayer)
        {
            Instantiate(ShrineVFXPerfabs, transform.position, transform.rotation);
            AudioManger.PlayShrineAudio();
            gameObject.SetActive(false);
            GameManager.PlayerGetOrbs(this);
        }
    }
}
