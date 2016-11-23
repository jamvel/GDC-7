using UnityEngine;
using System.Collections;

public class HitTrigger : MonoBehaviour {
    public bool isRight = false;

    private bool waitDamage = false;
    private bool waitSword = false;
    private bool waitObject = false;
    private bool waitProjectile = false;
    
    private float waitTime = 1f;
    private float waitSwordTime = 1.1f;
    private float waitProjectileTime = 2f;
    private bool exit = true;

    private Collider2D collision;

    private int noOfFrames = 0;
    private float framesToWait = 140f;
    private float framesToWaitEnemy = 90f;

    void Update(){
        if (waitSword && exit) {
            StartCoroutine(waitSeconds(waitSwordTime));
        } //else if(waitObject) {

        //}else if (waitProjectile) {

        //}
    }

    void OnTriggerEnter2D(Collider2D c) {
        if (c.tag == "Lava") {
            transform.parent.gameObject.GetComponent<Player>().updateHealthBar(new Damage(isRight, c.gameObject.GetComponent<DamageDealer>().damage));
        } else if (c.tag == "DamagePiece") {
            collision = c;
            waitSword = true;
        } else if (c.tag == "Enemy") {
            waitDamage = true;
        } else if (c.tag == "Projectile") {
            waitProjectile = true;
        }
        //things that deal damage to player

        //tags
        //lava
        //transform.parent.gameObject.GetComponent<Player>().updateHealthBar(new Damage(isRight, c.gameObject.GetComponent<DamageDealer>().damage));

        //projectile
        //transform.parent.gameObject.GetComponent<Player>().updateHealthBar(new Damage(isRight, c.gameObject.GetComponent<DamageDealer>().damage));

        //damagePiece
        //transform.parent.gameObject.GetComponent<Player>().updateHealthBar(new Damage(isRight, c.gameObject.GetComponent<DamageDealer>().damage));

        //enemy
        //transform.parent.gameObject.GetComponent<Player>().updateHealthBar(new Damage(isRight, c.gameObject.GetComponent<Enemy>().damage));
        /*
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
        */
    }

    IEnumerator waitSeconds(float waitTime) {
        exit = false;
        noOfFrames++;
        Debug.Log("Dealing Damage: "+noOfFrames);
        //transform.parent.gameObject.GetComponent<Player>().updateHealthBar(new Damage(isRight, collision.gameObject.GetComponent<DamageDealer>().damage));
        yield return new WaitForSeconds(waitTime);
        exit = true;
    }
}