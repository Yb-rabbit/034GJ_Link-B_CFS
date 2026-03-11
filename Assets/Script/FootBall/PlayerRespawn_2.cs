using UnityEngine;

/// <summary>
/// 玩家重生管理器
/// 功能：掉落检测、按键重生、位置重置、音频播放
/// </summary>
public class PlayerRespawn_2 : MonoBehaviour
{
    [Header("重生设置")]
    [Tooltip("将场景中的重生点物体拖拽到这里")]
    public Transform respawnPoint;
    [Tooltip("如果没有指定重生点，则使用此默认坐标")]
    public Vector3 defaultPosition = new Vector3(0, 5, 0); // 默认重生位置

    [Header("按键与检测")]
    [Tooltip("按下此键重生")]
    public KeyCode respawnKey = KeyCode.R;
    [Tooltip("低于此高度视为掉落出地图")]
    public float fallLimitY = -5f; // 掉落检测阈值

    [Header("音效设置")]
    public AudioClip respawnSound; // 重生音效

    private Rigidbody rb; // 刚体组件

    void Start()
    {
        // 获取刚体组件
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("PlayerRespawn: 没有找到Rigidbody组件，重生功能可能无法正常工作");
        }
    }

    void Update()
    {
        // 1. 按键手动重生
        if (Input.GetKeyDown(respawnKey))
        {
            RespawnPlayer();
        }

        // 2. 掉落出地图自动重生
        if (transform.position.y < fallLimitY)
        {
            RespawnPlayer();
        }
    }

    /// <summary>
    /// 重生方法（可供其他脚本调用）
    /// </summary>
    public void RespawnPlayer()
    {
        // 1. 取消鼠标锁定（让鼠标可见并可操作）
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // 2. 重置玩家位置
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
        }
        else
        {
            transform.position = defaultPosition;
        }

        // 3. 重置刚体状态（防止重生后继续下落）
        if (rb != null)
        {
            rb.velocity = Vector3.zero; // 清空速度
            rb.angularVelocity = Vector3.zero; // 清空角速度
        }

        // 4. 播放重生音效
        if (respawnSound != null)
        {
            AudioSource.PlayClipAtPoint(respawnSound, transform.position);
        }
    }
}
