using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedUpIcon : MonoBehaviour
{
    public float coolDown = 15f;
    public bool is_using = false;
    public GameObject speedupPrefab;
    private Image rd;
    private Player ps;
    private RectTransform rt;
    private Vector2 tarpos = new Vector2(617, -405);
    void Start()
    {
        rt = GetComponent<RectTransform>();
        Cube.OnSpeedDes.AddListener(HandleSpeedDes);//订阅销毁事件
        ps = Player.instance;
        rd = GetComponent<Image>();
        StartCoroutine(CoolDown());
        StartCoroutine(RunSpeedUp());
        rt.anchoredPosition = new Vector2(617, -607f);
    }
    
    void Update()
    {
        rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, tarpos, Time.deltaTime * 2.5f);
        rd.color = new Color(1-coolDown / 18f, 1-coolDown / 18f, 1-coolDown / 18f);
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (coolDown < 0.1f && !is_using && !ps.IsCubeOn && ps.CubeInHand<ps.CubeInHandLim)
            {
                ps.IsCubeOn = true;
                is_using = true;
                //生成新的加速方块
                GameObject speedCube = Instantiate(speedupPrefab);
                speedCube.transform.position = new Vector3(ps.pos[0] - 512, 10f, ps.pos[1] - 512);
                Cube shotSc = speedCube.GetComponent<Cube>();
                shotSc.pos[0] = 512;
                shotSc.pos[1] = 512;
                shotSc.type = 5; //加速方块
                shotSc.status = 1;
                shotSc.is_moving = true;
                ps.what_is_moving = 0;
                ps.CubeInHand++;
                //等待销毁事件
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (coolDown < 0.1f && is_using)
            {
                //触发时停
                //要不要也触发一个渲染效果的改变？
                coolDown = 15f + 3f;
                StartCoroutine(RunResume());
                
            }
        }
    }
    
    private void HandleSpeedDes()
    {
        coolDown = 15f+3f;
        is_using = false;
    }

    IEnumerator RunResume()
    {
        ps.is_resumed = true;
        yield return new WaitForSeconds(3f);
        ps.is_resumed = false;
        ps.gameSpeed = 1f; //重置速率
    }
    
    IEnumerator CoolDown()
    {
        while (true)
        {
            if (coolDown >= 0.1f)
            {
                yield return new WaitForSeconds(0.1f);
                coolDown -= 0.1f;
            }
            else
                yield return null;
        }
    }

    IEnumerator RunSpeedUp()
    {
        while (true)
        {
            if (is_using && ps.gameSpeed < 2f) // 最大速率2f 速率拉满的时候要不要给点动画呢（例如表示速度的线条
            {
                yield return new WaitForSeconds(1f);
                ps.gameSpeed += 0.0333f;
            }
            else
                yield return null;
        }
    }
}
