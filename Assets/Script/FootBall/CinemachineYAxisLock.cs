using UnityEngine;
using Cinemachine;

/// <summary>
/// 锁定Cinemachine摄像机Y轴的扩展脚本
/// 在Cinemachine的Body阶段处理Y轴锁定，避免影响正常Follow功能
/// </summary>
[ExecuteAlways, DisallowMultipleComponent]
public class CinemachineYAxisLock : CinemachineExtension
{
    [Tooltip("是否启用Y轴锁定")]
    public bool lockYAxis = true;
    
    [Tooltip("Y轴锁定值（留空则使用初始值）")]
    public float lockedYValue;

    private float initialYValue;
    private bool hasInitialized = false;

    protected override void Awake()
    {
        base.Awake();
        InitializeYValue();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        InitializeYValue();
    }

    private void InitializeYValue()
    {
        if (!hasInitialized && VirtualCamera != null)
        {
            // 获取当前摄像机的Y值作为初始值（如果未指定）
            if (lockedYValue == 0 && VirtualCamera.transform != null)
            {
                initialYValue = VirtualCamera.transform.position.y;
            }
            else
            {
                initialYValue = lockedYValue;
            }
            hasInitialized = true;
        }
    }

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, 
        ref CameraState state, 
        float deltaTime)
    {
        // 只在Body阶段（位置计算阶段）处理Y轴锁定
        if (stage == CinemachineCore.Stage.Body && lockYAxis)
        {
            // 获取当前摄像机位置
            Vector3 currentPosition = state.RawPosition;
            
            // 保持Y值不变，只更新X-Z平面
            currentPosition.y = initialYValue;
            
            // 更新摄像机状态（不直接修改transform，通过Cinemachine的state管理）
            state.RawPosition = currentPosition;
        }
    }

    // 编辑器中实时更新
    private void OnValidate()
    {
        if (VirtualCamera != null && !Application.isPlaying)
        {
            InitializeYValue();
        }
    }
}
