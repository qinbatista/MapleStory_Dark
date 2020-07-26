using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vzone : MonoBehaviour
{

    int player;

    // Start is called before the first frame update
    void Start()
    {
        player = LayerMask.NameToLayer("LayerPlayer");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == player) {
            Debug.Log("You Win!");
            GameManager.PlayerWin();
        }
    }
}
