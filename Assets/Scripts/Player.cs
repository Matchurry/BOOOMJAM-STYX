using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO.Compression;
using System.Threading;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using WaitUntil = UnityEngine.WaitUntil;

public class Player : MonoBehaviour{
    public static Player instance; // 静态实例变量
    public GameObject stardCubePrefab;
    public GameObject coreCubePrefab;
    public GameObject reinforcedCubePrefab;
    public GameObject pickupCrossPrefab;
    public GameObject pickupHeartPrefab;
    public GameObject pickupPointPrefab;
    public GameObject backGround1Prefab;
    public GameObject backGround2Prefab;
    public GameObject backGround3_1Prefab;
    public GameObject backGround3_2Prefab;
    public GameObject backGround3_3Prefab;
    public GameObject rock1Prefab;
    public GameObject rock2Prefab;
    public GameObject rockCubePrefab;
    private int[] bgpos = new int[3]; //地图场景常数数据 位置初始化 代表y向间距
    private GameObject[] bgPrefabs = new GameObject[3]; //地图场景预制体集合
    private GameObject[] bg3Pregabs = new GameObject[3]; //第三关背景预制体集合
    private const float CubeYValue = 0.505f;
    public float speed = 3f;
    public float gameSpeed = 1f; //游戏速度 影响物品生成速度和场景移动速度
    public bool is_resumed = false; //加速方块的时停触发效果
    public int what_is_moving = -1; //-1未指定 0方块 1装置
    public Animator animator;
    public Vector3 move;

    public int _protectedCol = 999;
    
    // 用于平衡游戏速率
    public bool next_bg = false;
    public bool next_summon = false;
    
    private Vector3 movement;
    public int[,,] map = new int[1024,1024,2];
    public int[] pos = new int[2];
    public static UnityEvent<int, int> OnCubePutOn = new UnityEvent<int, int>();
    public static UnityEvent<int, int> OnCubePutDown = new UnityEvent<int, int>();
    public bool IsCubeOn = false;
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

    public int level = 1;

    void Awake(){
        instance = this;
        level = 3;
        bgpos[0] = 8; bgPrefabs[0] = backGround1Prefab;
        bgpos[1] = 16; bgPrefabs[1] = backGround2Prefab;
        bgpos[2] = 16;
        bg3Pregabs[0] = backGround3_1Prefab;
        bg3Pregabs[1] = backGround3_2Prefab;
        bg3Pregabs[2] = backGround3_3Prefab;
    }

    void Start(){
        Application.targetFrameRate = 90;
        Bomb.OnBombTriggered.AddListener(GetBomb);
        Ballet.OnBalletHit.AddListener(GetBallet);
        StartCoroutine(RunSummon(1,1,1,35));
        StartCoroutine(RunDelayedLoop());
        StartCoroutine(RunBackGroundSummon(level-1));
        animator = GetComponent<Animator>();

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
        bool _forthefirstsummon = true;
        while(true)
        {
            bool _summoned = false;
            for(int i=-10; i<=10; i++){
                var pos = UnityEngine.Random.Range(1,100+1);
                yield return new WaitUntil(() => !is_resumed);
                if(pos<=EnemyPosibility){
                    //生成障碍物
                    SummonBomb(i);
                    _summoned = true;
                }
                else if(pos<=PlayerCubePosibility){
                    //生成玩家方块
                    SummonCube(i);
                    _summoned = true;
                }
                else if(pos<=PickupsPosibility){
                    //生成捡拾物
                    SummonPickups(i);
                    _summoned = true;
                }
                else if(pos<=FloatThingsPosibility){
                    //生成漂浮物动画
                }
            }

            if (_forthefirstsummon && !_summoned) //避免首次不生成导致后续不生成
            {
                //Debug.Log("find the none summon");
                next_summon = true;
            }

            if (_forthefirstsummon && _summoned)
            {
                //Debug.Log("Success");
                _forthefirstsummon = false;
            }
            
            yield return new WaitUntil(() => next_summon);
;        }
    }

