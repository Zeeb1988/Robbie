using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private float xVelocity;
    private BoxCollider2D clider;
    private float jumpTime;
    
    [Header("�ƶ�����")]
    public float speed = 8f;

    [Header("�¶��ƶ�����")]
    public float crouchSpeedDivisor = 3f;

    [Header("��Ծ����")]
    //��Ծ��
    public float jumpForce = 6.3f;
    //��������������
    public float jumpHoldForce = 1.9f;
    //�����������ĳ���ʱ��   ��λ��s
    public float jumpHoldDuration = 0.1f;
    //�¶׶���������
    public float jumpCrouchForce = 2.5f;

    

    [Header("״̬����")]
    //�����¶�״̬
    public bool crouch;
    //���ڽӴ�����״̬  
    public bool ground;
    //������Ծ״̬
    public bool jump;

    [Header("�������")]
    public LayerMask groundLayer;
    public float footOffset = 0.35f;
    public float headClearance = 0.5f;
    public float groundDistance = 0.2f;
    //��������
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

    //�������� ÿִ֡��һ��
    void Update()
    {
        if (!jumpTouched) 
        {
            jumpTouched = Input.GetButtonDown("Jump");
        }
        
        jumpHeld = Input.GetButton("Jump");
        crouchHeld = Input.GetButton("Crouch");
        //��������Ӧ����
       
        
    }
    //ÿ0.02��ִ��һ��
    private void FixedUpdate()
    {
        physicsDetection();
        Movement();
        jumpMovement();
    }
    //������ 
    void physicsDetection()
    {
        //RaycastHit2D Check = Physics2D.Raycast(); 
        if ( clider.IsTouchingLayers(groundLayer) )
        {
            ground = true;
        }
        else { ground = false; }
    
    }
    //  �����ƶ� �л������¶�״̬
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
    //������Ծ
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
            //���׼�ʱ��
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
    //��ɫ�¶�״̬
    void isCrouch()
    {
        crouch = true; 
        clider.size = colliderCrouchSize;
        clider.offset = colliderCrouchOffset;
    }
    //��ɫվ��״̬
    void StandUp()
    { 
        crouch = false;
        clider.size = colliderStandSize;
        clider.offset = colliderStandOffset;
    }
}
