using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using UnityEditor;
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
    [Tooltip("1普通 2加固 3炮台 4核心 5加速 -1未初始化")]
    public int type = -1;
    [Tooltip("血量")]
    public int HPs = 1;
    public int HPsLimit = 2;
    private bool _veryFirst; //用于平衡游戏速率生成
    private Vector3 _birthPos;
    private Player ps;
    private const float CubeYValue = 0.505f;
    public bool is_moving = false;
    private Vector3 tarpos;
    
    public static UnityEvent<int,int> CubeSelfDes = new UnityEvent<int,int>();
    public static UnityEvent OnCubeGet = new UnityEvent();
    public static UnityEvent OnCoreDes = new UnityEvent();
    public static UnityEvent OnSheldTri = new UnityEvent();
    
    public GameObject stardCubePrefab;
    public GameObject coreCubePrefab;
    public GameObject reinforcedCubePrefab;
    public GameObject shooterBalletPrefab;

    public static UnityEvent OnShooterDes = new UnityEvent();
    public static UnityEvent OnSpeedDes = new UnityEvent();

    private AudioController audioController;

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
            _veryFirst = true;
            _birthPos = transform.position;
        }
        audioController = FindObjectOfType<AudioController>();
    }

    void Update(){
        if(status==1){
            //为玩家阵营时
            if(ps.pos[0]==pos[0] && ps.pos[1]==pos[1] && !is_moving){
                transform.position = new Vector3(transform.position.x,CubeYValue-0.2f,transform.position.z);
            }
            else{
                if(is_moving)
                    tarpos = ps.transform.position + Vector3.up*1.85f;
                transform.position = Vector3.Lerp(transform.position, tarpos, 0.15f);
            }
        }
        else{
            //为漂浮物体时

            //进行漂浮移动
            if(!ps.is_resumed)
                transform.position -= Vector3.forward * (0.05f * ps.gameSpeed); // 乘上游戏速率以平衡`
            UpdatePos();
            if(ps.map[pos[0],pos[1]-1,0]==1){
                if(ps.CubeInHand+1<=ps.CubeInHandLim){
                    ForPlayersNeceInit();
                    ps.map[pos[0],pos[1],0]=1;
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

        if (ps.isShooterSkill && type == 3 && Input.GetKeyDown(KeyCode.Space))
        {
            GameObject sht = GameObject.Find("ShooterIcon");
            ShooterIcon shtSc = sht.GetComponent<ShooterIcon>();
            if (shtSc.coolDown <= 0.1f)
            {
                shtSc.coolDown = 5f;
                GameObject bt = Instantiate(shooterBalletPrefab);
                ShooterBalletSc sc = bt.GetComponent<ShooterBalletSc>();
                //召唤后给予初始位置即可 索敌在其脚本中
                bt.transform.position = transform.position;
                sc.pos = pos;
            }
        }

        if (_veryFirst && _birthPos.z - transform.position.z >= 1)
        {
            _veryFirst = false;
            ps.next_summon = true;
        }
    }

    void HandleOnCubePutOn(int x, int z){
        if(status==1 && x==pos[0] && z==pos[1] && ps.map[x,z,1]!=1){
            //被抬起
            ps.what_is_moving = 0;
            ps.map[x,z,0]=0;
            tarpos = ps.transform.position + Vector3.up*1.85f;
            is_moving=true;
        }
    }

    void HandleOnCubePutDown(int x, int z){
        if(status==1 && is_moving){
            //被放下
            ps.what_is_moving = -1;
            pos[0]=x;
            pos[1]=z;
            ps.map[x,z,0]=1;
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
            if(ps.map[x,z,1]!=1 && pos[0]==x && pos[1]==z){
                if (ps._protectedCol == x)
                {
                    OnSheldTri.Invoke();
                    return;
                }
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
        audioController.PlaySound("hitted");
        if (HPs==HPsLimit-1 && type!=3 && type!=4 && type!=5)
            SwitchToType(1);
        if(HPs<=0){
            ps.map[pos[0],pos[1],0]=0;
            ps.CubeInHand--;
            if (type == 3)
                OnShooterDes.Invoke();
            if (type == 5) 
                OnSpeedDes.Invoke();
            if (type == 4)
                OnCoreDes.Invoke();
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// 方块加血
    /// 自动处理是否变色
    /// </summary>
    private void HandheldOnCubeHpPickup(int x, int z){
        if(pos[0]==x && pos[1]==z && type!=4 && type!=3){
            if(HPs+1>=HPsLimit) HPs=HPsLimit;
            else HPs+=1;
            if(HPs==HPsLimit)
                SwitchToType(2);
            else
                SwitchToType(1);
        }
    }

    /*private void HandleCubeSelfDes(int x, int z){
        if(ps.map[x,z,1]!=1 && pos[0]==x && pos[1]==z)
            CubeHpsMinus();
    }*/

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
    private void ForPlayersNeceInit()
    {
        _veryFirst = false;
        tarpos = new Vector3(pos[0]-512,CubeYValue,pos[1]-512);
        Player.OnCubePutOn.AddListener(HandleOnCubePutOn);
        Player.OnCubePutDown.AddListener(HandleOnCubePutDown);
        Pickups.OnCubeHpPickup.AddListener(HandheldOnCubeHpPickup);
        Bomb.OnBombTriggered.AddListener(HandleOnBombTriggered);
        Cube.CubeSelfDes.AddListener(HandleOnBombTriggered);
        switch(type){
            case 1 :
                HPs = 1;
                HPsLimit = 2;
                break;
            case 2 : 
                HPs = 2;
                HPsLimit = 2;
                break;
            case 3:
                HPs = 1;
                HPsLimit = 1;
                break;
            case 4 :
                HPs = 1;
                HPsLimit = 1;
                break;
            case 5 :
                HPs = 3;
                HPsLimit = 3;
                break;
        }
    }
    /// <summary>
    /// 重要*
    /// 方块改变自身模型时调用（例如因血量变更导致的模型变化
    /// 调试中
    /// </summary>
    /// <param name="x">1标准 2加固 3炮台 4核心</param>
    private void SwitchToType(int x)
    {
        //生成一个新的方块 替换自身 只改变type 随后自身删除
        switch (x)
        {
            case 1:
                GameObject standardCube = Instantiate(stardCubePrefab);
                Cube standSc = standardCube.GetComponent<Cube>();
                standardCube.transform.position = transform.position;
                standSc.pos = pos;
                standSc.status = status;
                standSc.type = 1;
                break;
            case 2:
                GameObject reinCube = Instantiate(reinforcedCubePrefab);
                Cube reinSc = reinCube.GetComponent<Cube>();
                reinCube.transform.position = transform.position;
                reinSc.pos = pos;
                reinSc.status = status;
                reinSc.type = 2;
                break;
            default:
                break;
        }
        Destroy(gameObject);
    }
    
    IEnumerator Wait(float t){
        yield return new WaitForSeconds(t);
    }
}
