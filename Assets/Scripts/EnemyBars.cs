using UnityEngine;
using System.Collections;

public class EnemyBars : MonoBehaviour{
    public int maxHealth = 100;
    public int curHealth = 100;

    public float healthBarLength;

    // Use this for initialization
    void Start(){
        healthBarLength = Screen.width / 6;
    }

    // Update is called once per frame
    void Update(){
        AddjustCurrentHealth(0);
    }

    void OnGUI(){
        Vector2 targetPos;
        targetPos = GameObject.FindGameObjectWithTag("Enemy").transform.localPosition;
        GUI.Box(new Rect(targetPos.x, -targetPos.y, healthBarLength, 20), curHealth + "/" + maxHealth);
    }

    public void AddjustCurrentHealth(int adj){
        curHealth += adj;

        if (curHealth < 0)
            curHealth = 0;

        if (curHealth > maxHealth)
            curHealth = maxHealth;

        if (maxHealth < 1)
            maxHealth = 1;

        healthBarLength = (Screen.width / 6) * (curHealth / (float)maxHealth);
    }
}
