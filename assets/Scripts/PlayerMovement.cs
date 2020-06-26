using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rigidbody_robin;
    public float speed = 8f;
    public float crouchSpeedDivisor = 3f;
    float xVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody_robin = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate() {
        GroundMovement();
    }
    void GroundMovement()
    {
        xVelocity = Input.GetAxisRaw("Horizontal");//
        rigidbody_robin.velocity = new Vector2(xVelocity*speed, rigidbody_robin.velocity.y);
        FilpDirection();
    }
    void FilpDirection()
    {
        if (xVelocity<0)
            transform.localScale = new Vector2(-1,1);
        if (xVelocity>0)
            transform.localScale = new Vector2(1,1);
    }
}
