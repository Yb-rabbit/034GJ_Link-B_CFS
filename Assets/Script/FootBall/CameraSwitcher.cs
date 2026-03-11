using UnityEngine;
using Cinemachine; // 记得引用 Cinemachine 命名空间

public class CameraSwitcher : MonoBehaviour
{
    [Header("摄像机引用")]
    public CinemachineVirtualCamera playerCamera; // 拖入 VCam_Player
    public CinemachineVirtualCamera spectatorCamera; // 拖入 VCam_Spectator

    [Header("控制设置")]
    public MonoBehaviour playerMovementScript; // 拖入你的主角控制脚本（这个脚本里有 Update 控制移动）

    private bool isPlaying = true; // 当前是否在比赛中

    void Start()
    {
        // 游戏开始默认是玩家视角
        SwitchToPlayer();
    }

    void Update()
    {
        // 按下 Tab 键切换视角
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isPlaying)
            {
                SwitchToSpectator();
            }
            else
            {
                SwitchToPlayer();
            }
        }
    }

    // 切换到玩家视角
    void SwitchToPlayer()
    {
        isPlaying = true;
        
        // 核心逻辑：优先级抢夺
        // 数值越大，优先级越高，画面就会切给它
        playerCamera.Priority = 10;
        spectatorCamera.Priority = 5;

        // 启用玩家控制脚本
        if(playerMovementScript != null)
            playerMovementScript.enabled = true;
            
        Debug.Log("切换为：参赛模式");
    }

    // 切换到观战视角
    void SwitchToSpectator()
    {
        isPlaying = false;

        // 抢夺优先级
        playerCamera.Priority = 5;
        spectatorCamera.Priority = 10;

        // 禁用玩家控制脚本（当替补时不能动）
        if(playerMovementScript != null)
            playerMovementScript.enabled = false;
            
        Debug.Log("切换为：观战模式");
    }
}
