using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;

    [Header("鼠标灵敏度")]
    public float xSensitivity;
    public float ySensitivity;

    [Header("移动速度")]
    public float walkSpeed;
    public float runSpeed;

    [Header("跳跃力量")]
    public float jumpForce;

    [Header("加速度减速度")]
    public float accel;
    public float deaccel;

    private Vector3 volocity;
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

        rb.drag = 0;
    }

    private void Update()
    {
        Mouse();
        Run();
        MoveInput();
        Jump();
        Aim();

        //Debug.DrawRay(transform.position+Vector3.up*.2f, Vector3.down*0.3f,Color.green);
    }


    private void FixedUpdate()
    {
        Move();
        if (isJumping)
        {
            isJumping = false;
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }

    }

    //瞄准
    private void Aim()
    {
        if (Input.GetMouseButtonDown(1)) isAiming = !isAiming;

        //判断是否在瞄准状态，进行动画过渡
        float target = isAiming ? 1 : 0;


        float aim = animator.GetFloat("Aiming");
        bool isShake = target == 1 ? true : false;

        //为瞄准增添抖动效果，更具手感
        animator.SetBool("Aim", isShake);

        animator.SetFloat("Aiming", Mathf.Lerp(aim, target, 10 * Time.deltaTime));
    }

    //跑步
    private void Run()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRunning = !isRunning;
            animator.SetBool("Holstered", isRunning);
        }

    }

    //获取人物移动输入
    private void MoveInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        volocity = new Vector3(horizontal, 0, vertical).normalized;

    }

    //平滑处理人物移动
    private void Move()
    {
        //记录人物当前垂直速度，保持跳跃状态不受水平移动影响
        float yVel = rb.velocity.y;

        //获取人物当前速度
        Vector3 flatvel =new Vector3(rb.velocity.x, 0, rb.velocity.z);
        //获取人物目标速度
        float targetSpeed = isRunning ? runSpeed : walkSpeed;
        Vector3 targetVel = transform.TransformDirection(volocity) * targetSpeed;

        if(volocity.magnitude > 0.1f)
        {
            rb.velocity = Vector3.Lerp(flatvel, targetVel, accel * Time.deltaTime);
        }
        else
        {
            rb.velocity = Vector3.Lerp(flatvel, Vector3.zero, deaccel * Time.deltaTime);
        }

        //还原垂直速度
        rb.velocity = new Vector3(rb.velocity.x, yVel, rb.velocity.z);

        animator.SetFloat("Movement", volocity.magnitude);
    }


    //鼠标视角控制
    private void Mouse()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        //进行上下旋转
        xRotation -= y * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        animator.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        //进行左右旋转
        rb.transform.Rotate(Vector3.up * x * xSensitivity);
    }

    //判断是否在地面上
    private bool IsGrounded() => Physics.Raycast(transform.position + Vector3.up * .2f, Vector3.down, 0.3f, LayerMask.GetMask("Ground"));

    //跳跃
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded()) isJumping = true;

    }
}
