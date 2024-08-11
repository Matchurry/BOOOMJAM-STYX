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
    void Start()
    {
        //索敌 被召唤后遍历所有的障碍物

        GameObject[] bombs = GameObject.FindGameObjectsWithTag("Bomb");
        int smallestValue = int.MaxValue;
        foreach (GameObject bomb in bombs)
        {
            Bomb Sc = bomb.GetComponent<Bomb>();
            if (math.abs(Sc.pos[0]-pos[0])+math.abs(Sc.pos[1]-pos[1]) < smallestValue)
            {
                smallestValue = math.abs(Sc.pos[0] - pos[0]) + math.abs(Sc.pos[1] - pos[1]);
                bombWithSmallestValue = bomb;
            }
        }

        if (bombWithSmallestValue != null)
        {
            Debug.Log("找到 Value 最小的 Bomb: " + bombWithSmallestValue.name);
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
        if (bombWithSmallestValue != null)
        {
            tarpos = bombWithSmallestValue.transform.position;
        }
        transform.position = Vector3.Lerp(transform.position, tarpos, 0.1f);
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.25f);
        Destroy(bombWithSmallestValue);
        Destroy(gameObject);
    }
}
