using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	[HideInInspector]
	public Vector2 velocityVector;
	private Rigidbody2D rb;

	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		rb.velocity = velocityVector;

	}

	void OnBecameInvisible() {
		Destroy (gameObject);
	}
}
