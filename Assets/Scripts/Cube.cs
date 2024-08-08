using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Unity.Mathematics;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Events;

public class Cube : MonoBehaviour{
    public int[] pos = new int[2];
    /// <summary>
    /// 1代表玩家 2代表漂浮 -1为未初始化
    /// </summary>
    [Tooltip("1代表玩家 2代表漂浮 -1为未初始化")]
    public int status = -1;
    [Tooltip("1普通 2加固 3炮台 -1未初始化")]
    public int type = -1;
    [Tooltip("血量")]
    public int HPs = 1;
    public int HPsLimit = 2;

    public Player ps;
    private const float CubeYValue = 0.505f;
    private bool is_moving = false;
    private Vector3 tarpos;
    private Renderer rd;
    /// <summary>
    /// 有方块飘来但是已经达到上限时触发
    /// </summary>
    public static UnityEvent<int,int> CubeSelfDes = new UnityEvent<int,int>();
    /// <summary>
    /// 有方块成功加入玩家阵营时触发
    /// </summary>
    public static UnityEvent OnCubeGet = new UnityEvent();
    /// <summary>
    /// 有方块死亡时触发，引发周围四格血量变为1
    /// </summary>
    private static UnityEvent<int,int> OnCubeDied = new UnityEvent<int,int>();
    
    void Start(){
        rd = GetComponent<Renderer>();
        switch(type){
            case 1:
                HPs = 1;
                HPsLimit = 2;
                rd.material.color = Color.white;
                break;
            case 2:
                HPs = 1;
                HPsLimit = 2;
                rd.material.color = Color.gray;
                break;
        }

        while(status==-1){;}
        ps = Player.instance;
        if(status==1){
            //为玩家阵营时
            ForPlayersNeceInit();
        }
        else{
            //为漂浮物体时

            //生成到顶端
            //transform.position = new Vector3((float)Math.Round(UnityEngine.Random.Range(-5f,5f)),CubeYValue,30f);
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
            transform.position -= Vector3.forward * 0.05f;
            UpdatePos();
            if(ps.map[pos[0],pos[1]-1]==1){
                if(ps.CubeInHand+1<=ps.CubeInHandLim){
                    ForPlayersNeceInit();
                    ps.map[pos[0],pos[1]]=1;
                    OnCubeGet.Invoke();
                    status=1;
                    ps.CubeInHand++;
                }
                else{
                    CubeSelfDes.Invoke(pos[0],pos[1]-1);
                    Destroy(gameObject);
                }
            }
        }

        if(pos[1]-512<=-20){
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
    /// <summary>
    /// 处理普通障碍物爆炸事件
    /// 如果恰好位于爆炸点则血量-1 为0是自身摧毁
    /// </summary>
    void HandleOnBombTriggered(int x, int z){
        if(status==1 && !is_moving){
            if(pos[0]==x && pos[1]==z){
                CubeHpsMinus();
            }
        }
    }
    /// <summary>
    /// 方块血量减少一
    /// 自动判断是否会变色与销毁
    /// </summary>
    private void CubeHpsMinus(){
        HPs--;
        if(HPs==HPsLimit-1)
            rd.material.color=Color.white;
        if(HPs<=0){
            ps.map[pos[0],pos[1]]=0;
            ps.CubeInHand--;
            OnCubeDied.Invoke(pos[0],pos[1]);
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// 带条件的方块血量减少 x是几都无所谓 不会减到0
    /// 重载函数
    /// </summary>
    private void CubeHpsMinus(int x)
    {
        if (HPs == 1) return;
        else
        {
            CubeHpsMinus();
        }
    }
    
    /// <summary>
    /// 方块加血
    /// 自动处理是否变色
    /// </summary>
    private void HandheldOnCubeHpPickup(int x, int z){
        if(pos[0]==x && pos[1]==z && type!=4)
        {
            if (type == 1) HPsLimit = 2;
            if (HPs + 1 >= HPsLimit) HPs = HPsLimit;
            else HPs+=1;
            if(HPs==HPsLimit)
                rd.material.color=Color.gray;
            else
                rd.material.color=Color.white;
        }
    }

    private void HandleCubeSelfDes(int x, int z){
        if(pos[0]==x && pos[1]==z)
            CubeHpsMinus();
    }

    private void HandleOnCubeDied(int x, int z)
    {
        if(Math.Abs(pos[0]-x)+Math.Abs(pos[1]-z)==1)
            CubeHpsMinus(1);
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
        Pickups.OnCubeHpPickup.AddListener(HandheldOnCubeHpPickup);
        Bomb.OnBombTriggered.AddListener(HandleOnBombTriggered);
        Cube.CubeSelfDes.AddListener(HandleCubeSelfDes);
        Cube.OnCubeDied.AddListener(HandleOnCubeDied);
        switch(type){
            case 1 :
                HPs = 1;
                HPsLimit = 2;
                rd.material.color = Color.white;
                break;
            case 2 : 
                HPs = 2;
                HPsLimit = 2;
                rd.material.color = Color.gray;
                break;
            case 4 :
                HPs = 1;
                HPsLimit = 1;
                rd.material.color = new Color(1f,0.7673f,0f);
                break;
        }
    }

    IEnumerator Wait(float t){
        yield return new WaitForSeconds(t);
    }
}
