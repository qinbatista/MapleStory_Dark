using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerHeath : MonoBehaviour
{
    // Start is called before the first frame update

    int spikeLayer;
   
    public bool isDead;

    public int life;

    public GameObject DeathVFXPerfabs;
   
    void Start()
    {
        spikeLayer = LayerMask.NameToLayer("traps");
        GameManager.RegisterPlayerHeath(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == spikeLayer) {

            Instantiate(DeathVFXPerfabs, transform.position, transform.rotation);
            AudioManger.PlayDeathAudio();
            gameObject.SetActive(false);
            isDead = true;
            GameManager.DeadRest();
        
        }

       
    }

  
}
