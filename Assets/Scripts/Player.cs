using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour {
    public static int health = 100;
    public static int mana = 100;
    public Slider healthBar;
    public Slider manaBar;

    // Use this for initialization
    void Start () {
	    //InvokeRepeating("Damage",1,1);
    }

    void Damage(int dam){
        health = health - dam/*20*/;
        healthBar.value = health;
        if (health <= 0){
            //die
        }
    }

    void Heal(int heal){
        health = health + heal;
        if (health <= 100){
            healthBar.value = health;
        }else{
            healthBar.value = 100;
        }
    }

    void MagicUse(int use){
        mana = mana - use;
        manaBar.value = mana;
    }

    void MagicRestore(int rest){
        mana = mana + rest;
        if (mana <= 100){
            manaBar.value = mana;
        }else{
            manaBar.value = 100;
        }
    }

    // Update is called once per frame
    void Update () {
        Damage(1);
    }
}
