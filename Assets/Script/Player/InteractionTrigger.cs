using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    [Header("设置")]
    [Tooltip("这里填入你需要检测的目标对象（例如玩家）")]
    public GameObject targetObject;

    [Header("控制器引用")]
    [Tooltip("拖入玩家身上的 FirstPersonController 脚本")]
    public FirstPersonController playerController;

    // 当有物体进入触发器时调用
    private void OnTriggerEnter(Collider other)
    {
        // 检查进入的物体是否是指定目标
        if (other.gameObject == targetObject)
        {
            if (playerController != null)
            {
                // 进入区域：解锁鼠标 + 显示指针
                playerController.SetCursorState(false);
            }
        }
    }

    // 当有物体离开触发器时调用
    private void OnTriggerExit(Collider other)
    {
        // 检查离开的物体是否是指定目标
        if (other.gameObject == targetObject)
        {
            if (playerController != null)
            {
                // 离开区域：锁定鼠标 + 隐藏指针 + 恢复游戏
                playerController.SetCursorState(true);
            }
        }
    }
}