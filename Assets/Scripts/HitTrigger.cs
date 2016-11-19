using UnityEngine;
using System.Collections;

public class HitTrigger : MonoBehaviour {
    public bool isRight = false;
    void OnTriggerEnter2D(Collider2D c) {
        //things that deal damage to player
        if (c.tag == "DamagePiece" || c.tag == "Lava" || c.tag == "Enemy") {
            transform.parent.gameObject.GetComponent<Player>().updateHealthBar(new Damage(isRight, c.gameObject.GetComponent<DamageDealer>().damage));
        }

        //things that deal damage to enemy
        if (c.tag == "PlayerSword" || c.tag == "DamagePiece") {
            transform.parent.gameObject.GetComponent<Enemy>().updateHealthBar(new Damage(isRight, c.gameObject.GetComponent<DamageDealer>().damage));
        }
    }
}