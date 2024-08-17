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
    private bool _veryFirst = true; //用于平衡游戏速率生成
    private Vector3 _birthPos;

    public static UnityEvent<int,int> OnCubeHpPickup = new UnityEvent<int, int>();

    void Start(){
        ps = Player.instance;
        rd = GetComponent<Renderer>();
        _birthPos = transform.position;
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
        if (!ps.is_resumed)
        {
            transform.position -= Vector3.forward * (0.05f * ps.gameSpeed);
            if(type == 1) transform.Rotate(Vector3.up, 90 * Time.deltaTime);
            else transform.Rotate(Vector3.forward, 90 * Time.deltaTime);
        }
            
        UpdatePos();
        if(ps.map[pos[0],pos[1]-1,0]==1){
            //触发效果
            switch(type){
                case 1:
                    if(ps.HP.size+0.1f>ps.playerHpLimit)
                        ps.HP.size = ps.playerHpLimit;
                    else ps.HP.size += 0.1f;
                    //Debug.Log("Hp Recovered");
                    break;
                case 2:
                    OnCubeHpPickup.Invoke(pos[0],pos[1]-1);
                    //Debug.Log("CubeHp Recovered");
                    break;
                case 3:
                    //Debug.Log("Score Get");
                    ps.Score++;
                    break;
            }
            Destroy(gameObject);
        }

        if(pos[1]-512<=-20){
            Destroy(gameObject);
        }
        
        if (_veryFirst && _birthPos.z - transform.position.z >= 1)
        {
            _veryFirst = false;
            ps.next_summon = true;
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
