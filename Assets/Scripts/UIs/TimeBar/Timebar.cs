using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timebar : MonoBehaviour
{
    private float _startTime;
    private RectTransform _rt;
    public float _timeLimit = 60f;
    
    void Start()
    {
        _rt = GetComponent<RectTransform>();
        _startTime = Time.time;
    }
    
    void Update()
    {
        _rt.transform.localScale = new Vector3(
            ((Time.time-_startTime) / _timeLimit) * 19f,
            _rt.transform.localScale.y,
            _rt.transform.localScale.z);
    }
    
}
