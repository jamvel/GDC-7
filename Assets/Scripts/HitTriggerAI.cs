using UnityEngine;
using System.Collections;

public class HitTriggerAI : MonoBehaviour {
    private float waitTime = 1f;
    private bool cannotCheck = false;

    void OnTriggerEnter2D(Collider2D c) {
        if(!cannotCheck) {
            if (c.tag == "PlayerSword") {
                transform.gameObject.GetComponent<Enemy>().Damage(c.gameObject.GetComponent<DamageDealer>().damage);
            } else if (c.tag == "PlayerProjectile") {
                transform.gameObject.GetComponent<Enemy>().Damage(c.gameObject.GetComponent<DamageDealer>().damage);
            }
        }
    }

    IEnumerator waitSeconds(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        cannotCheck = false;
    }
}