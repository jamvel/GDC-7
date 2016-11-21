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
        StartCoroutine(DestroyAfterSecords());
    }

    void Update() {
        if (canDestory) {
            Destroy(gameObject);
        }
    }

    IEnumerator DestroyAfterSecords() {
        yield return new WaitForSeconds(timeToTravel);
        canDestory = true;
    }

    void OnBecameInvisible() {
		Destroy (gameObject);
	}
}
