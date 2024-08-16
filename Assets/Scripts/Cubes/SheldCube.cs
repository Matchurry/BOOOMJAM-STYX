using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SheldCube : MonoBehaviour
{
    public int[] pos = new int[2];
    
    private Player ps;
    private const float CubeYValue = 1.505f;
    public bool is_moving = false;
    private Vector3 tarpos;
    public GameObject sheld; //护盾特效
    private Sheld sheldSc;

    public static UnityEvent<int, int> OnSheldDes = new UnityEvent<int, int>();
    void Start()
    {
        ps = Player.instance;
        ForPlayersNeceInit();
        
        sheld = Instantiate(sheld); //生成护盾特效
        sheld.transform.position = transform.position;
        sheldSc = sheld.GetComponent<Sheld>();
    }
    
    void Update()
    {
        if(ps.pos[0]==pos[0] && ps.pos[1]==pos[1] && !is_moving){
            transform.position = new Vector3(transform.position.x,CubeYValue-0.2f,transform.position.z);
        }
        else{
            if(is_moving)
                tarpos = ps.transform.position + Vector3.up*1.25f;
            transform.position = Vector3.Lerp(transform.position, tarpos, 0.15f);
        }
        sheldSc.tarpos = transform.position + Vector3.forward * 1f;
    }
    
    private void ForPlayersNeceInit()
    {
        ps.what_is_moving = 1;
        tarpos = new Vector3(pos[0]-512,CubeYValue,pos[1]-512);
        Player.OnCubePutOn.AddListener(HandleOnCubePutOn);
        Player.OnCubePutDown.AddListener(HandleOnCubePutDown);
        Bomb.OnBombTriggered.AddListener(HandleOnBombTriggered);
        Cube.CubeSelfDes.AddListener(HandleOnBombTriggered);
    }
    void HandleOnCubePutOn(int x, int z){
        if(x==pos[0] && z==pos[1] && ps.map[x,z,1]==1){
            //被抬起
            ps.what_is_moving = 1;
            tarpos = ps.transform.position + Vector3.up*1.25f;
            is_moving=true;
            ps.map[x,z,1]=0;
        }
    }

    void HandleOnCubePutDown(int x, int z){
        if(is_moving){
            //被放下
            ps.what_is_moving = -1;
            pos[0]=x;
            pos[1]=z;
            ps.map[x,z,1]=1;
            tarpos = new Vector3(pos[0]-512,CubeYValue,pos[1]-512);
            is_moving=false;
        }
    }
    /// <summary>
    /// 处理普通障碍物爆炸事件
    /// 如果恰好位于爆炸点则血量-1 为0是自身摧毁
    /// </summary>
    void HandleOnBombTriggered(int x, int z){
        if(!is_moving){
            if(pos[0]==x && pos[1]==z){
                OnSheldDes.Invoke(pos[0],pos[1]); //触发护盾装置消失事件
                ps.map[x, z, 1] = 0;
                Destroy(gameObject);
            }
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
