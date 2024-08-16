using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEngine;

public class ShooterBalletSc : MonoBehaviour
{
    public int[] pos = new int[2];
    private Vector3 tarpos = new Vector3();
    private GameObject bombWithSmallestValue = null;
    private Player ps;
    void Start()
    {
        //索敌 被召唤后遍历所有的障碍物
        ps = Player.instance;
        GameObject[] bombs = GameObject.FindGameObjectsWithTag("Bomb");
        int smallestValue = int.MaxValue;
        foreach (GameObject bomb in bombs)
        {
            Bomb Sc = bomb.GetComponent<Bomb>();
            if(Sc.pos[1]<=ps.pos[1] || Sc.pos[1]>=25+512) continue; //在玩家背后的不会被索敌 太远的不会被索敌
            if (math.abs(Sc.pos[0]-pos[0])+math.abs(Sc.pos[1]-pos[1]) < smallestValue)
            {
                smallestValue = math.abs(Sc.pos[0] - pos[0]) + math.abs(Sc.pos[1] - pos[1]);
                bombWithSmallestValue = bomb;
            }
        }

        if (bombWithSmallestValue != null)
        {
            StartCoroutine(Attack());
        }
        else
        {
            Debug.Log("没有找到带有 Bomb 脚本的 GameObject");
            Destroy(gameObject);
        }
    }
    
    void Update()
    {
        if (bombWithSmallestValue is not null)
        {
            try
            {
                tarpos = bombWithSmallestValue.transform.position;
            }
            catch(MissingReferenceException) // 若在攻击到之前已经销毁 则无视本次攻击
            {
                bombWithSmallestValue = null;
                Destroy(gameObject);
            }
            
        }
        transform.position = Vector3.Lerp(transform.position, tarpos, 0.1f);
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.25f);
        try
        {
            Destroy(bombWithSmallestValue);
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.ToString());
        }
        
        Destroy(gameObject);
    }
}
