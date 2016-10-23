using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public float health;
    public float damage;
    
    /*public void DamageEnemy (int damage) {
        enemyStats.health -= damage;
        if(enemyStats.health <=0) {
            killEnemy(this);
        }
    }*/

    public static void killEnemy(Enemy enemy) {
        Destroy(enemy.gameObject);
    }
}
