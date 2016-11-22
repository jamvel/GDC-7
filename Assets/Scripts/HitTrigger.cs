using UnityEngine;
using System.Collections;

public class HitTrigger : MonoBehaviour {
public bool isRight = false;
private int counter = 0;

void Update(){
    counter++;//increment the counter each frame
}

void OnTriggerEnter2D(Collider2D c) {
    //things that deal damage to player
        if (c.tag == "Enemy") {
            transform.parent.gameObject.GetComponent<Player>().updateHealthBar(new Damage(isRight, c.gameObject.GetComponent<Enemy>().damage));
        }else if (c.tag == "DamagePiece") {
            if (counter > 150) {//to only receive one hit at a time instead of multiple hits from one single attack
                counter = 0;
                transform.parent.gameObject.GetComponent<Player>().updateHealthBar(new Damage(isRight, c.gameObject.GetComponent<DamageDealer>().damage));
            }
        } else if (c.tag == "Lava") {
            transform.parent.gameObject.GetComponent<Player>().updateHealthBar(new Damage(isRight, c.gameObject.GetComponent<DamageDealer>().damage));
        }else if(c.tag == "Projectile") {
            transform.parent.gameObject.GetComponent<Player>().updateHealthBar(new Damage(isRight, c.gameObject.GetComponent<DamageDealer>().damage));
        }
    }
}