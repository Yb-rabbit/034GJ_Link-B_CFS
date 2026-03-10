using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    [Header("设置")]
    public Transform pointA; // 起始位置
    public Transform pointB; // 目标位置
    public float speed = 2.0f; // 移动速度

    private bool isMoving = false;
    private bool goingToB = true; // true=去B点, false=回A点

    void Update()
    {
        if (isMoving)
        {
            // 确定目标位置
            Transform target = goingToB ? pointB : pointA;
            
            // 插值移动 (Vector3.MoveTowards 更平滑，也可用Lerp)
            transform.position = Vector3.MoveTowards(
                transform.position, 
                target.position, 
                speed * Time.deltaTime
            );

            // 到达检测
            if (Vector3.Distance(transform.position, target.position) < 0.01f)
            {
                isMoving = false;
                //Debug.Log("电梯已到达");
                // 可以在这里添加到达音效或开门逻辑
            }
        }
    }

    // 修改后的方法：智能判断方向
    public void StartElevator()
    {
        if (isMoving) return; // 如果正在动，就忽略点击

        // 简单的逻辑：如果现在离A近，就去B；如果离B近，就去A。
        // 这样无论初始状态如何，电梯都会去正确的一端
        float distToA = Vector3.Distance(transform.position, pointA.position);
        
        if (distToA < 0.5f) 
        {
            // 在A点，去B点
            goingToB = true;
        }
        else
        {
            // 在B点（或中间），去A点
            goingToB = false;
        }

        isMoving = true;
    }
}
