using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMoveClass : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;

    [Header("移动参数")]
    public float speed = 8f;
    public float crouchSpeedDivisor = 3f; //除与下蹲基数
    public float xVelocity; //x轴力的方向

    [Header("跳跃参数")]
    public float jumpForce = 6.3f; //跳跃高度
    public float jumpHoldForce = 1.9f;  //长按跳跃加成
    public float jumpHoldTime = 0.1f;  //长按跳跃时间
    public float crouchJumpForce = 2.5f; //蹲下跳跃加成

    float jumpTime;

    [Header("状态参数")]
    public bool isCrouch;
    public bool isOnGround;
    public bool isJump;
    public bool isHeadBlock;
    public bool isHanging;

    [Header("环境检查")]
    public float footOffset = 0.4f;
    public float headDistance = 0.5f;
    public float groundDistance = 0.2f;

    float playerHeight;
    public float eyeHeight = 1.5f;
    public float grabDistance = 0.4f;
    public float reachOffset = 0.7f;

    public LayerMask groundLayer;//地面层，面板中设置

    //按键设置
    bool jumpPress;
    bool jumpHold;
    bool crouchHold;
    bool crouchPress;
    bool jumpPressUp;

    //碰撞尺寸
    Vector2 StandSize;  //站立大小
    Vector2 StandOffset;  //站立尺寸
    Vector2 CrouchSize;  //下蹲大小
    Vector2 CrouchOffset; //下蹲尺寸

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        playerHeight = coll.size.y;
        StandSize = coll.size;
        StandOffset = coll.offset;
        CrouchSize = new Vector2(coll.size.x, coll.size.y / 2f);
        CrouchOffset = new Vector2(coll.offset.x, coll.offset.y / 2f);

    }
    private void Update()
    {
        if (GameManager.isGameOverBool())
            return;

        jumpPress = Input.GetButtonDown("Jump"); //按下
        jumpHold = Input.GetButton("Jump");  //长按
        crouchPress = Input.GetButtonDown("Crouch");
        crouchHold = Input.GetButton("Crouch");

        jumpPressUp = Input.GetButtonUp("Jump");
    }
    private void FixedUpdate() //刚体速度需要用fixedupdate
    {
        if (GameManager.isGameOverBool())
            return;
        
        EnvironmentCheck();
        GroundMovement();
        jumpDo();
    }

    void GroundMovement() { //获取键盘输入水平移动
        if (isHanging)
            return;

        if (crouchHold && !isCrouch && isOnGround)    //如果按下下蹲键，且不是下蹲状态必须在地面上，才执行下蹲
            Crouch();
        else if ((!crouchHold  || !isOnGround) && isCrouch && !isHeadBlock) // 没按下蹲或者不在地面，并且是下蹲状态时键执行站立
            StandUp();
        

        xVelocity = Input.GetAxis("Horizontal"); //范围 -1 到 1  

        if (isCrouch)  //如果是下蹲状态，提前降低速度
            xVelocity /= crouchSpeedDivisor;
        rb.velocity = new Vector2(xVelocity * speed, rb.velocity.y);
        FlipDirection();
    }

    void jumpDo() {

        if (isHanging) {
            if (jumpPress) {
                rb.bodyType = RigidbodyType2D.Dynamic;
                isHanging = false;
                rb.velocity = new Vector2(rb.velocity.x, 15f);
            }

            if (crouchPress) {
                rb.bodyType = RigidbodyType2D.Dynamic;
                isHanging = false;
            }
        }

        if (jumpPress && isOnGround && !isJump && !isHeadBlock)
        {
            if (isCrouch) {
                StandUp();
                rb.AddForce(new Vector2(0f, crouchJumpForce), ForceMode2D.Impulse);
            }

            isOnGround = false;
            isJump = true;

            jumpTime = Time.time + jumpHoldTime;
            //用增加y轴二维向量的方式来实现跳跃，Impulse（突然的动力效果）
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            AudioManger.PlayJumpAudio();
        }
        else if (isJump) {
            if (jumpHold)
                rb.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);
            if (jumpTime < Time.time) 
                isJump = false;
        }
    }
    void FlipDirection(){ //翻转人物方向
        if (xVelocity < 0) //如果x小于0，说明角色向左，开始翻转角色
            transform.localScale = new Vector3(-1, 1,1);
        if (xVelocity > 0) //如果x小于0，说明角色向左，开始翻转角色
            transform.localScale = new Vector3(1, 1,1);
    }

    //环境检查
    void EnvironmentCheck() {

        //给角色离地的两个顶点添加射线，这样可以判断是左脚还是右脚与地面接触
        //=====参数：Raycast(从哪射，往哪射，射多远，射谁)
        RaycastHit2D leftFootCheck = Raycast(new Vector2(-footOffset,0f), Vector2.down, groundDistance,groundLayer);
        RaycastHit2D rightFootCheck = Raycast(new Vector2(footOffset, 0f), Vector2.down, groundDistance, groundLayer);


        //角色是否与地面接触  //coll.IsTouchingLayers(groundLayer )方法
        //判断脚部射线是否射到地面
        if (leftFootCheck || rightFootCheck)
        {
            isOnGround = true;
          
        } 
        else isOnGround = false;
        
        RaycastHit2D headCheck = Raycast(new Vector2(0f, coll.size.y), Vector2.up, headDistance, groundLayer);//添加头顶射线

        if (headCheck)
            isHeadBlock = true;
        else isHeadBlock = false;

        float direction = transform.localScale.x;
        Vector2 grabDir = new Vector2(direction, 0f);

        //添加头部向前的射线（判断头顶前方有无遮挡） 1号线
        RaycastHit2D headGrabCheck = Raycast(new Vector2(footOffset * direction, playerHeight), grabDir, grabDistance, groundLayer);
        //添加眼部前方的射线（判断眼部前方有无遮挡）2号线
        RaycastHit2D eyesGrabCheck = Raycast(new Vector2(footOffset * direction, eyeHeight), grabDir, grabDistance, groundLayer);
        //添加头顶前方向下的射线（判断头顶前方射线向下垂直一段距离有没有遮挡）3号线
        RaycastHit2D headDownGrabCheck = Raycast(new Vector2(reachOffset * direction, playerHeight), Vector2.down, grabDistance, groundLayer);


        //满足悬挂的条件：1.不在地面 2.下落状态时（y轴上力为负）3. 1号线未被遮挡 4. 2号线和3号线被遮挡
        if(!isOnGround  && eyesGrabCheck && headDownGrabCheck && !headGrabCheck){
            rb.bodyType = RigidbodyType2D.Static; //刚体置为静态
            isHanging = true;
        }
    }

    void Crouch() {
        isCrouch = true;
        coll.size = CrouchSize;
        coll.offset = CrouchOffset;
       
    }
    void StandUp()
    {
        isCrouch = false;
        coll.size = StandSize;
        coll.offset = StandOffset;
    }

    //重载Raycast方法，省去了每次生成射线时重复方法内的步骤
    RaycastHit2D Raycast(Vector2 offset, Vector2 rayDiraction, float length, LayerMask layer) {
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(pos+offset, rayDiraction, length, layer);
        Debug.DrawRay(pos + offset, rayDiraction * length);
        return hit;
    }
}
