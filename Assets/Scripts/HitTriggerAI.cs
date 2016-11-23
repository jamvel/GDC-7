using UnityEngine;
using System.Collections;

public class HitTriggerAI : MonoBehaviour {
    private float waitTime = 1f;
    private bool cannotCheck = false;

    void OnTriggerEnter2D(Collider2D c) {
        if(!cannotCheck) {
            if (c.tag == "PlayerSword") {
                cannotCheck = true;
                transform.gameObject.GetComponent<Enemy>().Damage(c.gameObject.GetComponent<DamageDealer>().damage);
                StartCoroutine(waitSeconds(waitTime));
            } else if (c.tag == "PlayerProjectile") {
                cannotCheck = true;
                transform.gameObject.GetComponent<Enemy>().Damage(c.gameObject.GetComponent<DamageDealer>().damage);
                StartCoroutine(waitSeconds(waitTime));
            }
        }
    }

    IEnumerator waitSeconds(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        cannotCheck = false;
    }
}