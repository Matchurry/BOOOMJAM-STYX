using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextForDebug : MonoBehaviour{
    public TextMeshProUGUI tm;
    public Player ps;
    private RectTransform rt;

    private Vector2 playerPos = new Vector2(956f, 388.5f);
    
    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (tm.name == "CubeInHand")
            tm.text = ps.CubeInHand.ToString();
        else
        {
            tm.text = " / " + ps.CubeInHandLim.ToString();
            if (ps.CubeInHand >= 10)
                rt.anchoredPosition = new Vector2(41.74f, 0);
            else
                rt.anchoredPosition = new Vector3(20.2f, 0);
        }
            
        
        
    }
}
