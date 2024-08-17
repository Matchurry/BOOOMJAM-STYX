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

        };
        RestartClick.Invoke();
    }
}
