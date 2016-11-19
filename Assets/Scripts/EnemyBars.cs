using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyBars : MonoBehaviour
{
    public static int health = 100;
    public Slider[] healthBar;

    // Use this for initialization
    void Start()
    {
        //InvokeRepeating("Damage",1,1);
    }

    void Damage(int dam, int i)
    {
        health = health - dam/*20*/;
        healthBar[i].value = health;
        if (health <= 0)
        {
            //die
        }
    }
    /*
    void Heal(int heal, int i)
    {
        health = health + heal;
        if (health <= 100)
        {
            healthBar[i].value = health;
        }
        else
        {
            healthBar[i].value = 100;
        }
    }*/

    // Update is called once per frame
    void Update()
    {
        Damage(1, 0);
    }
}

