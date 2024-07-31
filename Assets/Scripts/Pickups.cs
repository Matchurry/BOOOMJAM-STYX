using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;

public class Pickups : MonoBehaviour{
    public int[] pos = new int[2];
    [Tooltip("1玩家血量 2方块血量 3分数道具 -1未初始化")]
    public int type = -1;
    public Player ps;
    private const float CubeYValue = 0.505f;
    private Renderer rd;

    public static UnityEvent<int,int> OnCubeHpPickup = new UnityEvent<int, int>();

    void Start(){
        ps = Player.instance;
        rd = GetComponent<Renderer>();
        //transform.position = new Vector3((float)Math.Round(UnityEngine.Random.Range(-5f,5f)),CubeYValue,30f);
        switch(type){
            case 1:
                rd.material.color = new Color(0.9339f,0.3832f,0.3832f);
                break;
            case 2:
                rd.material.color = new Color(0.5094f,0.1177f,0.1177f);
                break;
            case 3:
                rd.material.color = new Color(0.8867f,0.7081f,0.2969f);
                break;
        }
    }

    void Update(){
        transform.position -= Vector3.forward * 0.05f;
        UpdatePos();
        if(ps.map[pos[0],pos[1]-1]==1){
            //触发效果
            switch(type){
                case 1:
                    if(ps.HP.size+0.1f>1f)
                        ps.HP.size = 1f;
                    else ps.HP.size += 0.1f;
                    Debug.Log("Hp Recovered");
                    break;
                case 2:
                    OnCubeHpPickup.Invoke(pos[0],pos[1]-1);
                    Debug.Log("CubeHp Recoered");
                    break;
                case 3:
                    Debug.Log("Score Get");
                    ps.Score++;
                    break;
            }
            Destroy(gameObject);
        }

        if(pos[1]-512<=-20){
            Destroy(gameObject);
        }
    }

    private void UpdatePos(){
        pos[0]=TransToPos(transform.position.x);
        pos[1]=TransToPos(transform.position.z);
    }

    private int TransToPos(float x){
        return (int)Math.Round(x+512);
    }
}
