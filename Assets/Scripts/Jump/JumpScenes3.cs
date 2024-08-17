using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpScenes3 : MonoBehaviour
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
        //SceneManager.LoadScene(4);
        SceneManager.LoadSceneAsync(16).completed += (operation) => //启动普通关1
        {
            //教程关
            //没有远程敌人、激光、任何技能
            GameObject ps = GameObject.Find("Player");
            Player sc = ps.GetComponent<Player>();
            sc.level = 1; //场景
            sc.now_level = 2; // 普通关2
            sc.isReading = false; //教程提示
            sc.isShooterAnemy = false; //远程敌人
            sc.isLazerAttcking = false; //激光敌人
            sc.isShooterSkill = false; //技能1
            sc.isSheldSkill = false; //技能2
            sc.isSheldSkill = false; //技能3
            sc.CubeInHandLim = 11; //方块上限
            sc.timeLimitToWin = 180; //坚持时间
        };
    }
}
