using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SheldIcon : MonoBehaviour
{
    public float coolDown = 1f;
    public bool is_using = false;
    public GameObject sheldPrefab;
    public static UnityEvent sheldRefresh = new UnityEvent();
    private SheldCube shedSc;
    private Image rd;
    private Player ps;
    private RectTransform rt;
    private Vector2 tarpos = new Vector2(-162, 135);

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        SheldCube.OnSheldDes.AddListener(HandleSheldDes); //订阅销毁事件
        SheldCube.sheldCubePutUp.AddListener(RefreshCD);
        SheldCube.sheldCubePutDown.AddListener(PutDown);
        ps = Player.instance;
        rd = GetComponent<Image>();

        rt.anchoredPosition = new Vector2(-162, -56.3f);
    }

    void Start()
    {
        if(!ps.isSheldSkill)
            Destroy(gameObject);
        StartCoroutine(CoolDown());
    }
    
    void Update()
    {
        rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, tarpos, Time.deltaTime * 2.5f);
        rd.color = new Color(1-coolDown / 10f, 1-coolDown / 10f, 1-coolDown / 10f);
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (coolDown < 0.1f && !is_using && !ps.IsCubeOn)
            {
                ps.IsCubeOn = true;
                is_using = true;
                GameObject sheldCube = Instantiate(sheldPrefab);
                sheldCube.transform.position = new Vector3(ps.pos[0] - 512, 10f, ps.pos[1] - 512);
                shedSc = sheldCube.GetComponent<SheldCube>();
                shedSc.pos[0] = 512;
                shedSc.pos[1] = 512;
                shedSc.is_moving = true;
                //等待销毁事件
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log(ps._protectedCol);
        }

        if (is_using && ps._protectedCol == 999 && coolDown <= 0.1f)
        {
            sheldRefresh.Invoke();
            coolDown = 10f;
        }
    }

    private void PutDown(int x, int z)
    {
        coolDown = 10f; //启动
    }
    
    private void HandleSheldDes(int x, int z)
    {
        coolDown = 10f;
        is_using = false;
    }

    private void RefreshCD()
    {
        coolDown = 10f;
    }
    
    IEnumerator CoolDown()
    {
        while (true)
        {
            if (!is_using)
            {
                if (coolDown >= 0.1f)
                {
                    yield return new WaitForSeconds(0.1f);
                    coolDown -= 0.1f;
                }
                else
                    yield return null;
            }
            else
            {
                if(shedSc.IsUnityNull() || shedSc.IsDestroyed()) yield return null;
                if (ps._protectedCol == 999 && coolDown >= 0.1f && !shedSc.is_moving)
                {
                    yield return new WaitForSeconds(0.1f);
                    coolDown -= 0.1f;
                }
                else
                    yield return null;
            }

            
        }
    }

}
