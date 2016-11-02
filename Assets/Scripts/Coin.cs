using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {
    public int value = 10;
    public float speed = 4.0f;
    private Transform target = null;

    void Update() {
        if (target != null) {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);

            if (transform.position == target.position) {
                target.gameObject.GetComponent<Player>().updateCoinCounter(value);
                DestroyObject(this.gameObject);
            }
        }
        
    }
    void OnTriggerEnter2D(Collider2D c) {
        if(c.gameObject.tag == "Player") {
            target = c.gameObject.transform;
        }
    }

}
