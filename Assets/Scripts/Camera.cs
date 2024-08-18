using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Camera : MonoBehaviour
{
    /// <summary>
    /// 玩家物体
    /// </summary>
    public GameObject target;
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
    public static UnityEvent psDead = new UnityEvent();
    public GameObject winUI;
    
    

    void Start()
    {
        ps = Player.instance;
        _canvasUI = GameObject.Find("Canvas");
        _canvasRectTransform = _canvasUI.GetComponent<RectTransform>();
        Bomb.OnBombTriggered.AddListener(StartShake);
        Cube.CubeSelfDes.AddListener(StartShake);
        Cube.OnCubeGet.AddListener(StartPushIn);
        Cube.OnCoreDes.AddListener(PlayerDead);
        Ballet.OnBalletHit.AddListener(StartShake);
        lazerSign.lazerAttack.AddListener(StartShake);
        Timebar.playerWin.AddListener(WinUI);
        ContinueBt.nextLevel.AddListener(NextLevel);
        Level1Turto.AddListener(StartLevel1);
        
        if(ps.isReading)
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
            psDead.Invoke();
            ps.is_resumed = true;
            PlayerDead();
        }
    }

    void LateUpdate(){
        //摄像机平滑跟踪
        Vector3 targetPosition = target.transform.position + Vector3.up * height;
        Vector3 desiredPosition = targetPosition + Vector3.back * distance;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    }

    public void StartShake(){
        shakeStartTime = Time.time;
    }

    private void StartPushIn(){
        StartCoroutine(PushIn());
    }

    private void PlayerDead()
    {
        _isdead = true;
        GameObject fail = Instantiate(failedUI);
        RectTransform failUIRect = fail.GetComponent<RectTransform>();
        failUIRect.SetParent(_canvasRectTransform);
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

    private void WinUI()
    {
        GameObject turUI = Instantiate(winUI);
        RectTransform turUIRect = turUI.GetComponent<RectTransform>();
        turUIRect.SetParent(_canvasRectTransform);
        turUIRect.anchoredPosition = new Vector2(0, 0);
    }
    
    private void NextLevel()
    {
        if (ps.now_level == 0)
        {
            //启动D2
            SceneManager.LoadSceneAsync(2);
        }
        else if (ps.now_level == 1)
        {
            //启动D3
            SceneManager.LoadSceneAsync(3);
        }
        else if (ps.now_level == 2)
        {
            //启动D4
            SceneManager.LoadSceneAsync(4);
        }
        else if (ps.now_level == 3)
        {
            SceneManager.LoadSceneAsync(7);
        }
        else if (ps.now_level == 4)
        {
            //分支
            if (ps.choseChange == 0)
                SceneManager.LoadSceneAsync(10);
            else
                SceneManager.LoadSceneAsync(11);
        }
        else if (ps.now_level == 5)
        {
            //分支
            if (ps.choseChange == 0)
                SceneManager.LoadSceneAsync(14);
            else
                SceneManager.LoadSceneAsync(15);
        }
        else if (ps.now_level == 6)
        {
            SceneManager.LoadScene(16);
        }
    }
}
