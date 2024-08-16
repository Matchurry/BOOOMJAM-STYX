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

        if (tm.name == "Score")
            tm.text = "Score: "+ ps.Score.ToString();
        else
            tm.text = "CubeInHand: " + ps.CubeInHand.ToString();
    }
}
