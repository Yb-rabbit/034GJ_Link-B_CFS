using UnityEngine;
using System.Collections;

public class SimpleFog : MonoBehaviour
{
    [Header("设置")]
    public float viewRadius = 10f;
    public string targetLayerName = "FogObject"; // 需要应用迷雾的Layer名字

    void Start()
    {
        // 程序启动时，自动给指定Layer的物体换上迷雾Shader
        ApplyFogShaderToLayer(targetLayerName);
    }

    void Update()
    {
        // 向 Shader 传递玩家位置和视野半径
        Shader.SetGlobalVector("_PlayerPos", transform.position);
        Shader.SetGlobalFloat("_ViewRadius", viewRadius);
    }

    // --- 下面是缺失的那个函数定义，请务必保留 ---

    void ApplyFogShaderToLayer(string layerName)
    {
        // 1. 找到场景里所有激活的物体
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        // 2. 获取Layer编号
        int layerIndex = LayerMask.NameToLayer(layerName);

        if (layerIndex == -1)
        {
            Debug.LogError($"Layer名字 '{layerName}' 不存在，请去Tags & Layers面板添加！");
            return;
        }

        // 3. 遍历所有物体
        foreach (GameObject obj in allObjects)
        {
            // 如果物体的Layer是我们指定的Layer
            if (obj.layer == layerIndex)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    // 获取当前的材质
                    Material currentMat = renderer.material;

                    // 创建一个新材质，使用迷雾 Shader
                    // 注意：这里假设你的Shader名字叫 "Custom/SimpleDistanceFog"
                    Material newMat = new Material(Shader.Find("Custom/SimpleDistanceFog"));

                    // 如果旧材质有主贴图，传给新材质（保留原本的样子）
                    if (currentMat.HasProperty("_MainTex"))
                    {
                        newMat.SetTexture("_MainTex", currentMat.mainTexture);
                    }

                    // 应用新材质
                    renderer.material = newMat;
                }
            }
        }
    }
}
