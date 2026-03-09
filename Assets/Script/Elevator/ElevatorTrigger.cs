using UnityEngine;

public class ElevatorTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 检测是否是玩家（确保Player物体挂了Collider且Tag设为Player）
        if (other.CompareTag("Player"))
        {
            // 将玩家设为电梯的子物体，玩家会跟随电梯移动
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 玩家离开电梯平台，取消父子关系
            other.transform.SetParent(null);
        }
    }
}
