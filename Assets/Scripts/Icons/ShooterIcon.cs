using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShooterIcon : MonoBehaviour
{
    public float coolDown = 10f;
    public bool is_using = false;
    public GameObject shooterPrefab;
    private Image rd;
    private Player ps;
    private RectTransform rt;
    private Vector2 tarpos = new Vector2(433, -405);
    
    void Start()
    {
        rt = GetComponent<RectTransform>();
        Cube.OnShooterDes.AddListener(HandleShooterDes);//订阅销毁事件
        ps = Player.instance;
        
        if(!ps.isShooterSkill)
            Destroy(gameObject);
        
        rd = GetComponent<Image>();
        StartCoroutine(CoolDown());
        rt.anchoredPosition = new Vector2(433, -607f);
    }

    
    void Update()
    {
        rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, tarpos, Time.deltaTime * 2.5f);
        if(is_using)
            rd.color = new Color(1-coolDown / 5.1f, 1-coolDown / 5.1f, 1-coolDown / 5.1f);
        else
            rd.color = new Color(1-coolDown / 10.1f, 1-coolDown / 10.1f, 1-coolDown / 10.1f);
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (coolDown < 0.1f && !is_using && !ps.IsCubeOn && ps.CubeInHand<ps.CubeInHandLim)
            {
                ps.IsCubeOn = true;
                is_using = true;
                coolDown = 5f;
                //生成新的炮塔
                GameObject shotCube = Instantiate(shooterPrefab);
                shotCube.transform.position = new Vector3(ps.pos[0] - 512, 10f, ps.pos[1] - 512);
                Cube shotSc = shotCube.GetComponent<Cube>();
                shotSc.pos[0] = 512;
                shotSc.pos[1] = 512;
                shotSc.type = 3;
                shotSc.status = 1;
                shotSc.is_moving = true;
                ps.what_is_moving = 0;
                ps.CubeInHand++;
                //等待销毁事件
            }
        }
    }

    private void HandleShooterDes()
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
