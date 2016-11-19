using UnityEngine;
using System.Collections;

public class HitTriggerAI : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D c) {
        //things that deal damage to enemy
        if (c.tag == "PlayerSword") {
            transform.gameObject.GetComponent<Enemy>().Damage(c.gameObject.GetComponent<DamageDealer>().damage);
        }
    }
}