using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpScenes6 : MonoBehaviour
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
        //SceneManager.LoadScene(7);
        SceneManager.LoadSceneAsync(16).completed += (operation) =>
        {
            GameObject ps = GameObject.Find("Player");
            Player sc = ps.GetComponent<Player>();
            sc.level = 2; //场景
            sc.now_level = 3; // 抉择1
            sc.isReading = false; //教程提示
            sc.isShooterAnemy = true; //远程敌人
            sc.isLazerAttcking = false; //激光敌人
            sc.isShooterSkill = true; //技能1
            sc.isSheldSkill = false; //技能2
            sc.isSheldSkill = false; //技能3
            sc.CubeInHandLim = 12; //方块上限
            sc.playerHpLimit = 0.9f; //玩家血量上限
            sc.timeLimitToWin = 60*4; //坚持时间
        };
    }
}
