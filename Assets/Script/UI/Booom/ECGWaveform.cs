using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer), typeof(MeshCollider))]
public class ECGWaveform : MonoBehaviour
{
    [Header("渲染设置")]
    public int totalPoints = 300;        // 波形总点数
    public float width = 10f;            // 波形总宽度
    public float height = 1f;             // 振幅
    
    [Header("动画设置")]
    public float scrollSpeed = 0.5f;      // 滚动速度
    public float frequency = 5f;          // 波形频率
    
    private LineRenderer lineRenderer;
    private Vector3[] points;             // 存储点阵
    private MeshCollider meshCollider;
    private int pointsCount;
    
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        pointsCount = totalPoints;
        lineRenderer.positionCount = pointsCount;
        points = new Vector3[pointsCount];
        GenerateInitialPoints();
    }
    
    void Update()
    {
        ScrollWaveform();
        UpdateLineRenderer();
        UpdateMeshCollider();
    }
    
    void GenerateInitialPoints()
    {
        for (int i = 0; i < pointsCount; i++)
        {
            float x = (float)i / (pointsCount - 1) * width;
            float y = CalculateWaveformY(x);
            points[i] = new Vector3(x, y, 0);
        }
    }
    
    float CalculateWaveformY(float x)
    {
        float t = x / width;
        // 应用滚动偏移
        float scrollOffset = Time.time * scrollSpeed;
        float phase = (t * Mathf.PI * 2 * frequency) + scrollOffset;
        // 生成心电图风格的波形（正弦波 + 高频杂波模拟）
        float y = Mathf.Sin(phase) * 0.3f;
        y += Mathf.Sin(phase * 3f) * 0.1f;   // 谐波
        y += Mathf.Sin(phase * 7f) * 0.05f;  // 更高频调制
        return y * height;
    }
    
    void ScrollWaveform()
    {
        // 滚动第一个点移动到末尾
        Vector3 firstPoint = points[0];
        for (int i = 0; i < pointsCount - 1; i++)
        {
            points[i] = points[i + 1];
        }
        float x = (pointsCount - 1) * (width / (pointsCount - 1));
        float newY = CalculateWaveformY(x);
        points[pointsCount - 1] = new Vector3(x, newY, 0);
    }
    
    void UpdateLineRenderer()
    {
        lineRenderer.SetPositions(points);
    }
    
    void UpdateMeshCollider()
    {
        Mesh mesh = new Mesh();
        lineRenderer.BakeMesh(mesh, Camera.main, false);
        meshCollider.sharedMesh = mesh;
    }
} 