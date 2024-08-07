using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextForDebug : MonoBehaviour{
    public TextMeshProUGUI tm;
    public Player ps;

    private Vector2 playerPos = new Vector2(956f, 388.5f);
    
    void Start(){
    }

    void Update() {
        playerPos = UnityEngine.Camera.main.WorldToScreenPoint(ps.transform.position);
        Vector2 direction = (Vector2)Input.mousePosition - playerPos;
        float angleRadians = Mathf.Atan2(direction.y, direction.x); 
        float angleDegrees = angleRadians * Mathf.Rad2Deg;

        if (tm.name == "Score")
            tm.text = angleDegrees.ToString();
        //"Score: "+ ps.Score.ToString();
        else
            tm.text = "CubeInHand: " + ps.CubeInHand.ToString();
    }
}
