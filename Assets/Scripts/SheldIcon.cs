using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SheldIcon : MonoBehaviour
{
public float coolDown = 1f;
    public bool is_using = false;
    public GameObject sheldPrefab;
    private Image rd;
    private Player ps;
    void Start()
    {
        Cube.OnSpeedDes.AddListener(HandleSheldDes);//订阅销毁事件
        ps = Player.instance;
        rd = GetComponent<Image>();
        StartCoroutine(CoolDown());
    }
    
    void Update()
    {
        rd.color = new Color(1-coolDown / 10f, 1-coolDown / 10f, 1-coolDown / 10f);
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (coolDown < 0.1f && !is_using && !ps.IsCubeOn)
            {
                ps.IsCubeOn = true;
                is_using = true;
                //生成新的加速方块
                GameObject sheldCube = Instantiate(sheldPrefab);
                sheldCube.transform.position = new Vector3(ps.pos[0] - 512, 10f, ps.pos[1] - 512);
                SheldCube shedSc = sheldCube.GetComponent<SheldCube>();
                shedSc.pos[0] = 512;
                shedSc.pos[1] = 512;
                shedSc.is_moving = true;
                //等待销毁事件
            }
        }
    }
    
    private void HandleSheldDes()
    {
        coolDown = 10f;
        is_using = false;
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

}