    IEnumerator RunDelayedLoop(){
        for(int i=-1; i<=1; i++)
            for(int j=-1; j<=1; j++){
                if(map[i+512,j+512,0]==0){
                    HP.size=1;
                    PutCube(i,j);
                    yield return new WaitForSeconds(0.1f);
                }
            }
    }
    /// <summary>
    /// 生成关卡地图场景
    /// </summary>
    /// <param name="x">当前关卡</param>
    /// <returns></returns>
    IEnumerator RunBackGroundSummon(int x)
    {
        //16.7
        //提前召唤一些加载
        if (x != 2)
        {
            for (int i = 1; i <= 5; i++)
            {
                GameObject bg1 = Instantiate(bgPrefabs[x]);
                GameObject bg2 = Instantiate(bgPrefabs[x]);
                bg1.transform.position = new Vector3(bgpos[x], 0.3f, 50-16.7f*i);
                bg2.transform.position = new Vector3(-bgpos[x], 0.3f, 50-16.7f*i);
                bg2.transform.rotation = new Quaternion(0, 1, 0,0);
            }
            while (true)
            {
                next_bg = false;
                GameObject bg1 = Instantiate(bgPrefabs[x]);
                GameObject bg2 = Instantiate(bgPrefabs[x]);
                bg1.transform.position = new Vector3(bgpos[x], 0.3f, 50);
                bg2.transform.position = new Vector3(-bgpos[x], 0.3f, 50);
                bg2.transform.rotation = new Quaternion(0, 1, 0,0);
                yield return new WaitUntil(() => next_bg);
                yield return new WaitForSeconds(0.1f); //防止多生成
            }
        }
        else
        {
            for (int i = 1; i <= 5; i++)
            {
                GameObject bg1 = Instantiate(bg3Pregabs[(i-1)%3]);
                GameObject bg2 = Instantiate(bg3Pregabs[(i-1)%3]);
                bg1.transform.position = new Vector3(bgpos[x], 0.3f, 50-16.7f*i);
                bg2.transform.position = new Vector3(-bgpos[x], 0.3f, 50-16.7f*i);
                bg2.transform.rotation = new Quaternion(0, 1, 0,0);
            }
            int a = 0;
            while (true)
            {
                if (a == 3) a = 0;
                next_bg = false;
                GameObject bg1 = Instantiate(bg3Pregabs[a]);
                GameObject bg2 = Instantiate(bg3Pregabs[a]);
                bg1.transform.position = new Vector3(bgpos[x], 0.3f, 50);
                bg2.transform.position = new Vector3(-bgpos[x], 0.3f, 50);
                bg2.transform.rotation = new Quaternion(0, 1, 0,0);
                a++;
                yield return new WaitUntil(() => next_bg);
                yield return new WaitForSeconds(0.1f); //防止多生成
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

        move = new Vector3(horizontalInput, 0, verticalInput);
        transform.LookAt(transform.position + new Vector3(horizontalInput, 0, verticalInput));


        if (horizontalInput==0) movement.x=0; 
        if(verticalInput==0) movement.z=0;
        var tarpos = transform.position + movement * (speed * Time.deltaTime);
        if((map[TransToPos(tarpos.x),TransToPos(tarpos.z),0]==1 || map[pos[0],pos[1],0]!=1)
           && (map[TransToPos(tarpos.x),TransToPos(tarpos.z),1]!=1))
            transform.position=tarpos;

        //抬起Cube
        if (Input.GetKeyDown(KeyCode.Mouse0) && !IsCubeOn && Can_PutUp()){
            //PutCube();
            startTime = Time.time;
            int x = TransToPos(AimPosNow().x);
            int z = TransToPos(AimPosNow().z);
            OnCubePutOn.Invoke(x,z);
            IsCubeOn = true;
            
        }

        //放下Cube
        if(Input.GetKeyDown(KeyCode.Mouse0) && IsCubeOn && Can_PutDown() && Time.time-startTime>=0.1f){
            int x = TransToPos(AimPosNow().x);
            int z = TransToPos(AimPosNow().z);
            OnCubePutDown.Invoke(x,z);
            IsCubeOn=false;
        }

        UpdateAnimator();


        if (map[pos[0],pos[1],0]!=1){
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
        if (x == 0 && y == -1)
        {
            GameObject cube = Instantiate(coreCubePrefab);
            Cube cubeSc = cube.GetComponent<Cube>();
            cube.transform.position = new Vector3(x,CubeYValue+10,y);
            cubeSc.pos[0] = TransToPos(x);
            cubeSc.pos[1] = TransToPos(y);
            cubeSc.status = 1;
            cubeSc.type = 4;
        }
        else
        {
           GameObject cube = Instantiate(stardCubePrefab);
           Cube cubeScript = cube.GetComponent<Cube>();
           cube.transform.position = new Vector3(x,CubeYValue+10,y);
           cubeScript.pos[0] = TransToPos(x);
           cubeScript.pos[1] = TransToPos(y);
           cubeScript.status = 1;
           cubeScript.type = 1; //开局生成普通方块
        }
        map[x+512,y+512,0] = 1;
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
        var pos = UnityEngine.Random.Range(1,100+1);
        //if(pos<= 13)
        //cubeScript.type = 2; //其实是3 炮塔 炮塔已经是主动技能
        if (pos <= 13 + 25) {
            GameObject reinforcedCube = Instantiate(reinforcedCubePrefab);
            Cube cubeSc = reinforcedCube.GetComponent<Cube>();
            reinforcedCube.transform.position = new Vector3(i,CubeYValue,45f);
            cubeSc.type = 2;
            cubeSc.status = 2;
        }
        else
        {
            GameObject standardCube = Instantiate(stardCubePrefab);
            Cube cubeSc = standardCube.GetComponent<Cube>();
            standardCube.transform.position = new Vector3(i,CubeYValue,45f);
            cubeSc.type = 1;
            cubeSc.status = 2;
        }
    }
    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// 生成炸弹
    /// </summary>
    private void SummonBomb(int i)
    {
        var pos = UnityEngine.Random.Range(1, 3 + 1);
        GameObject bomb = null;
        switch (pos)
        {
            case 1:
                bomb = Instantiate(rock1Prefab);
                break;
            case 2:
                bomb = Instantiate(rock2Prefab);
                break;
            case 3:
                bomb = Instantiate(rockCubePrefab);
                break;
        }
        Bomb bbSc = bomb.GetComponent<Bomb>();
        bomb.transform.position = new Vector3(i,CubeYValue,45f);
        bomb.transform.Rotate(Vector3.forward, Random.Range(0,360));
        //这里是炸弹类型的随机 30%攻击型吧?
        pos = UnityEngine.Random.Range(1, 100 + 1);
        if (pos <= 30)
            bbSc.type = 2; //攻击型
        else
            bbSc.type = 1; //冲撞型
    }

    private void SummonPickups(int i)
    {
        GameObject pickup = null;
        Pickups puSc = null;
        var pos = UnityEngine.Random.Range(1,100+1);
        if (pos <= 13)
        {
            pickup = Instantiate(pickupPointPrefab);
            puSc = pickup.GetComponent<Pickups>();
            pickup.transform.position = new Vector3(i,CubeYValue,45f);
            puSc.type = 3;
        }
        else if (pos <= 13 + 25)
        {
            pickup = Instantiate(pickupHeartPrefab);
            puSc = pickup.GetComponent<Pickups>();
            pickup.transform.position = new Vector3(i,CubeYValue,45f);
            puSc.type = 1;
        }
        else
        {
            pickup = Instantiate(pickupCrossPrefab);
            puSc = pickup.GetComponent<Pickups>();
            pickup.transform.position = new Vector3(i,CubeYValue,45f);
            puSc.type = 2;
        }
            
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
        if (angleDegrees is <= 22.5f and >= -22.5f && map[x + 1, z, 0]==0) //右
        {
            map[x+1,z,0]=1;
            return new Vector3(x+1-512,CubeYValue,z-512);
        }
        else if (angleDegrees is <= 67.5f and >= 22.5f && map[x + 1, z + 1, 0]==0)//右上
        {
            map[x+1,z+1,0]=1;
            return new Vector3(x+1-512,CubeYValue,z-512+1);
        } 
        else if (angleDegrees is <= 112.5f and >= 67.5f && map[x, z + 1, 0]==0)//上
        {
            map[x,z+1,0]=1;
            return new Vector3(x-512,CubeYValue,z-512+1);
        }
        else if (angleDegrees is <= 157.5f and >= 112.5f && map[x - 1, z + 1, 0]==0)//左上
        {
            map[x-1,z+1,0]=1;
            return new Vector3(x-1-512,CubeYValue,z-512+1);
        }
        else if (angleDegrees is <= -157.5f or >= 157.5f && map[x - 1, z, 0]==0)//左
        {
            map[x-1,z,0]=1;
            return new Vector3(x-1-512,CubeYValue,z-512);
        }
        else if (angleDegrees is <= -112.5f and >= -157.5f && map[x - 1, z - 1, 0]==0)//左下
        {
            map[x-1,z-1,0]=1;
            return new Vector3(x-1-512,CubeYValue,z-512-1);
        }
        else if (angleDegrees is <= -67.5f and >= -112.5f && map[x , z - 1, 0]==0)//下
        {
            map[x,z-1,0]=1;
            return new Vector3(x-512,CubeYValue,z-512-1);
        }
        else if(map[x + 1, z - 1, 0]==0)//右下
        {
            map[x+1,z-1,0]=1;
            return new Vector3(x+1-512,CubeYValue,z-512-1);
        }
        else
        {
            Debug.Log("不应该运行到这里");
            return new Vector3(x-512,CubeYValue,z-512);
        }
    }
    /// <summary>
    /// 返回指示表所指向的方块位置
    /// </summary>
    /// <returns></returns>
    public Vector3 AimPosNow()
    {
        var playerPos = (Vector2)UnityEngine.Camera.main.WorldToScreenPoint(transform.position) - Vector2.up * 100;
        var direction = (Vector2)Input.mousePosition - playerPos;
        var angleRadians = Mathf.Atan2(direction.y, direction.x); 
        var angleDegrees = angleRadians * Mathf.Rad2Deg;
        if(angleDegrees is <= 22.5f and >= -22.5f) return new Vector3(pos[0]-512+1,CubeYValue,pos[1]-512); //右
        else if (angleDegrees is <= 67.5f and >= 22.5f) return new Vector3(pos[0]-512+1, CubeYValue, pos[1]-512+1); //右上
        else if(angleDegrees is <= 112.5f and >= 67.5f) return new Vector3(pos[0]-512,CubeYValue,pos[1]-512+1); //上
        else if (angleDegrees is <= 157.5f and >= 112.5f) return new Vector3(pos[0]-512-1, CubeYValue, pos[1] - 512 + 1); //左上
        else if(angleDegrees is <= -157.5f or >= 157.5f) return new Vector3(pos[0]-512-1,CubeYValue,pos[1]-512); //左
        else if(angleDegrees is <= -112.5f and >= -157.5f) return new Vector3(pos[0]-512-1,CubeYValue,pos[1]-512-1); //左下
        else if(angleDegrees is <= -67.5f and >= -112.5f) return new Vector3(pos[0]-512,CubeYValue,pos[1]-512-1); //下
        else return new Vector3(pos[0]-512+1,CubeYValue,pos[1]-512-1); //右下
    }

    private bool Can_PutDown()
    {
        if(what_is_moving==0)
            return map[TransToPos(AimPosNow().x),TransToPos(AimPosNow().z),0] != 1;
        else if (what_is_moving == 1)
            return map[TransToPos(AimPosNow().x), TransToPos(AimPosNow().z), 1] != 1
                && map[TransToPos(AimPosNow().x), TransToPos(AimPosNow().z), 0] == 1;
        else return false;
    }
    /// <summary>
    /// 障碍物爆炸事件
    /// </summary>
    private void GetBomb(int x,int z){
        if(pos[0]==x && pos[1]==z){
            HP.size -= 0.2f;
        }
    }
    /// <summary>
    /// 障碍物射击的子弹
    /// </summary>
    private void GetBallet(int x, int z)
    {
        if (math.abs(pos[0] - x) + math.abs(pos[1] - z) <= 1)
        {
            HP.size -= 0.2f;
        }
    }
    
    private bool Can_PutUp(){
        if(map[TransToPos(AimPosNow().x),TransToPos(AimPosNow().z),0]==1) return true;
        else return false;
    }

    void UpdateAnimator()
    {
        animator.SetFloat("speed", move.magnitude);
    }

}
