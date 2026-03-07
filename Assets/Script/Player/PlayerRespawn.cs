using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("重生设置")]
    [Tooltip("将场景中的重生点物体拖拽到这里")]
    public Transform respawnPoint;

    [Tooltip("如果没有指定重生点，则使用此默认坐标")]
    public Vector3 defaultPosition = new Vector3(0, 1, 0);

    [Header("按键与检测")]
    [Tooltip("按下此键重生")]
    public KeyCode respawnKey = KeyCode.R;
    
    [Tooltip("低于此高度视为掉落出地图")]
    public float fallLimitY = -10f;

    [Header("音效设置")]
    public AudioClip respawnSound;

    private CharacterController controller;

    void Start()
    {
        // 获取角色控制器组件
        controller = GetComponent<CharacterController>();
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
        // --- 核心步骤 ---
        // 必须先禁用 CharacterController，否则设置位置可能无效
        if (controller != null)
        {
            controller.enabled = false;
        }
        // 关闭鼠标显示
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // 执行位置重置
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
        }
        else
        {
            transform.position = defaultPosition;
        }

        // 重新启用 CharacterController
        if (controller != null)
        {
            controller.enabled = true;
        }
        //Debug.Log("玩家已重生！");

        // 重生音频播放
        if (respawnSound != null)
        {
            AudioSource.PlayClipAtPoint(respawnSound, transform.position);
        }

    }
}
