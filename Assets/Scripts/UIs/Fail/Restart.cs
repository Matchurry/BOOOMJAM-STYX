using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Restart : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private Player ps;
    private Image _image;
    public static UnityEvent RestartClick = new UnityEvent();
    void Start()
    {
        _image = GetComponent<Image>();
        ps = Player.instance;
    }
    
    void Update()
    {
        
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        _image.color = new Color(0,0,0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _image.color = new Color(1,1,1);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _image.color = new Color(0, 0, 0, 0.5f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SceneManager.LoadSceneAsync(16).completed += (operation) =>
        {
            if (ps.now_level == 0) //教程关重开
            {
                GameObject ps = GameObject.Find("Player");
                Player sc = ps.GetComponent<Player>();
                sc.level = 1; //场景
                sc.now_level = 0; //教程
                sc.isReading = true; //教程提示
                sc.isShooterAnemy = false; //远程敌人
                sc.isLazerAttcking = false; //激光敌人
                sc.isShooterSkill = false; //技能1
                sc.isSheldSkill = false; //技能2
                sc.isSheldSkill = false; //技能3
                sc.CubeInHandLim = 9; //方块上限
                sc.timeLimitToWin = 60; //坚持时间
            }
            else if (ps.now_level == 2) //普通关2重开
            {
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
            }
            else if (ps.now_level == 3) //抉择1重开
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
            }
            else if (ps.now_level == 4) //抉择2重开
            {
                GameObject ps2 = GameObject.Find("Player");
                Player sc = ps2.GetComponent<Player>();
                sc.level = 2; //场景
                sc.now_level = 4; // 抉择2
                sc.isReading = false; //教程提示
                sc.isShooterAnemy = true; //远程敌人
                sc.isLazerAttcking = false; //激光敌人
                sc.isShooterSkill = true; //技能1
                sc.isSpeedSkill = ps.isSpeedSkill; //技能2
                sc.isSheldSkill = ps.isSheldSkill; //技能3
                sc.CubeInHandLim = ps.CubeInHandLim; //方块上限
                sc.playerHpLimit = ps.playerHpLimit; //玩家血量上限
                sc.timeLimitToWin = 60 * 5; //坚持时间
                sc.speed = ps.speed; //移动速度
                sc.choseChange = ps.choseChange;
            }
            else if (ps.now_level == 5) //最终回重开
            {
                GameObject ps2 = GameObject.Find("Player");
                Player sc = ps2.GetComponent<Player>();
                sc.level = 3; //场景
                sc.now_level = 5; // 最终回
                sc.isReading = false; //教程提示
                sc.isShooterAnemy = true; //远程敌人
                sc.isLazerAttcking = false; //激光敌人
                sc.isShooterSkill = true; //技能1
                sc.isSpeedSkill = ps.isSpeedSkill; //技能2
                sc.isSheldSkill = ps.isSheldSkill; //技能3
                sc.CubeInHandLim = ps.CubeInHandLim; //方块上限
                sc.playerHpLimit = ps.playerHpLimit; //玩家血量上限
                sc.timeLimitToWin = 60 * 5; //坚持时间
                sc.speed = ps.speed; //移动速度
                sc.choseChange = ps.choseChange;
            }
            else if (ps.now_level == 6) //Boss关重开
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
            }
        };
        RestartClick.Invoke();
        Destroy(gameObject);
    }
}
