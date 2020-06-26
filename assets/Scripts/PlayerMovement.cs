using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rigidbody_robin;
    private BoxCollider2D coll_robin;
    //movement
    public float StandardSpeed = 8f;
    public float crouchSpeedDivisor = 3f;
    public float notOnGroundSpeedDivisor = 10f;
    //juamp
    public float jumpForce = 60f;
    public float jumpPush = 1000f;
    public float jumpHoldForce = 1.9f;
    public float juampHoldDuration = 0.1f;
    public float crouchJumpBoost = 2.5f;
    public float firstJump = 0;
    public float secondJump = 0;
    float jumpTime;
    public float JumpinAirMinDuration = 0.1f;
    public float JumpinAirMaxDuration = 0.3f;
    //status
    public bool isCrouch;
    public bool isOnGround;
    public bool isJump;
    public bool isPushed;
    float left_stand = 1;
    //enviernment
    public LayerMask groundLayer;
    float xVelocity;
    bool jumpPressed;
    bool jumpHeld;
    bool crouchHeld;
    bool isMoving;
    float lastVelocity;
    //collider
    Vector2 colliderStandSize;
    Vector2 colliderStandOffset;
    Vector2 ColliderCrushSize;
    Vector2 ColliderCrushOffset;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody_robin = GetComponent<Rigidbody2D>();
        coll_robin = GetComponent<BoxCollider2D>();
        colliderStandSize = coll_robin.size;
        colliderStandOffset = coll_robin.offset;
        ColliderCrushSize = new Vector2(coll_robin.size.x, coll_robin.size.y/2f);
        ColliderCrushOffset =  new Vector2(coll_robin.offset.x, coll_robin.offset.y/2f);
    }

    // Update is called once per frame
    void Update()
    {
        jumpPressed = Input.GetButtonDown("Jump");
        jumpHeld = Input.GetButton("Jump");
        crouchHeld = Input.GetButton("Crouch");
        isMoving = Input.GetButton("Horizontal");
    }
    private void FixedUpdate() {
        GroundMovement();
        PhysicsCheck();
        inAirMovement();
    }
    void PhysicsCheck()
    {
        if (coll_robin.IsTouchingLayers(groundLayer))
            isOnGround = true;
        else
            isOnGround = false;
    }
    void GroundMovement()
    {
        FilpDirection();
        if (crouchHeld)
            Crouch();
        else if (!crouchHeld && isCrouch)
            StandUp();
        xVelocity = Input.GetAxisRaw("Horizontal");
        if (xVelocity >0 && isJump==false)
            lastVelocity=1;
        else if (xVelocity <0&& isJump==false)
            lastVelocity=-1;
        else if (xVelocity == 0 &&isJump==false)
            lastVelocity=0;

        if (isCrouch)
            xVelocity /= crouchSpeedDivisor;
        else if (!isOnGround && isMoving)
        {
            xVelocity /= notOnGroundSpeedDivisor;
        }

        if (isOnGround)
        {
            rigidbody_robin.velocity = new Vector2(xVelocity*StandardSpeed, rigidbody_robin.velocity.y);
        }
        else if(!isOnGround && isJump && lastVelocity!=0)
        {
            if(jumpPressed && !isOnGround && isPushed==false)
            {
                print("psuh");
                secondJump = Time.time;
                if (secondJump-firstJump<JumpinAirMaxDuration&& secondJump-firstJump>JumpinAirMinDuration)
                {
                    rigidbody_robin.AddForce(new Vector2(0f,jumpForce*0.3f),ForceMode2D.Impulse);
                    rigidbody_robin.AddForce(new Vector2(jumpPush*left_stand,0f),ForceMode2D.Impulse);
                    isPushed=true;
                }
            }
            else if(isPushed==false)
            {
                // print("no psuh");
                rigidbody_robin.velocity = new Vector2(lastVelocity*StandardSpeed, rigidbody_robin.velocity.y);
            }
        }
        else if(!isOnGround && !isJump)
        {
            rigidbody_robin.velocity = new Vector2(xVelocity*StandardSpeed, rigidbody_robin.velocity.y);
        }
        // if (!isOnGround && !isJump && isMoving)
        // {
        //     rigidbody_robin.velocity = new Vector2(xVelocity*StandardSpeed, rigidbody_robin.velocity.y);
        // }
        // else if(!isOnGround && !isJump && isMoving)
        // {
        //     print("3: xVelocity="+xVelocity +" xVelocity="+xVelocity);
        //     rigidbody_robin.velocity = new Vector2(xVelocity*StandardSpeed, rigidbody_robin.velocity.y);
        // }

    }
    void inAirMovement()
    {
        //jump up
        if (jumpHeld && isOnGround && !isJump)
        {
            isJump = true;
            isOnGround=false;
            rigidbody_robin.AddForce(new Vector2(0f,jumpForce),ForceMode2D.Impulse);
            firstJump = Time.time;
        }
        else if (isOnGround && isJump)
        {
            // if(jumpHeld)
            //     rigidbody_robin.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);
            // if (jumpTime< Time.time)
            isJump = false;
            isPushed = false;
        }
        else if (!isOnGround)
        {
            isJump = true;
        }
        //jump forward
        if(jumpPressed && !isOnGround && isPushed==false)
        {
            secondJump = Time.time;
            if (secondJump-firstJump<JumpinAirMaxDuration&& secondJump-firstJump>JumpinAirMinDuration)
            {
                rigidbody_robin.AddForce(new Vector2(0f,jumpForce*0.3f),ForceMode2D.Impulse);
                rigidbody_robin.AddForce(new Vector2(jumpPush*left_stand,0f),ForceMode2D.Impulse);
                isPushed=true;
            }
        }


    }

    void FilpDirection()
    {
        if (xVelocity<0)
        {
            transform.localScale = new Vector2(-1,1);
            left_stand = -1;
        }
        if (xVelocity>0)
        {
            transform.localScale = new Vector2(1,1);
            left_stand = 1;
        }
    }
    void Crouch()
    {
        isCrouch =true;
        coll_robin.size = ColliderCrushSize;
        coll_robin.offset = ColliderCrushOffset;
    }
    void StandUp()
    {
        isCrouch=false;
        coll_robin.size = colliderStandSize;
        coll_robin.offset = colliderStandOffset;
    }
}
