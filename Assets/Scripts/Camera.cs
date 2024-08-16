using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;

public class Camera : MonoBehaviour
{
    /// <summary>
    /// 玩家物体
    /// </summary>
    public Transform target;
    /// <summary>
    /// 摄像机与玩家的距离
    /// </summary>
    public float distance = 6f;
    /// <summary>
    /// 摄像机的高度
    /// </summary>
    public float height = 7f;
    /// <summary>
    /// 平滑插值速度
    /// </summary>
    public float smoothSpeed = 0.125f;
    /// <summary>
    /// 摄像机抖动事件持续时间
    /// </summary>
    public float shakeDuration = 0.5f;
    /// <summary>
    /// 摄像机抖动事件强度
    /// </summary>
    public float shakeIntensity = 0.2f;
    private float shakeStartTime;
    private GameObject _canvasUI;
    private RectTransform _canvasRectTransform;
    private Player ps;
    private bool _isdead = false;
    public GameObject failedUI;
    public static UnityEvent Level1Turto = new UnityEvent();
    public GameObject turtoUI;
    
    

    void Start()
    {
        ps = Player.instance;
        _canvasUI = GameObject.Find("Canvas");
        _canvasRectTransform = _canvasUI.GetComponent<RectTransform>();
        Bomb.OnBombTriggered.AddListener(StartShake);
        Cube.CubeSelfDes.AddListener(StartShake);
        Cube.OnCubeGet.AddListener(StartPushIn);
        Ballet.OnBalletHit.AddListener(StartShake);
        Level1Turto.AddListener(StartLevel1);
        
        Level1Turto.Invoke();
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.P)){
            StartShake();
        }
        if (Time.time < shakeStartTime + shakeDuration){
            float t = (Time.time - shakeStartTime) / shakeDuration;
            float shakeX = Random.Range(-1f, 1f) * shakeIntensity * (1 - t);
            float shakeY = Random.Range(-1f, 1f) * shakeIntensity * (1 - t);
            transform.position += new Vector3(shakeX, shakeY, 0);
        }
        
        
        if (!_isdead && ps.HP.size <= 0.1f) //玩家血量归零
        {
            _isdead = true;
            GameObject fail = Instantiate(failedUI);
            RectTransform failUIRect = fail.GetComponent<RectTransform>();
            failUIRect.SetParent(_canvasRectTransform);
        }
    }

    void LateUpdate(){
        //摄像机平滑跟踪
        Vector3 targetPosition = target.position + Vector3.up * height;
        Vector3 desiredPosition = targetPosition + Vector3.back * distance;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    }

    public void StartShake(){
        shakeStartTime = Time.time;
    }

    private void StartPushIn(){
        StartCoroutine(PushIn());
    }

    private bool is_in = false;
    IEnumerator PushIn(){
        if(is_in==false){
            is_in = true;
            distance = 6-1;
            height = 7-2;
            yield return new WaitForSeconds(0.5f);
            distance = 6;
            height = 7;
            is_in = false;
        }
    }

    private void StartShake(int x, int z){
        StartShake();
    }

    private void StartLevel1()
    {
        GameObject turUI = Instantiate(turtoUI);
        RectTransform turUIRect = turUI.GetComponent<RectTransform>();
        turUIRect.SetParent(_canvasRectTransform);
    }
}
