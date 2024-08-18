using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpScenes1 : MonoBehaviour
{
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Jump()
    {
        SceneManager.LoadSceneAsync(16).completed += (operation) =>
        {
            //教程关
            //没有远程敌人、激光、任何技能
            GameObject ps = GameObject.Find("Player");
            Player sc = ps.GetComponent<Player>();
            sc.level = 1; //场景
            sc.now_level = 0; //教程
            sc.isReading = true; //教程提示
            sc.isShooterAnemy = false; //远程敌人
            sc.isLazerAttcking = false; //激光敌人
            sc.isShooterSkill = false; //技能1
            sc.isSpeedSkill = false; //技能2
            sc.isSheldSkill = false; //技能3
            sc.CubeInHandLim = 9; //方块上限
            sc.timeLimitToWin = 60; //坚持时间
        };
        Destroy(gameObject);
    }
    
}
