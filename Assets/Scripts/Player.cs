using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO.Compression;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

public class Player : MonoBehaviour{
    public static Player instance; // 静态实例变量
    public GameObject cubePrefab;
    public GameObject bombPrefab;
    public GameObject pickupPrefab;
    private const float CubeYValue = 0.505f;
    public float speed = 3f;
    private Vector3 movement;
    public int[,] map = new int[1024,1024];
    public int[] pos = new int[2];
    public static UnityEvent<int, int> OnCubePutOn = new UnityEvent<int, int>();
    public static UnityEvent<int, int> OnCubePutDown = new UnityEvent<int, int>();
    private bool IsCubeOn = false;
    private float startTime=0f;
    public Scrollbar HP;
    public int Score = 0;
    /// <summary>
    /// 当前属于玩家阵营的方块数
    /// </summary>
    public int CubeInHand = 0;
    /// <summary>
    /// 玩家可以持有的方块上限
    /// 大于此上限时不会有新的方块加入
    /// </summary>
    public int CubeInHandLim = 9;

    void Awake(){
        instance = this;
    }

    void Start(){
        Application.targetFrameRate = 90;
        Bomb.OnBombTriggered.AddListener(GetBomb);
        StartCoroutine(RunSummon(5,5,8,35));
        StartCoroutine(RunDelayedLoop());
    }
    /// <summary>
    /// 开始顶端物品生成协程
    /// 请注意参数都是1-100的，四个之和不要大于100，第五个是留给空位的
    /// </summary>
    /// <param name="EnemyPosibility">障碍物生成的概率</param>
    /// <param name="PlayerCubePosibility">玩家方块生成的概率</param>
    /// <param name="PickupsPosibility">捡拾物生成的概率</param>
    /// <param name="FloatThingsPosibility">漂浮动画的生成概率</param>
    /// <returns></returns>
    IEnumerator RunSummon(int EnemyPosibility, int PlayerCubePosibility, int PickupsPosibility, int FloatThingsPosibility){
        PlayerCubePosibility+=EnemyPosibility;
        PickupsPosibility+=PlayerCubePosibility;
        FloatThingsPosibility+=PickupsPosibility;
        while(true){
            for(int i=-10; i<=10; i++){
                var pos = UnityEngine.Random.Range(1,100+1);
                if(pos<=EnemyPosibility){
                    //生成障碍物
                    SummonBomb(i);
                }
                else if(pos<=PlayerCubePosibility){
                    //生成玩家方块
                    SummonCube(i);
                }
                else if(pos<=PickupsPosibility){
                    //生成捡拾物
                    SummonPickups(i);
                }
                else if(pos<=FloatThingsPosibility){
                    //生成漂浮物动画
                }
            }
            yield return new WaitForSeconds(0.7f);
        }
    }

    IEnumerator RunDelayedLoop(){
        for(int i=-1; i<=1; i++)
            for(int j=-1; j<=1; j++){
                if(map[i+512,j+512]==0){
                    HP.size=1;
                    PutCube(i,j);
                    yield return new WaitForSeconds(0.1f);
                }
            }
    }

    void Update(){

        //更新玩家map位置
        pos[0]=TransToPos(transform.position.x);
        pos[1]=TransToPos(transform.position.z);

        //玩家移动
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector3 movement = new Vector3(horizontalInput>0?1:-1, 0f, verticalInput>0?1:-1);
        
        if(horizontalInput==0) movement.x=0; 
        if(verticalInput==0) movement.z=0;
        var tarpos = transform.position + movement * (speed * Time.deltaTime);
        if(map[TransToPos(tarpos.x),TransToPos(tarpos.z)]==1 || map[pos[0],pos[1]]!=1)
            transform.position=tarpos;

        //抬起Cube
        if (Input.GetKeyDown(KeyCode.Mouse0) && !IsCubeOn && Can_PutUp()){
            //PutCube();
            startTime = Time.time;
            int x = TransToPos(AimPosNow().x);
            int z = TransToPos(AimPosNow().z);
            if(map[x,z]==1) OnCubePutOn.Invoke(x,z);
            IsCubeOn = true;
            map[x,z]=0;
        }

        //放下Cube
        if(Input.GetKeyDown(KeyCode.Mouse0) && IsCubeOn && Can_PutDown() && Time.time-startTime>=0.1f){
            int x = TransToPos(AimPosNow().x);
            int z = TransToPos(AimPosNow().z);
            OnCubePutDown.Invoke(x,z);
            IsCubeOn=false;
            map[x,z]=1;
        }

        if(map[pos[0],pos[1]]!=1){
            speed = 5f;
            HP.size -= 0.0005f;
            var rd = GetComponent<Renderer>();
            rd.material.color=new Color(0.9811f,0.3656f,0.3878f);
        }
        else{
            speed = 3f;
            var rd = GetComponent<Renderer>();
            rd.material.color=new Color(0.6745f,0.8747f,1);
        }
    }
    /// <summary>
    /// 生成一个新的方块，输入位置信息，直接生成。
    /// 仅用于关卡开始的地图生成，请勿在游戏中使用此函数
    /// </summary>
    private void PutCube(int x, int y){
        GameObject cube = Instantiate(cubePrefab);
        Cube cubeScript = cube.GetComponent<Cube>();
        cube.transform.position = new Vector3(x,CubeYValue+10,y);
        cubeScript.pos[0] = TransToPos(x);
        cubeScript.pos[1] = TransToPos(y);
        cube.transform.localScale = new Vector3(1, 1, 1);
        map[x+512,y+512] = 1;
        cubeScript.status = 1;
        if(x==0 && y==-1) cubeScript.type = 4;
        else cubeScript.type = 2;
        CubeInHand++;
    }
    /// <summary>
    /// 从元素的位置信息转换到地图块的数组位置信息
    /// </summary>
    /// <param name="x">元素的某方向位置</param>
    private int TransToPos(float x){
        return (int)Math.Round(x+512);
    }

