using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pos : MonoBehaviour
{
    public float 抖动范围 = 1f;
    public float 抖动频率 = 1f; // 抖动频率，以秒为单位
    public float 抖动速度 = 1f; // 抖动速度
    public float 放缩范围 = 0.2f; // 放缩范围
    public float 放缩频率 = 1f; // 放缩频率，以秒为单位
    public float 放缩速度 = 1f; // 放缩速度

    private Vector2 initialPosition;
    private Vector2 targetPosition;
    private Vector3 initialScale;
    private Vector3 targetScale;
    private float positionTimer;
    private float scaleTimer;

    void Start()
    {
        // 记录初始位置和缩放
        initialPosition = transform.position;
        targetPosition = initialPosition;
        initialScale = transform.localScale;
        targetScale = initialScale;
        positionTimer = 0f;
        scaleTimer = 0f;
    }

    void Update()
    {
        // 更新计时器
        positionTimer += Time.deltaTime;
        scaleTimer += Time.deltaTime;

        // 当计时器达到抖动频率时，计算新的目标位置并重置计时器
        if (positionTimer >= 抖动频率)
        {
            // 在抖动范围内计算新的目标位置
            float newX = initialPosition.x + Random.Range(-抖动范围, 抖动范围);
            float newY = initialPosition.y + Random.Range(-抖动范围, 抖动范围);

            // 限制目标位置在初始范围内
            newX = Mathf.Clamp(newX, initialPosition.x - 抖动范围, initialPosition.x + 抖动范围);
            newY = Mathf.Clamp(newY, initialPosition.y - 抖动范围, initialPosition.y + 抖动范围);

            targetPosition = new Vector2(newX, newY);

            // 重置计时器
            positionTimer = 0f;
        }

        // 当计时器达到放缩频率时，计算新的目标缩放并重置计时器
        if (scaleTimer >= 放缩频率)
        {
            // 在放缩范围内计算新的目标缩放
            float newScale = initialScale.x + Random.Range(-放缩范围, 放缩范围);

            // 限制目标缩放在初始范围内
            newScale = Mathf.Clamp(newScale, initialScale.x - 放缩范围, initialScale.x + 放缩范围);

            targetScale = new Vector3(newScale, newScale, initialScale.z);

            // 重置计时器
            scaleTimer = 0f;
        }

        // 使用 Lerp 方法平滑地移动到目标位置
        transform.position = Vector2.Lerp(transform.position, targetPosition, Time.deltaTime * 抖动速度);

        // 使用 Lerp 方法平滑地缩放到目标缩放
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 放缩速度);
    }
}



