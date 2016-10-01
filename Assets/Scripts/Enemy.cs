using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    [System.Serializable]
    public class EnemyStats {
        public int Health = 100;
    }

    public EnemyStats enemyStats = new EnemyStats();
    
    public void DamageEnemy (int damage) {
        enemyStats.Health -= damage;
        if(enemyStats.Health <=0) {
            KillEnemy(this);
        }
    }

    public static void KillEnemy(Enemy enemy) {
        Destroy(enemy.gameObject);
    }
}
