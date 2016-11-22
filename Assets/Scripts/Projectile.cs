using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
    public float timeToTravel = 0f;

    [HideInInspector]
	public Vector2 velocityVector;
	private Rigidbody2D rb;
    private EmmiterVector emmiterArrow;
    private bool canDestory = false;

    void Start () {
        emmiterArrow = GetComponent<EmmiterVector>();
        rb = GetComponent<Rigidbody2D> ();
		rb.velocity = velocityVector;
        StartCoroutine(DestroyAfterSeconds());
    }

    void Update() {
        if (canDestory) {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D c) {
        if (c.gameObject.tag == "Player") {
            Destroy(gameObject);
        }
    }

    IEnumerator DestroyAfterSeconds() {
        yield return new WaitForSeconds(timeToTravel);
        canDestory = true;
    }

    void OnBecameInvisible() {
		Destroy (gameObject);
	}
}
