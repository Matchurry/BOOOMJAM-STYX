using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 5f; // 玩家移动速度
    public Vector3 movement; // 玩家移动方向
    public static int[,] map = new int[1024,1024];
    private int[] pos = new int[2];

    void Start()
    {
        map[0+512,0+512]=1;
        rb = GetComponent<Rigidbody>();
        Debug.Log(Vector3.right.ToString());
    }

    void Update()
    {
        //更新玩家map位置
        pos[0]=(int)transform.position.x+512;
        pos[1]=(int)transform.position.z+512;

        //玩家移动
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontalInput>0?1:-1, 0f, verticalInput>0?1:-1);
        if(horizontalInput==0) movement.x=0;
        if(verticalInput==0) movement.z=0;
        transform.position += movement * speed * Time.deltaTime;

        //生成cube
        if (Input.GetKeyDown(KeyCode.Space) && Can_PutDown(pos[0],pos[1],movement))
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = PutDownPos(pos[0],pos[1],movement);
            cube.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    bool Can_PutDown(int x, int z, Vector3 dir){
        if(dir==Vector3.right && map[x+1,z]==0) return true;
        else if(dir==Vector3.left && map[x-1,z]==0) return true;
        else if(dir==Vector3.up && map[x,z+1]==0) return true;
        else if(map[x,z-1]==0) return true;
        else return false;
    }

    Vector3 PutDownPos(int x, int z, Vector3 dir){
        if(dir==Vector3.right && map[x+1,z]==0){
            map[x+1,z]=1;
            return new Vector3(x+1-512,0,z-512);
        }
        else if(dir==Vector3.left && map[x-1,z]==0){
            map[x-1,z]=1;
            return new Vector3(x-1-512,0,z-512);
        }
        else if(dir==Vector3.up && map[x,z+1]==0) {
            map[x,z+1]=1;
            return new Vector3(x-512,0,z+1-512);
        }
        else{
            map[x,z-1]=1;
            return new Vector3(x-512,0,z-1-512);
        }
    }
}
