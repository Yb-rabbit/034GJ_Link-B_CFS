using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 8f;           // 移动速度
    public float turnSpeed = 10f;          // 旋转平滑度
    public float sprintMultiplier = 1.5f;  // 冲刺倍率

    private Rigidbody rb;
    private Vector3 moveInput;
    private Camera mainCamera; // 仅用来获取视角方向，不控制摄像机

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // 冻结旋转，防止方块摔倒
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        
        // 自动获取场景里的主摄像机
        // 前提：你的 Main Camera 标签必须是 "MainCamera"
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 如果摄像机丢了（极少情况），尝试重新获取
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            // 如果还是空，就停止这一帧的逻辑
            if (mainCamera == null) return;
        }

        // 1. 获取键盘输入
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 2. 【核心】计算相对于摄像机的移动方向
        // 获取摄像机的正前方和正右方向量
        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camRight = mainCamera.transform.right;

        // 把 Y 轴归零，确保我们在平面上移动（摄像机如果是俯视的，forward 会有向下的分量）
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize(); // 重新计算长度为1，保证速度一致
        camRight.Normalize();

        // 最终的移动方向 = 摄像机前方 * 前后键 + 摄像机右方 * 左右键
        moveInput = (camForward * v + camRight * h).normalized;

        // 3. 角色旋转（只有有输入时才转，防止归零报错）
        if (moveInput != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        // 4. 物理移动
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;

        // 使用 MovePosition 进行物理移动，推人手感更好
        if (moveInput != Vector3.zero)
        {
            rb.MovePosition(rb.position + moveInput * currentSpeed * Time.fixedDeltaTime);
        }
    }

    // 【新增】当脚本被禁用时（也就是按Tab切换队伍，TeamManager把你关掉时）
    void OnDisable()
    {
        // 清空输入，并让刚体瞬间停止
        moveInput = Vector3.zero;
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
