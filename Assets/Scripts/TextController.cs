using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextController : MonoBehaviour {

    public float maxHealth = 100;
    public float curHealth = 100;

    public Text instruction;

    // Use this for initialization
    void Start(){
    }

    // Update is called once per frame
    void Update(){
        AddjustCurrentHealth(-0.1f);
        instruction.text = curHealth + "/" + maxHealth;
    }

    public void Write(string value){
        instruction.text = value;
    }

    public void AddjustCurrentHealth(float adj){
        curHealth += adj;

        if (curHealth < 0)
            curHealth = 0;

        if (curHealth > maxHealth)
            curHealth = maxHealth;

        if (maxHealth < 1)
            maxHealth = 1;
    }
}
