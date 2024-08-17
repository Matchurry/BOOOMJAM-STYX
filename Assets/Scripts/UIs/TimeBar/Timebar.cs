using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timebar : MonoBehaviour
{
    private float _startTime;
    private RectTransform _rt;
    public float _timeLimit = 60f;
    private Vector2 tarpos = new Vector2(0, 463.28f);
    
    void Start()
    {
        _rt = GetComponent<RectTransform>();
        _startTime = Time.time;
        _rt.anchoredPosition = new Vector2(0, 564.28f);
    }
    
    void Update()
    {
        _rt.anchoredPosition = Vector2.Lerp(_rt.anchoredPosition, tarpos, Time.deltaTime * 2.5f);
        _rt.transform.localScale = new Vector3(
            ((Time.time-_startTime) / _timeLimit) * 19f,
            _rt.transform.localScale.y,
            _rt.transform.localScale.z);
    }
    
}
