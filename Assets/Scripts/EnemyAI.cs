using UnityEngine;
using System.Collections;

/*
 * Enemy AI class
 * This class targets the player only if the player is on the platform that user is on
 * Otherwise if the enemy is not on the platform the enemy will move from one bound to another
 * 
 */
public class EnemyAI : MonoBehaviour {
    public Transform target;

    public float updateRate = 5f;
    public float speed = 15f;
    public float range = 15f;
    public float range2 = 10f;
    public float stop = 0;
    public float rotationSpeed = 1;
    private float distanceToPlayer = 0;

    [HideInInspector]
    public bool pathEnded = false;

    private Rigidbody2D rigidBody;
    private bool searchingPlayer = false;
    private bool directionLookingAt = true;
    private Animator animator;

    void Start(){
        animator = this.GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        target = GameObject.FindWithTag("Player").transform;
       
        //moving the enemy to the player
        if(target == null) {
            if(!searchingPlayer) {
                searchingPlayer = true;
                //start looking for player here 
            }
            return;
        }
    }

    void Update() {
        //rotate to look at the player

        distanceToPlayer = Vector3.Distance(transform.position, target.position);
        if ((distanceToPlayer <= range2) && (distanceToPlayer >= range)){
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position), rotationSpeed * Time.deltaTime);
        }else if (distanceToPlayer <= range && distanceToPlayer > stop){
            //move towards the player
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position), rotationSpeed * Time.deltaTime);
            transform.position += transform.forward * speed * Time.deltaTime;
        }else if (distanceToPlayer <= stop){
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position), rotationSpeed * Time.deltaTime);
        }
    }
}
