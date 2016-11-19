using UnityEngine;
using System.Collections;

public class HitTrigger : MonoBehaviour {
    public bool isRight = false;
    void OnTriggerEnter2D(Collider2D c) {
        //things that deal damage to player

        if (c.tag == "Enemy") {
            transform.parent.gameObject.GetComponent<Player>().updateHealthBar(new Damage(isRight, c.gameObject.GetComponent<Enemy>().damage));
        }

        if (c.tag == "DamagePiece") {
            transform.parent.gameObject.GetComponent<Player>().updateHealthBar(new Damage(isRight, c.gameObject.GetComponent<DamageDealer>().damage));
        }else if (c.tag == "Lava") {
            transform.parent.gameObject.GetComponent<Player>().updateHealthBar(new Damage(isRight, c.gameObject.GetComponent<DamageDealer>().damage));
        }
    }
}