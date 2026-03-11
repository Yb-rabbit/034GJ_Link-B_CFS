using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 8f; // 移动速度
    public float turnSpeed = 10f; // 旋转平滑度
    public float sprintMultiplier = 1.5f; // 冲刺倍率

    [Header("x-z平面边界设置")]
    public float boundaryMinX = -10f; // x轴最小边界
    public float boundaryMaxX = 10f;  // x轴最大边界
    public float boundaryMinZ = -10f; // z轴最小边界
    public float boundaryMaxZ = 10f;  // z轴最大边界

    [Header("传送设置")]
    public Vector3 respawnPosition = new Vector3(0, 5, 0); // 传送目标位置（如平台中心）

    private Rigidbody rb;
    private Vector3 moveInput;
    private Camera mainCamera; // 仅用来获取视角方向，不控制摄像机

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // 冻结旋转，防止方块摔倒
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        // 自动获取场景里的主摄像机
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 如果摄像机丢了（极少情况），尝试重新获取
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }

        // 1. 获取键盘输入
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 2. 计算相对于摄像机的移动方向
        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camRight = mainCamera.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();
        moveInput = (camForward * v + camRight * h).normalized;

        // 3. 角色旋转（只有有输入时才转，防止归零报错）
        if (moveInput != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        // 【核心】x-z平面边界检测：超出范围时传送
        if (transform.position.x < boundaryMinX || transform.position.x > boundaryMaxX ||
            transform.position.z < boundaryMinZ || transform.position.z > boundaryMaxZ)
        {
            RespawnPlayer();
        }
    }

    void FixedUpdate()
    {
        // 4. 物理移动
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;
        if (moveInput != Vector3.zero)
        {
            rb.MovePosition(rb.position + moveInput * currentSpeed * Time.fixedDeltaTime);
        }
    }

    // 传送玩家到指定位置
    void RespawnPlayer()
    {
        transform.position = respawnPosition; // 设置位置
        rb.velocity = Vector3.zero; // 重置速度，防止继续移动
        rb.angularVelocity = Vector3.zero; // 重置角速度
    }

    // 当脚本被禁用时（按Tab切换队伍时）
    void OnDisable()
    {
        moveInput = Vector3.zero;
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
