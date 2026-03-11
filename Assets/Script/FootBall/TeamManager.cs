using UnityEngine;
using UnityEngine.AI;
using Cinemachine;

public class TeamManager : MonoBehaviour
{
    public static TeamManager Instance; // 单例
    [Header("摄像机设置")]
    public CinemachineVirtualCamera vcamPlayer;   // 跟随玩家的摄像机
    public CinemachineVirtualCamera vcamSpectator; // 观战摄像机（锁定球）
    [Header("队伍成员")]
    public Transform[] redTeamPlayers;
    public Transform[] blueTeamPlayers;
    [Header("球")]
    public Transform ball; // 正中心的锁定对象

    private enum PlayMode { RedTeam, BlueTeam, Spectator }
    private PlayMode currentMode = PlayMode.Spectator;
    private Transform currentControlledPlayer;

    void Awake() { Instance = this; }

    void Start()
    {
        // 初始化：观战模式锁定球
        SetMode(PlayMode.Spectator);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) { SwitchMode(); }
    }

    void SwitchMode()
    {
        if (currentMode == PlayMode.Spectator) currentMode = PlayMode.RedTeam;
        else if (currentMode == PlayMode.RedTeam) currentMode = PlayMode.BlueTeam;
        else if (currentMode == PlayMode.BlueTeam) currentMode = PlayMode.Spectator;
        SetMode(currentMode);
    }

    void SetMode(PlayMode mode)
    {
        currentMode = mode;

        // 1. 清理旧状态（归还AI控制权）
        if (currentControlledPlayer != null) { EnablePlayerControl(currentControlledPlayer, false); }

        if (mode == PlayMode.Spectator)
        {
            // 观战模式：锁定球
            vcamSpectator.Priority = 20; // 高优先级
            vcamPlayer.Priority = 5;     // 低优先级
            vcamSpectator.Follow = ball; // 锁定球
            vcamSpectator.LookAt = ball; // 锁定球

            // 恢复所有AI
            foreach (var p in redTeamPlayers) EnableAI(p, true);
            foreach (var p in blueTeamPlayers) EnableAI(p, true);
        }
        else
        {
            // 夺舍模式：跟随玩家
            Transform targetPlayer = (mode == PlayMode.RedTeam) ? redTeamPlayers[0] : blueTeamPlayers[0];
            currentControlledPlayer = targetPlayer;

            vcamSpectator.Priority = 5;     // 低优先级
            vcamPlayer.Priority = 20;       // 高优先级
            vcamPlayer.Follow = targetPlayer; // 跟随玩家
            vcamPlayer.LookAt = targetPlayer; // 锁定玩家

            // 夺舍：关闭AI，开启玩家控制
            EnablePlayerControl(targetPlayer, true);
        }
    }

    // 辅助方法：切换玩家/AI控制
    void EnablePlayerControl(Transform player, bool enable)
    {
        var playerCtrl = player.GetComponent<PlayerController>();
        var aiCtrl = player.GetComponent<AIController>();
        var agent = player.GetComponent<NavMeshAgent>();

        if (enable)
        {
            playerCtrl.enabled = true;
            aiCtrl.enabled = false;
            agent.enabled = false; // 禁用导航，避免冲突
        }
        else
        {
            playerCtrl.enabled = false;
            aiCtrl.enabled = true;
            agent.enabled = true; // 恢复AI导航
        }
    }

    void EnableAI(Transform player, bool enable)
    {
        var playerCtrl = player.GetComponent<PlayerController>();
        var aiCtrl = player.GetComponent<AIController>();
        var agent = player.GetComponent<NavMeshAgent>();

        playerCtrl.enabled = false;
        aiCtrl.enabled = enable;
        agent.enabled = enable;
    }
}
