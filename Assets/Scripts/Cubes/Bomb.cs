using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bomb : MonoBehaviour{
    public int[] pos = new int[2];
    private const float CubeYValue = 0.505f;
    public int type = -1; //0普通障碍物 1攻击型 -1未初始化
    private Player ps;
    public static UnityEvent<int, int> OnBombTriggered = new UnityEvent<int, int>();
    private Renderer rd;
    private bool _veryFirst = true; //用于平衡游戏速率生成
    private Vector3 _birthPos;
    public GameObject warnSignPrefab;
    public GameObject balletPrefab;

    void Start(){
        rd = GetComponent<Renderer>();
        ps = Player.instance;
        _birthPos = transform.position;
        if (type == 2) 
            StartCoroutine(Attack()); //启动攻击型障碍物攻击携程
    }

    void Update(){
        if(!ps.is_resumed)
            transform.position -= Vector3.forward * (0.05f * ps.gameSpeed);
        UpdatePos();
        if(ps.map[pos[0],pos[1]-1,0]==1){
            OnBombTriggered.Invoke(pos[0],pos[1]-1);
            Destroy(gameObject);
        }

        if(pos[1]-512<=-20){
            Destroy(gameObject);
        }
        
        if (_veryFirst && _birthPos.z - transform.position.z >= 1)
        {
            _veryFirst = false;
            ps.next_summon = true;
        }
    }

    /// <summary>
    /// 由地图位置更新到pos数组位置
    /// </summary>
    private void UpdatePos(){
        pos[0]=TransToPos(transform.position.x);
        pos[1]=TransToPos(transform.position.z);
    }

    /// <summary>
    /// 从元素的位置信息转换到地图块的数组位置信息
    /// </summary>
    /// <param name="x">元素的某方向位置</param>
    private int TransToPos(float x){
        return (int)Math.Round(x+512);
    }
    /// <summary>
    /// 攻击型障碍物攻击携程
    /// </summary>
    IEnumerator Attack() {
        while(true){
           //等待约10-15秒攻击一次
           yield return new WaitForSeconds(UnityEngine.Random.Range(5, 10));
           if (pos[1] <= ps.pos[1]) //在玩家后方时不再攻击
               yield break;
           if(pos[1] >= ps.pos[1]+20) //在玩家太前面不攻击
               continue;
           //发动攻击 
           GameObject warn = Instantiate(warnSignPrefab);
           warn.transform.position = new Vector3(ps.pos[0]-512, 10.00f, ps.pos[1]-512);
           GameObject ba = Instantiate(balletPrefab);
           ba.transform.position = transform.position;
           Ballet blSc = ba.GetComponent<Ballet>();
           blSc.endPoint = new Vector3(ps.pos[0]-512, 10.00f, ps.pos[1]-512);
        }
    }
}
