using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxCame : MonoBehaviour
{
    public Transform skyboxCamera; // 辅助摄像机
    public float rotationSpeed = 10.0f; // 旋转速度
    public Material[] skyboxMaterials; // 将天空盒材质数组赋值

    void Update()
    {
        skyboxCamera.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        if (Input.GetMouseButtonDown(0)) // 检测鼠标左键按下
        {
            int randomIndex = Random.Range(0, skyboxMaterials.Length);
            RenderSettings.skybox = skyboxMaterials[randomIndex];
        }
    }
}
