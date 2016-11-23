using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
    public bool fromPlayer = false;

    [HideInInspector]
	public Vector2 velocityVector;
	private Rigidbody2D rb;
    private EmmiterVector emmiterArrow;

    void Start () {
        emmiterArrow = GetComponent<EmmiterVector>();
        rb = GetComponent<Rigidbody2D> ();
		rb.velocity = velocityVector;
    }
    
    void OnTriggerEnter2D(Collider2D c) {
        if(fromPlayer) {
            if (c.gameObject.tag == "Enemy") {
                Destroy(gameObject);
            }else if (c.gameObject.tag == "Wall") {
                Destroy(gameObject);
            }
        } else {
            if (c.gameObject.tag == "Player") {
                Destroy(gameObject);
            } else if (c.gameObject.tag == "Wall") {
                Destroy(gameObject);
            } else if (c.gameObject.tag == "Floor") {
                Destroy(gameObject);
            } else if (c.gameObject.tag == "Lava") {
                Destroy(gameObject);
            } else if (c.gameObject.tag == "Entry") {
                Destroy(gameObject);
            }
        }
    }

    void OnBecameInvisible() {
		Destroy (gameObject);
	}
}
