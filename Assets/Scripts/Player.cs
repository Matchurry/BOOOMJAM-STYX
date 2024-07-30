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

public class Player : MonoBehaviour{
    public static Player instance; // 静态实例变量
    public GameObject cubePrefab;
    public GameObject bombPrefab;
    private const float CubeYValue = 0.505f;
    public float speed = 3f;
    private Vector3 movement;
    private int dir = 1;
    public int[,] map = new int[1024,1024];
    public int[] pos = new int[2];
    public static UnityEvent<int, int> OnCubePutOn = new UnityEvent<int, int>();
    public static UnityEvent<int, int> OnCubePutDown = new UnityEvent<int, int>();
    private bool IsCubeOn = false;
    private float startTime=0f;
    public Scrollbar HP;

    void Awake(){
        instance = this;
    }

    void Start(){
        Bomb.OnBombTriggered.AddListener(GetBomb);
        StartCoroutine(RunDelayedLoop());
        StartCoroutine(RunBlockSummonLoop());
        StartCoroutine(RunBombSummonLoop());
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

    IEnumerator RunBlockSummonLoop(){
        while(true){
            SummonCube();
            yield return new WaitForSeconds(UnityEngine.Random.Range(3f,6f));
        }
    }

    IEnumerator RunBombSummonLoop(){
        while(true){
            SummonBomb();
            yield return new WaitForSeconds(UnityEngine.Random.Range(6f,10f));
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
        
        if(horizontalInput!=0) dir = horizontalInput < 0 ? 4 : 2; //1up 2right 3down 4left
        else movement.x=0;
        if(verticalInput!=0) dir = verticalInput > 0 ? 1 : 3;
        else movement.z=0;
        var tarpos = transform.position + movement * speed * Time.deltaTime;
        if(map[TransToPos(tarpos.x),TransToPos(tarpos.z)]==1 || map[pos[0],pos[1]]!=1)
            transform.position=tarpos;

        //抬起Cube
        if (Input.GetKeyDown(KeyCode.Space) && !IsCubeOn && Can_PutUp()){
            //PutCube();
            startTime = Time.time;
            int x = TransToPos(AimPosNow().x);
            int z = TransToPos(AimPosNow().z);
            if(map[x,z]==1) OnCubePutOn.Invoke(x,z);
            IsCubeOn = true;
            map[x,z]=0;
        }

        //放下Cube
        if(Input.GetKeyDown(KeyCode.Space) && IsCubeOn && Can_PutDown() && Time.time-startTime>=0.1f){
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
    /// 生成一个新的方块，不需任何参数，取决于玩家的位置。
    /// 请注意提前进行判断。
    /// </summary>
    private void PutCube(){
        GameObject cube = Instantiate(cubePrefab);
        Cube cubeScript = cube.GetComponent<Cube>();
        cube.transform.position = PutDownPos(pos[0],pos[1],dir);
        cubeScript.pos[0] = TransToPos(cube.transform.position.x);
        cubeScript.pos[1] = TransToPos(cube.transform.position.z);
        cube.transform.localScale = new Vector3(1, 1, 1);
        cubeScript.status = 1;
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
    private void SummonCube(){
        GameObject cube = Instantiate(cubePrefab);
        Cube cubeScript = cube.GetComponent<Cube>();
        cube.transform.localScale = new Vector3(1, 1, 1);
        cubeScript.status = 2;
    }
    /// <summary>
    /// 生成炸弹
    /// </summary>
    private void SummonBomb(){
        GameObject bomb = Instantiate(bombPrefab);
    }

    /// <summary>
    /// 输入玩家位置信息和方向，返回目标方块位置的三维坐标
    /// 注意此函数还会直接操作地图数据
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="_dir"></param>
    /// <returns></returns>
    private Vector3 PutDownPos(int x, int z, int _dir){
        if(_dir==2 && map[x+1,z]==0){
            map[x+1,z]=1;
            return new Vector3(x+1-512,CubeYValue,z-512);
        }
        else if(_dir==4 && map[x-1,z]==0){
            map[x-1,z]=1;
            return new Vector3(x-1-512,CubeYValue,z-512);
        }
        else if(_dir==1 && map[x,z+1]==0) {
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
        if(dir==2) return new Vector3(pos[0]-512+1,CubeYValue,pos[1]-512);
        else if(dir==4) return new Vector3(pos[0]-512-1,CubeYValue,pos[1]-512);
        else if(dir==1) return new Vector3(pos[0]-512,CubeYValue,pos[1]-512+1);
        else return new Vector3(pos[0]-512,CubeYValue,pos[1]-512-1);
    }

    private bool Can_PutDown(){
        if(map[TransToPos(AimPosNow().x),TransToPos(AimPosNow().z)]==1) return false;
        else return true;
    }

    private void GetBomb(int x,int z){
        if(Math.Abs(pos[0]-x)+Math.Abs(pos[1]-z)<=2 && Math.Abs(pos[0]-x)<=1 && Math.Abs(pos[1]-z)<=1){
            HP.size -= 0.2f;
        }
    }

    private bool Can_PutUp(){
        if(map[TransToPos(AimPosNow().x),TransToPos(AimPosNow().z)]==1) return true;
        else return false;
    }
}
