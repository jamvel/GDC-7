using UnityEngine;
using System.Collections;

public class HitTrigger : MonoBehaviour {
    public bool isRight = false;
    private int noOfFrames = 0;
    private float framesToWait = 140f;
    private float framesToWaitEnemy = 90f;

    void Update(){
        noOfFrames++;
    }

    void OnTriggerEnter2D(Collider2D c) {
    //things that deal damage to player
        if (c.tag == "Lava") {
            transform.parent.gameObject.GetComponent<Player>().updateHealthBar(new Damage(isRight, c.gameObject.GetComponent<DamageDealer>().damage));
        }else if(c.tag == "Projectile") {
            transform.parent.gameObject.GetComponent<Player>().updateHealthBar(new Damage(isRight, c.gameObject.GetComponent<DamageDealer>().damage));
        }else if(noOfFrames > framesToWait) {
            noOfFrames = 0;
            if (c.tag == "DamagePiece") {
                transform.parent.gameObject.GetComponent<Player>().updateHealthBar(new Damage(isRight, c.gameObject.GetComponent<DamageDealer>().damage));
            }else if (c.tag == "Enemy") {
                transform.parent.gameObject.GetComponent<Player>().updateHealthBar(new Damage(isRight, c.gameObject.GetComponent<Enemy>().damage));
            }  
        }
    }
}