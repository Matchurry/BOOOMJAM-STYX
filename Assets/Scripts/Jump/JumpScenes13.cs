using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpScenes13 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Jump()
    {
        //SceneManager.LoadScene(16);
        SceneManager.LoadSceneAsync(16).completed += (operation) =>
        {
            GameObject ps = GameObject.Find("Player");
            Player sc = ps.GetComponent<Player>();
            sc.level = 3; //场景
            sc.now_level = 6; // Boss
            sc.isReading = false; //教程提示
            sc.isShooterAnemy = true; //远程敌人
            sc.isLazerAttcking = true; //激光敌人
            sc.isShooterSkill = true; //技能1
            sc.isSpeedSkill = true; //技能2
            sc.isSheldSkill = true; //技能3
            sc.CubeInHandLim = 12; //方块上限
            sc.playerHpLimit = 0.8f; //玩家血量上限
            sc.timeLimitToWin = 60 * 3; //坚持时间
            sc.choseChange = 1;
        };
    }
}
