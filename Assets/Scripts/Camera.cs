using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform target; // 玩家物体
    public float distance = 50f; // 摄像机与玩家的距离
    public float height = 30f; // 摄像机的高度
    public float smoothSpeed = 0.125f; // 平滑插值速度
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        Vector3 targetPosition = target.position + Vector3.up * height; // 计算目标位置
        Vector3 desiredPosition = targetPosition + Vector3.back * distance; // 计算摄像机期望位置

        // 使用 Vector3.Lerp 进行平滑插值
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    }
}
