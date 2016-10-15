using UnityEngine;
using System.Collections;

public class EnemyBoltController : MonoBehaviour {

    public float speed;
    public PlayerController target;
    //need to create impact effect for bullets then use here
    public GameObject impactEffect;
    public int damageToGive;

    private Animator animator;
    private Rigidbody2D myRigidBody2D;

	void Start () {
        animator = this.GetComponent<Animator>();
        target = FindObjectOfType<PlayerController>();
        myRigidBody2D = GetComponent<Rigidbody2D>();
        
        if(target.transform.position.x < transform.position.x) {
            animator.SetBool("IsRight", false);
            speed = -speed;
        }else {
            animator.SetBool("IsRight", true);
        }
	}
	
	void Update () {
        myRigidBody2D.velocity = new Vector2(speed, myRigidBody2D.velocity.y);

    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player") {
            //deal damage
        }
    }
}
