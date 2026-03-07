using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("=== 移动设置 ===")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;

    [Header("=== 组件引用 ===")]
    [Tooltip("必须手动拖拽 CharacterController")]
    public CharacterController controller;
    
    [Tooltip("必须手动拖拽 Main Camera (或者 Cinemachine 下的 Camera)")]
    public Transform cameraTransform;

    // 内部变量
    private Vector3 _moveDirection;
    private float _verticalSpeed;

    void Start()
    {
        // 自动查找组件（防止手动拖拽遗漏）
        if (controller == null) controller = GetComponent<CharacterController>();
        if (cameraTransform == null) cameraTransform = Camera.main.transform;

        // 游戏启动时，默认锁定鼠标
        SetCursorState(true);
    }

    void Update()
    {
        HandleMovement();
        HandleCursorToggle();
    }

    // 处理移动和重力
    private void HandleMovement()
    {
        // 1. 获取键盘输入
        float horizontal = Input.GetAxis("Horizontal"); // A / D
        float vertical = Input.GetAxis("Vertical");     // W / S

        // 2. 基于摄像机朝向计算移动向量
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        // 关键：将 Y 轴设为 0，确保玩家永远只在水平面上移动
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // 组合方向
        Vector3 move = (camForward * vertical) + (camRight * horizontal);

        // 3. 处理奔跑速度
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // 4. 处理重力
        if (controller.isGrounded)
        {
            _verticalSpeed = -2f; // 轻微向下压，确保贴地
        }
        else
        {
            _verticalSpeed += Physics.gravity.y * Time.deltaTime;
        }

        // 5. 应用最终移动
        Vector3 finalMove = (move * currentSpeed) + (Vector3.up * _verticalSpeed);
        controller.Move(finalMove * Time.deltaTime);
    }

    // 处理 ESC 键手动切换鼠标
    private void HandleCursorToggle()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 如果当前是锁定，就解锁；否则锁定
            SetCursorState(Cursor.lockState != CursorLockMode.Locked);
        }
    }

    /// <summary>
    /// 公共接口：控制鼠标状态（锁定/解锁）
    /// </summary>
    /// <param name="isLocked">true = 锁定鼠标(游戏模式), false = 解锁鼠标(UI模式)</param>
    public void SetCursorState(bool isLocked)
    {
        if (isLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
