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
    public LayerMask playerMask;

    private Transform rightBound;
    private Transform leftBound;

    public float updateRate = 5f;
    public float speed = 15f;
    public float range = 10f;
    public float range2 = 15f;
    public float stop = 0;
    public float rotationSpeed = 1;

    private float distanceToPlayer = 0;

    //[HideInInspector]
    //public bool pathEnded = false;

    private Rigidbody2D rigidBody;
    private bool searchingPlayer = false;

    private bool directionLookingAt = true;
    private Animator animator;

    private Vector2 startCoordinates;
    private Vector2 leftCastRange;
    private Vector2 rightCastRange;

    void Start(){
        animator = this.GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        target = GameObject.FindWithTag("Player").transform;
        rightBound = transform.FindChild("Right_Bound");
        leftBound = transform.FindChild("Left_Bound");
        startCoordinates = GetComponent<Transform>().position;
        
        leftCastRange = new Vector2(startCoordinates.x - range , startCoordinates.y);
        rightCastRange = new Vector2(startCoordinates.x - range, startCoordinates.y); ;

        //moving the enemy to the player
        //can be removed
        if (target == null) {
            if(!searchingPlayer) {
                searchingPlayer = true;
                //start looking for player here 
                if(SearchForPlayer()) {
                    searchingPlayer = true;
                }else {
                    searchingPlayer = false;
                }
            }
            return;
        }
    }

    void Update() {
        //rotate to look at the player
        if(SearchForPlayer()) { //player is in the same platform as skeleton
            searchingPlayer = true;
        }else { // player is on a different platform from user
            searchingPlayer = false;
            
            //check which boundary is closest to the enemy to start from there
            float leftDistance = Vector2.Distance(transform.position, leftBound.position);
            float rightDistance = Vector2.Distance(transform.position, rightBound.position);
            
            if (leftDistance > rightDistance) {
                moveEnemy(leftBound);
            }else {
                moveEnemy(rightBound);
            }


        }

        /*

        distanceToPlayer = Vector3.Distance(transform.position, target.position);
        //range = 10
        //range2 = 15
        if ((distanceToPlayer <= range2) && (distanceToPlayer >= range)){
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position), rotationSpeed * Time.deltaTime);
        }else if (distanceToPlayer <= range && distanceToPlayer > stop){
            //move towards the player
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position), rotationSpeed * Time.deltaTime);
            transform.position += transform.forward * speed * Time.deltaTime;
        }else if (distanceToPlayer <= stop){
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position), rotationSpeed * Time.deltaTime);
        }
        */
    }

    //check if player is in the same platform as the enemy
    public bool SearchForPlayer() {
        if (Physics2D.Linecast(transform.position, leftBound.position, playerMask) || Physics2D.Linecast(transform.position, rightBound.position, playerMask)){ //Bound of platform check
            Debug.DrawLine(transform.position, rightBound.position, Color.magenta);
            Debug.DrawLine(transform.position, leftBound.position, Color.magenta);
            return true;
        } else {
            return false;
        }
    }

    public void moveEnemy(Transform objective) {
        float step = speed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, objective.position, step);
    }
}
