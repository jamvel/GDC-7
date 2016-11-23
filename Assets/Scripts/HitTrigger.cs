using UnityEngine;
using System.Collections;

public class HitTrigger : MonoBehaviour {
    public bool isRight = false;
    private float waitTime = 1f;
    private bool cannotCheck = false;

    void OnTriggerEnter2D(Collider2D c) {
        Debug.Log("Collsion with: " + c.tag);
        if (!cannotCheck) {
            if (c.tag == "Lava") {
                cannotCheck = true;
                transform.parent.gameObject.GetComponent<Player>().updateHealthBar(new Damage(isRight, c.gameObject.GetComponent<DamageDealer>().damage));
            }else if (c.tag == "Projectile") {
                cannotCheck = true;
                transform.parent.gameObject.GetComponent<Player>().updateHealthBar(new Damage(isRight, c.gameObject.GetComponent<DamageDealer>().damage));
                StartCoroutine(waitSeconds(waitTime));
            } else if (c.tag == "DamagePiece") {
                cannotCheck = true;
                transform.parent.gameObject.GetComponent<Player>().updateHealthBar(new Damage(isRight, c.gameObject.GetComponent<DamageDealer>().damage));
                StartCoroutine(waitSeconds(waitTime));
            } else if (c.tag == "Enemy") {
                cannotCheck = true;
                transform.parent.gameObject.GetComponent<Player>().updateHealthBar(new Damage(isRight, c.gameObject.GetComponent<Enemy>().damage));
                StartCoroutine(waitSeconds(waitTime));
            }
        }
    }

    IEnumerator waitSeconds(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        cannotCheck = false;
    }
}