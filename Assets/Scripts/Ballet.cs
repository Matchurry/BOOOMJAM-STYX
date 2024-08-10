using System.Collections;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.Events;

public class Ballet : MonoBehaviour
{
    public Vector3 endPoint; // 终点
    public float maxHeight = 5f; // 抛物线最高高度
    public float duration = 1.5f; // 抛物线运动持续时间
    private float startTime;
    public static UnityEvent<int, int> OnBalletHit = new UnityEvent<int, int>();
    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        if (Time.time - startTime < duration)
        {
            var t = (Time.time - startTime) / duration;
            transform.position = CalculateParabolaPoint(t);
        }
        else
        {
            OnBalletHit.Invoke((int)endPoint.x+512,(int)endPoint.z+512);
            Destroy(gameObject);
        }
    }
    
    private Vector3 CalculateParabolaPoint(float t)
    {
        float x = Mathf.SmoothStep(transform.position.x, endPoint.x, t*0.2f);
        float z = Mathf.SmoothStep(transform.position.z, endPoint.z, t*0.2f);
        float y = Mathf.Sin(Mathf.PI * t) * maxHeight;
        return new Vector3(x, y, z);
    }
}