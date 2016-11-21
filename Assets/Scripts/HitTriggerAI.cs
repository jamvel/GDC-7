using UnityEngine;
using System.Collections;

public class HitTriggerAI : MonoBehaviour {
    private int counter = 0;

    void Update(){
        counter++;//increment the counter each frame
    }

    void OnTriggerEnter2D(Collider2D c) {
        //things that deal damage to enemy
        if (counter > 35){//to only receive one hit at a time instead of multiple hits from one single attack
            counter = 0;

            if (c.tag == "PlayerSword") {
                transform.gameObject.GetComponent<Enemy>().Damage(c.gameObject.GetComponent<DamageDealer>().damage);
            }
        }
    }
}