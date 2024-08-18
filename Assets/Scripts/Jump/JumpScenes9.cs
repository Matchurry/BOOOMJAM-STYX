using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Blackboard = NodeCanvas.Framework.Blackboard;

public class JumpScenes9 : MonoBehaviour
{
    [SerializeField] public Blackboard bb;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Jump()
    {
        //SceneManager.LoadScene(10);
        
        var ca = bb.GetVariable<float>("change").value;
        if ((int)ca == 0)
        { 
            // 否
            SceneManager.LoadSceneAsync(16).completed += (operation) =>
            {
                GameObject ps = GameObject.Find("Player");
                Player sc = ps.GetComponent<Player>();
                sc.level = 2; //场景
                sc.now_level = 4; // 抉择2
                sc.isReading = false; //教程提示
                sc.isShooterAnemy = true; //远程敌人
                sc.isLazerAttcking = false; //激光敌人
                sc.isShooterSkill = true; //技能1
                sc.isSpeedSkill = false; //技能2
                sc.isSheldSkill = false; //技能3
                sc.CubeInHandLim = 9; //方块上限
                sc.playerHpLimit = 0.9f; //玩家血量上限
                sc.timeLimitToWin = 60 * 5; //坚持时间
                sc.speed = 4; //移动速度小幅增加
                sc.choseChange = 0;
            };
        }
        else
        {
            // 是
            SceneManager.LoadSceneAsync(16).completed += (operation) =>
            {
                GameObject ps = GameObject.Find("Player");
                Player sc = ps.GetComponent<Player>();
                sc.level = 2; //场景
                sc.now_level = 4; // 抉择2
                sc.isReading = false; //教程提示
                sc.isShooterAnemy = true; //远程敌人
                sc.isLazerAttcking = false; //激光敌人
                sc.isShooterSkill = true; //技能1
                sc.isSpeedSkill = true; //技能2
                sc.isSheldSkill = false; //技能3
                sc.CubeInHandLim = 12; //方块上限
                sc.playerHpLimit = 0.8f; //玩家血量上限
                sc.timeLimitToWin = 60 * 5; //坚持时间
                sc.choseChange = 1;
            };
        }
        Destroy(gameObject);
    }
}
