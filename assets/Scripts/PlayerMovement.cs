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
    public bool isHeadBlocked;
    public bool isAroundStair;
    public bool isPushed;
    public bool isHanging;
    float left_stand = 1;
    //enviernment
    public float footOffset  = 0.4f;
    public float headClearance = 0.5f;
    public float StairClearance = 0.3f;
    public float groundDistance = 0.2f;
    float playerHeight;
    public float eyeHeight = 1.5f;
    public float grabDistance = 0.4f;
    public float reachOffset = 0.7f;
    public LayerMask groundLayer;
    public LayerMask PositionMovementLayer;
    float xVelocity;
    float yVelocity;
    bool jumpPressed;
    bool jumpHeld;
    bool crouchHeld;
    bool isMoving;
    bool isUping;
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
        playerHeight = coll_robin.size.y;
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
        isUping = Input.GetButton("Vertical");
    }
    private void FixedUpdate() {
        GroundMovement();
        PhysicsCheck();
        inAirMovement();
        onStairMovement();
    }
    void PhysicsCheck()
    {
        RaycastHit2D leftCheck = Raycast(new Vector2(-footOffset,0f), Vector2.down, groundDistance, groundLayer);
        RaycastHit2D rightCheck = Raycast(new Vector2(footOffset,0f), Vector2.down, groundDistance, groundLayer);
        if (leftCheck || rightCheck)
            isOnGround = true;
        else
            isOnGround = false;
        RaycastHit2D headCheck = Raycast(new Vector2(0f, coll_robin.size.y), Vector2.up, headClearance, groundLayer);
        if(headCheck)
        {
            isHeadBlocked = true;
        }
        else
        {
            isHeadBlocked = false;
        }
        RaycastHit2D stairCheck = Raycast(new Vector2(-0.25f, coll_robin.size.y), Vector2.right, StairClearance, PositionMovementLayer);
        if(stairCheck)
        {
            isAroundStair = true;
        }
        else
        {
            isAroundStair = false;
        }

        float directtin = transform.localScale.x;
        Vector2 grabDir = new Vector2(directtin, 0f);
        // RaycastHit2D blockedCheck = Raycast(new Vector2(footOffset* directtin, playerHeight), grabDir, grabDistance, groundLayer);
        // RaycastHit2D wallCheck = Raycast(new Vector2(footOffset* directtin, eyeHeight), grabDir, grabDistance, groundLayer);
        // RaycastHit2D ledgeCheck = Raycast(new Vector2(reachOffset* directtin, playerHeight), Vector2.down, grabDistance, groundLayer);
        // print(isHeadBlocked);
        // if(isHeadBlocked)
        // {
        //     rigidbody_robin.velocity = new Vector2(rigidbody_robin.velocity.x,StandardSpeed);
        // }
        // if(!isOnGround && rigidbody_robin.velocity.x<0f && ledgeCheck && wallCheck && !blockedCheck)
        // {
        //     rigidbody_robin.bodyType  = RigidbodyType2D.Static;
        //     isHanging = true;
        // }
    }
    void GroundMovement()
    {
        FilpDirection();
        if (crouchHeld)
            Crouch();
        else if (!crouchHeld && isCrouch && !isHeadBlocked)
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

        if (isOnGround)
        {
            rigidbody_robin.velocity = new Vector2(xVelocity*StandardSpeed, rigidbody_robin.velocity.y);
        }
        else if(!isOnGround && isJump && lastVelocity!=0)
        {
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
            else if(isPushed==false)
            {
                rigidbody_robin.velocity = new Vector2(lastVelocity*StandardSpeed, rigidbody_robin.velocity.y);
            }
        }
        else if(!isOnGround && !isJump)
        {
            rigidbody_robin.velocity = new Vector2(xVelocity*StandardSpeed, rigidbody_robin.velocity.y);
        }

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
    void onStairMovement()
    {
        //BackDirection();
        yVelocity = Input.GetAxisRaw("Vertical");


        if (isAroundStair && isUping)
        {
            float ss = rigidbody_robin.transform.position.x;
            int sss = (int)rigidbody_robin.transform.position.x;
            if(Mathf.Abs(rigidbody_robin.transform.position.x-Mathf.Floor(rigidbody_robin.transform.position.x))>0.551 && lastVelocity>0)
            {
                isHanging = true;
                rigidbody_robin.bodyType  = RigidbodyType2D.Static;
                // print("isAroundStair="+isAroundStair); 
                // print("1ss:"+ss);
                // print("1sss:"+sss);
                // print("1ssss:"+Mathf.Floor(rigidbody_robin.transform.position.x));
                // print("1ssss:s"+Mathf.Abs(rigidbody_robin.transform.position.x-Mathf.Floor(rigidbody_robin.transform.position.x)));
                // print("lastVelocity="+lastVelocity);
                rigidbody_robin.transform.position = new Vector2(((Mathf.Floor(rigidbody_robin.transform.position.x))+1.55f),rigidbody_robin.transform.position.y);
            }
            else if(Mathf.Abs(rigidbody_robin.transform.position.x-Mathf.Floor(rigidbody_robin.transform.position.x))<0.551 && lastVelocity>0)
            {
                isHanging = true;
                rigidbody_robin.bodyType  = RigidbodyType2D.Static;
                // print("isAroundStair="+isAroundStair);
                // print("2ss:"+ss);
                // print("2sss:"+sss);
                // print("2ssss:"+Mathf.Floor(rigidbody_robin.transform.position.x));
                // print("2ssss:s"+Mathf.Abs(rigidbody_robin.transform.position.x-Mathf.Floor(rigidbody_robin.transform.position.x)));
                rigidbody_robin.transform.position = new Vector2(((Mathf.Floor(rigidbody_robin.transform.position.x))+0.45f),rigidbody_robin.transform.position.y);
            }

            else if(Mathf.Abs(rigidbody_robin.transform.position.x-Mathf.Floor(rigidbody_robin.transform.position.x))<0.449 && lastVelocity<0)
            {
                isHanging = true;
                rigidbody_robin.bodyType  = RigidbodyType2D.Static;
                // print("isAroundStair="+isAroundStair);
                // print("3ss:"+ss);
                // print("3sss:"+sss);
                // print("3ssss:"+Mathf.Floor(rigidbody_robin.transform.position.x));
                // print("3ssss:s"+Mathf.Abs(rigidbody_robin.transform.position.x-Mathf.Floor(rigidbody_robin.transform.position.x)));
                rigidbody_robin.transform.position = new Vector2(((Mathf.Floor(rigidbody_robin.transform.position.x))-0.55f),rigidbody_robin.transform.position.y);
            }

            else if(Mathf.Abs(rigidbody_robin.transform.position.x-Mathf.Floor(rigidbody_robin.transform.position.x))>0.449 && lastVelocity<0)
            {
                isHanging = true;
                rigidbody_robin.bodyType  = RigidbodyType2D.Static;
                // print("isAroundStair="+isAroundStair);
                // print("4ss:"+ss);
                // print("4sss:"+sss);
                // print("4ssss:"+Mathf.Floor(rigidbody_robin.transform.position.x));
                // print("4ssss:s"+Mathf.Abs(rigidbody_robin.transform.position.x-Mathf.Floor(rigidbody_robin.transform.position.x)));
                rigidbody_robin.transform.position = new Vector2(((Mathf.Floor(rigidbody_robin.transform.position.x))+0.45f),rigidbody_robin.transform.position.y);
            }
        }
        if(rigidbody_robin.bodyType  == RigidbodyType2D.Static && isAroundStair==false && isOnGround==false)
        {
            isHanging=false;
            isOnGround=true;
            // print("Dynamic");
            rigidbody_robin.bodyType  = RigidbodyType2D.Dynamic;
        }
        if (jumpPressed&&isHanging)
        {
            rigidbody_robin.bodyType  = RigidbodyType2D.Dynamic;
            isHanging = false;
            rigidbody_robin.AddForce(new Vector2(0f,jumpForce*0.3f),ForceMode2D.Impulse);
            rigidbody_robin.velocity = new Vector2( left_stand*(StandardSpeed+5), rigidbody_robin.velocity.y);
        }
        if (isHanging&&isUping)
        {
            float climbingValue = 0f;
            if (yVelocity<0)
            {
                climbingValue = -0.1f;
            }
            else
            {
                climbingValue = 0.1f;
            }
            rigidbody_robin.transform.position = new Vector2(rigidbody_robin.transform.position.x, rigidbody_robin.transform.position.y+climbingValue);
            if(isOnGround && yVelocity<0)
            {
                rigidbody_robin.bodyType  = RigidbodyType2D.Dynamic;
                isHanging = false;
                // rigidbody_robin.AddForce(new Vector2(0f,jumpForce*0.3f),ForceMode2D.Impulse);
                // rigidbody_robin.velocity = new Vector2( 0, rigidbody_robin.velocity.y);
            }
            // rigidbody_robin.velocity = new Vector2(yVelocity*StandardSpeed, rigidbody_robin.velocity.y);
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
    RaycastHit2D  Raycast(Vector2 offset, Vector2 rayDirection, float length, LayerMask layer)
    {
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(pos+ offset, rayDirection, length, layer);
        Color color = hit? Color.red : Color.green;
        Debug.DrawRay(pos+offset, rayDirection* length, color);
        return hit;
    }
}
