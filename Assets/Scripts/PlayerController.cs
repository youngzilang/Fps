using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;

    //鼠标灵敏度
    public float xSensitivity ;
    public float ySensitivity ;

    //人物移动速度
    public float speed ;

    //跳跃力
    public float jumpForce;

    private Vector3 volocity ;
    private float xRotation = 0f;
    private bool isRunning = false;
    private bool isJumping = false;
    private bool isAiming = false;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();

        //隐藏鼠标
        Cursor.lockState = CursorLockMode.Locked;   
    }

    private void Update()
    {
        Mouse();
        Run();
        Move();
        Jump();
        Aim();

        //Debug.DrawRay(transform.position+Vector3.up*.2f, Vector3.down*0.3f,Color.green);
    }


    private void FixedUpdate()
    {
        if (isJumping)
        {
            isJumping = false;
            volocity.y = jumpForce;
        }
        rb.velocity = volocity;

    }

    //瞄准
    private void Aim()
    {
        if(Input.GetMouseButtonDown(1)) isAiming =!isAiming;

        //判断是否在瞄准状态，进行动画过渡
        float target =isAiming ? 1 : 0;


        float aim = animator.GetFloat("Aiming");
        bool isShake=target==1?true:false;

        //为瞄准增添抖动效果，更具手感
        animator.SetBool("Aim", isShake);

        animator.SetFloat("Aiming", Mathf.Lerp(aim, target, 10 * Time.deltaTime));
    }   

    //跑步
    private void Run()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if(!isRunning)
            {
                speed =6;
                isRunning = true;
                animator.SetBool("Holstered", true);
            }
            else
            {
                speed = 4;
                isRunning = false;
                animator.SetBool("Holstered", false);   
            }
        }
       
    }

    //人物移动
    private void Move()
    {
        float horizontal= Input.GetAxis("Horizontal");
        float vertical= Input.GetAxis("Vertical");

        Vector3 dir=(vertical*transform.forward + horizontal*transform.right).normalized;
        volocity = dir * speed;
        volocity.y = rb.velocity.y;

        //激活跑步动作
        animator.SetFloat("Movement", dir.magnitude);
    }

    //鼠标视角控制
    private void Mouse()
    {
        float x= Input.GetAxis("Mouse X");
        float y= Input.GetAxis("Mouse Y");

        //进行上下旋转
        xRotation -= y * ySensitivity ;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        animator.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        //进行左右旋转
        rb.transform.Rotate(Vector3.up * x * xSensitivity);
    }

    //判断是否在地面上
    private bool IsGrounded()=> Physics.Raycast(transform.position+Vector3.up*.2f, Vector3.down, 0.3f, LayerMask.GetMask("Ground"));

    //跳跃
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded()) isJumping = true;

    }
}
