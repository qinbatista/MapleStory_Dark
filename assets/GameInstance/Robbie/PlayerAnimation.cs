using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    Animator anim;

    PlayerMoveClass movement;
    Rigidbody2D rb;
    int isGroundID;
    int isJumpingID;
    int isHangingID;
    int isCrouchingID;
    int speedID;
    int fallID;

    void Start()
    {
        anim = GetComponent<Animator>();
        movement = GetComponentInParent<PlayerMoveClass>();
        rb = GetComponentInParent<Rigidbody2D>();
        //使用将string参数转换为hash值的方法，可以避免移动设备中某些编码问题(推荐使用此方法)
        speedID = Animator.StringToHash("speed");
        isGroundID = Animator.StringToHash("isOnGround");
        isJumpingID = Animator.StringToHash("isJumping");
        isHangingID = Animator.StringToHash("isHanging");
        isCrouchingID = Animator.StringToHash("isCrouching");
        fallID = Animator.StringToHash("verticalVelocity");
    }

    // Update is called once per frame
    void Update()
    {
        //anim.SetFloat("speed", Mathf.Abs(movement.xVelocity));
        anim.SetFloat(speedID, Mathf.Abs(movement.xVelocity));
        anim.SetBool(isGroundID, movement.isOnGround);
        anim.SetBool(isJumpingID, movement.isJump);
        anim.SetBool(isHangingID, movement.isHanging);
        anim.SetBool(isCrouchingID, movement.isCrouch);

        anim.SetFloat(fallID,rb.velocity.y); //与Animator中的verticalVelocity值对应（y轴动力加速度对应的动画）

    }
}
