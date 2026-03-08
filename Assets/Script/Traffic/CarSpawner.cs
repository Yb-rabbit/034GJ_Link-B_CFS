using UnityEngine;
using System.Collections;

public class CarSpawner : MonoBehaviour
{
    [Header("车辆预制体")]
    public GameObject[] carPrefabs;

    [Header("生成参数")]
    public float minInterval = 0.5f;
    public float maxInterval = 3f;
    
    [Header("方向设置")]
    public float spawnRotationY = 0f;

    [Header("性能控制")]
    [Tooltip("场景中允许存在的最大车辆总数")]
    public int sceneMaxCarLimit = 50; // 这里设置最大数量

    void Start()
    {
        // 将设置的限制同步给 CarAI 类的静态变量
        CarAI.MaxCarLimit = sceneMaxCarLimit;
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // 随机等待时间
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            // --- 核心逻辑：检查车辆总数 ---
            // 如果当前车辆数量已经达到上限，跳过本次生成，等待下一轮循环检查
            if (CarAI.TotalCarCount >= CarAI.MaxCarLimit)
            {
                // Debug.Log("车辆已达上限，暂停生成");
                continue; 
            }

            // 随机选择车型
            if (carPrefabs.Length > 0)
            {
                int index = Random.Range(0, carPrefabs.Length);
                Instantiate(carPrefabs[index], transform.position, Quaternion.Euler(0, spawnRotationY, 0));
            }
        }
    }
}
