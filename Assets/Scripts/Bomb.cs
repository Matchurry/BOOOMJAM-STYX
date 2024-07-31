using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bomb : MonoBehaviour{
    public int[] pos = new int[2];
    private const float CubeYValue = 0.505f;
    public Player ps;
    public static UnityEvent<int, int> OnBombTriggered = new UnityEvent<int, int>();

    void Start(){
        ps = Player.instance;
        //transform.position = new Vector3((float)Math.Round(UnityEngine.Random.Range(-5f,5f)),CubeYValue,30f);
    }

    void Update(){
        transform.position -= Vector3.forward * 0.05f;
        UpdatePos();
        if(ps.map[pos[0],pos[1]-1]==1){
            OnBombTriggered.Invoke(pos[0],pos[1]-1);
            Destroy(gameObject);
        }

        if(pos[1]-512<=-20){
            Destroy(gameObject);
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
}
