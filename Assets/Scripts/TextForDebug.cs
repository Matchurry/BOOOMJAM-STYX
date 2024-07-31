using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextForDebug : MonoBehaviour{
    public TextMeshProUGUI tm;
    public Player ps;
    void Start(){
    }

    void Update(){
        if(tm.name=="Score")
            tm.text = "Score: "+ps.Score.ToString();
        else
            tm.text = "CubeInHand: "+ps.CubeInHand.ToString();
    }
}
