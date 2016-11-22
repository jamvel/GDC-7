using UnityEngine;
using System.Collections;

public class HitTriggerAI : MonoBehaviour {
    private int counter = 0;
    private int counterP = 0;

    void Update(){
        counter++;//increment the counter each frame
        counterP++;
    }

    void OnTriggerEnter2D(Collider2D c) {
        //things that deal damage to enemy
        if (c.tag == "PlayerSword") {
            if (counter > 35) {//to only receive one hit at a time instead of multiple hits from one single attack
                counter = 0;
                transform.gameObject.GetComponent<Enemy>().Damage(c.gameObject.GetComponent<DamageDealer>().damage);
            } 
        } else if (c.tag == "PlayerProjectile") {
            if (counterP > 20) {//to only receive one hit at a time instead of multiple hits from one single attack
                counterP = 0;
                transform.gameObject.GetComponent<Enemy>().Damage(c.gameObject.GetComponent<DamageDealer>().damage);
            }
        }
    }
}