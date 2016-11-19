using UnityEngine;
using System.Collections;

public class HitTrigger : MonoBehaviour {
    public bool isRight = false;
    void OnTriggerEnter2D(Collider2D c) {
        if(c.tag == "Enemy") {
            transform.parent.gameObject.GetComponent<Player>().updateHealthBar(new Damage(isRight, c.gameObject.GetComponent<Enemy>().damage));
        }

        if (c.tag == "DamagePiece") {
            transform.parent.gameObject.GetComponent<Player>().updateHealthBar(new Damage(isRight, c.gameObject.GetComponent<DamageDealer>().damage));
        }

        if (c.tag == "PlayerSword") {
            transform.parent.gameObject.GetComponent<Enemy>().updateHealthBar(new Damage(isRight, c.gameObject.GetComponent<DamageDealer>().damage));
        }
    }
}