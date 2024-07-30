using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Cube : MonoBehaviour
{
    public int[] pos = new int[2];
    /// <summary>
    /// 1代表玩家 2代表漂浮 -1为未初始化
    /// </summary>
    [Tooltip("1代表玩家 2代表漂浮 -1为未初始化")]
    public int status = -1;
    public Player ps;
    private const float CubeYValue = 0.505f;
    private bool is_moving = false;
    private Vector3 tarpos;

    void Start(){
        while(status==-1){;}
        ps = Player.instance;
        if(status==1){
            //为玩家阵营时
            ForPlayersNeceInit();
        }
        else{
            //为漂浮物体时

            //生成到顶端
            transform.position = new Vector3((float)Math.Round(UnityEngine.Random.Range(-5f,5f)),CubeYValue,30f);
        }
    }

    void Update(){
        if(status==1){
            //为玩家阵营时
            if(ps.pos[0]==pos[0] && ps.pos[1]==pos[1] && !is_moving){
                transform.position = new Vector3(transform.position.x,CubeYValue-0.2f,transform.position.z);
            }
            else{
                if(is_moving)
                    tarpos = ps.transform.position + Vector3.up*1.25f;
                transform.position = Vector3.Lerp(transform.position, tarpos, 0.1f);
            }
        }
        else{
            //为漂浮物体时

            //进行漂浮移动
            transform.position -= Vector3.forward * 0.01f;
            UpdatePos();
            if(ps.map[pos[0],pos[1]-1]==1){
                ForPlayersNeceInit();
                ps.map[pos[0],pos[1]]=1;
                status=1;
            }
        }

        if(pos[1]<=-20){
            Destroy(gameObject);
        }
    }

    void HandleOnCubePutOn(int x, int z){
        if(status==1 && x==pos[0] && z==pos[1]){
            //被抬起
            tarpos = ps.transform.position + Vector3.up*1.25f;
            is_moving=true;
        }
    }

    void HandleOnCubePutDown(int x, int z){
        if(status==1 && is_moving){
            //被放下
            pos[0]=x;
            pos[1]=z;
            tarpos = new Vector3(pos[0]-512,CubeYValue,pos[1]-512);
            is_moving=false;
        }
    }

    void HandleOnBombTriggered(int x, int z){
        if(status==1 && !is_moving){
            //自身位于爆炸点3x3范围内
            if(Math.Abs(pos[0]-x)+Math.Abs(pos[1]-z)<=2 && Math.Abs(pos[0]-x)<=1 && Math.Abs(pos[1]-z)<=1){
                ps.map[pos[0],pos[1]]=0;
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

    /// <summary>
    /// 作为玩家可支配的方块完成创建或身份转变时，需要使用此函数
    /// </summary>
    private void ForPlayersNeceInit(){
        tarpos = new Vector3(pos[0]-512,CubeYValue,pos[1]-512);
        Player.OnCubePutOn.AddListener(HandleOnCubePutOn);
        Player.OnCubePutDown.AddListener(HandleOnCubePutDown);
        Bomb.OnBombTriggered.AddListener(HandleOnBombTriggered);
    }

    IEnumerator Wait(float t){
        yield return new WaitForSeconds(t);
    }
}