    /// <summary>
    /// 生成新的漂浮方块以供玩家使用
    /// </summary>
    private void SummonCube(int i){
        GameObject cube = Instantiate(cubePrefab);
        Cube cubeScript = cube.GetComponent<Cube>();
        cube.transform.position = new Vector3(i,CubeYValue,30f);
        cube.transform.localScale = new Vector3(1, 1, 1);
        cubeScript.status = 2;
        var pos = UnityEngine.Random.Range(1,100+1);
        if(pos<= 13)
            cubeScript.type = 2; //其实是3 炮塔
        else if(pos <= 13+25)
            cubeScript.type = 2;
        else 
            cubeScript.type = 1;
    }
    /// <summary>
    /// 生成炸弹
    /// </summary>
    private void SummonBomb(int i){
        GameObject bomb = Instantiate(bombPrefab);
        bomb.transform.position = new Vector3(i,CubeYValue,30f);
        //这里是炸弹类型的随机
    }

    private void SummonPickups(int i){
        GameObject pickup = Instantiate(pickupPrefab);
        Pickups puSc = pickup.GetComponent<Pickups>();
        pickup.transform.position = new Vector3(i,CubeYValue,30f);
        var pos = UnityEngine.Random.Range(1,100+1);
        if(pos <= 13)
            puSc.type = 3;
        else if(pos <= 13+25)
            puSc.type = 1;
        else
            puSc.type = 2;
    }

    /// <summary>
    /// 输入玩家位置信息和方向，返回目标方块位置的三维坐标
    /// 注意此函数还会直接操作地图数据
    /// </summary>
    private Vector3 PutDownPos(int x, int z, int _dir){
        var playerPos = (Vector2)UnityEngine.Camera.main.WorldToScreenPoint(transform.position);
        var direction = (Vector2)Input.mousePosition - playerPos;
        var angleRadians = Mathf.Atan2(direction.y, direction.x); 
        var angleDegrees = angleRadians * Mathf.Rad2Deg;
        if(angleDegrees is <= 45 and >= -45 && map[x+1,z]==0){
            map[x+1,z]=1;
            return new Vector3(x+1-512,CubeYValue,z-512);
        }
        else if(angleDegrees is <= -135 or >= 135 && map[x-1,z]==0){
            map[x-1,z]=1;
            return new Vector3(x-1-512,CubeYValue,z-512);
        }
        else if(angleDegrees is <= 135 and >= 45 && map[x,z+1]==0) {
            map[x,z+1]=1;
            return new Vector3(x-512,CubeYValue,z+1-512);
        }
        else{
            map[x,z-1]=1;
            return new Vector3(x-512,CubeYValue,z-1-512);
        }
    }
    /// <summary>
    /// 返回指示表所指向的方块位置
    /// </summary>
    /// <returns></returns>
    public Vector3 AimPosNow(){
        var playerPos = (Vector2)UnityEngine.Camera.main.WorldToScreenPoint(transform.position);
        var direction = (Vector2)Input.mousePosition - playerPos;
        var angleRadians = Mathf.Atan2(direction.y, direction.x); 
        var angleDegrees = angleRadians * Mathf.Rad2Deg;
        if(angleDegrees is <= 45 and >= -45) return new Vector3(pos[0]-512+1,CubeYValue,pos[1]-512); //右
        else if(angleDegrees is <= -135 or >= 135) return new Vector3(pos[0]-512-1,CubeYValue,pos[1]-512); //左
        else if(angleDegrees is <= 135 and >= 45) return new Vector3(pos[0]-512,CubeYValue,pos[1]-512+1); //上
        else return new Vector3(pos[0]-512,CubeYValue,pos[1]-512-1); //下
    }

    private bool Can_PutDown()
    {
        return map[TransToPos(AimPosNow().x),TransToPos(AimPosNow().z)] != 1;
    }
    /// <summary>
    /// 障碍物爆炸事件
    /// </summary>
    private void GetBomb(int x,int z){
        if(pos[0]==x && pos[1]==z){
            HP.size -= 0.2f;
        }
    }

    private bool Can_PutUp(){
        if(map[TransToPos(AimPosNow().x),TransToPos(AimPosNow().z)]==1) return true;
        else return false;
    }
}
