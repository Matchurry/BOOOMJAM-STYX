using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timebar : MonoBehaviour
{
    private float _startTime;
    private RectTransform _rt;
    private Vector2 tarpos = new Vector2(0, 463.28f);
    private Player ps;
    private bool isStop = false;
    public static UnityEvent playerWin = new UnityEvent();
    
    void Start()
    {
        ps = Player.instance;
        _rt = GetComponent<RectTransform>();
        _rt.anchoredPosition = new Vector2(0, 564.28f);
        Camera.psDead.AddListener(Stop);
        StartCoroutine(start());
    }
    
    void Update()
    {
        if (!ps.isReading && !isStop)
        {
            _rt.anchoredPosition = Vector2.Lerp(_rt.anchoredPosition, tarpos, Time.deltaTime * 2.5f);
            _rt.transform.localScale = new Vector3(
                ((Time.time-_startTime) / ps.timeLimitToWin) * 19f,
                _rt.transform.localScale.y,
                _rt.transform.localScale.z);

            if ((Time.time - _startTime) >= ps.timeLimitToWin)
            {
                playerWin.Invoke();
                Destroy(gameObject);
            }
        }
        
    }

    IEnumerator start()
    {
        yield return new WaitUntil(() => !ps.isReading);
        _startTime = Time.time;
    }


    private void Stop()
    {
        isStop = true;
    }
}
