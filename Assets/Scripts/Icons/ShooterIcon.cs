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
    void Start()
    {
        Cube.OnShooterDes.AddListener(HandleShooterDes);//订阅销毁事件
        ps = Player.instance;
        rd = GetComponent<Image>();
        StartCoroutine(CoolDown());
    }

    
    void Update()
    {
        rd.color = new Color(1-coolDown / 10f, 1-coolDown / 10f, 1-coolDown / 10f);
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (coolDown < 0.1f && !is_using && !ps.IsCubeOn && ps.CubeInHand<ps.CubeInHandLim)
            {
                ps.IsCubeOn = true;
                is_using = true;
                coolDown = 10f;
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
            if (!is_using && coolDown >= 0.1f)
            {
                yield return new WaitForSeconds(0.1f);
                coolDown -= 0.1f;
            }
            else
                yield return null;
        }
    }
}
