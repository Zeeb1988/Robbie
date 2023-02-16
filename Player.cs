using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private float xVelocity;
    private BoxCollider2D clider;
    private float jumpTime;
    
    [Header("移动参数")]
    public float speed = 8f;

    [Header("下蹲移动基数")]
    public float crouchSpeedDivisor = 3f;

    [Header("跳跃参数")]
    //跳跃力
    public float jumpForce = 6.3f;
    //长按额外增加力
    public float jumpHoldForce = 1.9f;
    //长按增加力的持续时间   单位：s
    public float jumpHoldDuration = 0.1f;
    //下蹲额外增加力
    public float jumpCrouchForce = 2.5f;

    

    [Header("状态参数")]
    //处于下蹲状态
    public bool crouch;
    //处于接触地面状态  
    public bool ground;
    //处于跳跃状态
    public bool jump;

    [Header("环境检测")]
    public LayerMask groundLayer;
    public float footOffset = 0.35f;
    public float headClearance = 0.5f;
    public float groundDistance = 0.2f;
    //按键参数
    private bool jumpTouched;
    private bool jumpHeld;
    private bool crouchHeld;

    private Vector2 colliderStandSize;
    private Vector2 colliderStandOffset;
    private Vector2 colliderCrouchSize;
    private Vector2 colliderCrouchOffset;



     
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        clider = GetComponent<BoxCollider2D>();
        colliderStandSize = clider.size;
        colliderStandOffset = clider.offset;    
        colliderCrouchSize = new Vector2(clider.size.x, clider.size.y/2f);
        colliderCrouchOffset = new Vector2(clider.offset.x, clider.offset.y/2f);
    }

    //按键监听 每帧执行一次
    void Update()
    {
        if (!jumpTouched) 
        {
            jumpTouched = Input.GetButtonDown("Jump");
        }
        
        jumpHeld = Input.GetButton("Jump");
        crouchHeld = Input.GetButton("Crouch");
        //从输入响应按键
       
        
    }
    //每0.02秒执行一次
    private void FixedUpdate()
    {
        physicsDetection();
        Movement();
        jumpMovement();
    }
    //地面检测 
    void physicsDetection()
    {
        //RaycastHit2D Check = Physics2D.Raycast(); 
        if ( clider.IsTouchingLayers(groundLayer) )
        {
            ground = true;
        }
        else { ground = false; }
    
    }
    //  人物移动 切换方向及下蹲状态
    void Movement() 
    {
        if (crouchHeld && !crouch && ground) { isCrouch(); }
        else if (!crouchHeld && crouch) { StandUp(); }
        else if (!ground && crouch) { StandUp(); }

        xVelocity = Input.GetAxis("Horizontal");

        if (crouch) { xVelocity /= crouchSpeedDivisor; }

        rb.velocity = new Vector2(xVelocity * speed, rb.velocity.y);

        if (xVelocity > 0) { transform.localScale = new Vector3(1, 1, 1); }
        if (xVelocity < 0) { transform.localScale = new Vector3(-1, 1, 1); }
    }
    //人物跳跃
    void jumpMovement()
    {
        if (jumpTouched && ground && !jump)
        {
            if (crouch && ground) 
            {
                StandUp();
                rb.AddForce(new Vector2(0f,jumpCrouchForce),ForceMode2D.Impulse);
            }
            ground = false;
            jump = true;
            //简易计时器
            jumpTime = Time.time + jumpHoldDuration;   
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            jumpTouched = false;    
        } else if (jump)
        {
            if (jumpHeld) 
            {
              rb.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);
            }
            if (jumpTime < Time.time)
            {
                jump = false;
            }
        }
        
    }
    //角色下蹲状态
    void isCrouch()
    {
        crouch = true; 
        clider.size = colliderCrouchSize;
        clider.offset = colliderCrouchOffset;
    }
    //角色站立状态
    void StandUp()
    { 
        crouch = false;
        clider.size = colliderStandSize;
        clider.offset = colliderStandOffset;
    }
}
