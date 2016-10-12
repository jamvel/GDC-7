using UnityEngine;
using System.Collections;

/*
 * Enemy AI class
 * This class targets the player only if the player is on the platform that user is on
 * Otherwise if the enemy is not on the platform the enemy will move from one bound to another
 * 
 */
public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public Transform rightBound;
    public Transform leftBound;

    public LayerMask playerMask;

    public float updateRate = 5f;
    public float speed = 15f;
    public float range = 10f;
    public float range2 = 15f;
    public float stop = 0;
    public float rotationSpeed = 1;
    public float precisionOfEnemyToBounds = 0.26f;

    private float distanceToPlayer = 0;

    //[HideInInspector]
    //public bool pathEnded = false;

    private Rigidbody2D rigidBody;
    private bool searchingPlayer = false;
    private bool walkingToLeftBound = true;

    private bool directionLookingAt = true;
    private Animator animator;

    private Vector2 startCoordinates;
    private Vector2 leftCastRange;
    private Vector2 rightCastRange;

    void Start()
    {
        animator = this.GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        target = GameObject.FindWithTag("Player").transform;
        
        //rightBound = GameObject.FindGameObjectWithTag("Platform").transform.FindChild("Right_Bound");
        //leftBound  = GameObject.FindGameObjectWithTag("Platform").transform.FindChild("Left_Bound");

        //rightBound = transform.FindChild("Right_Bound");
        //leftBound = transform.FindChild("Left_Bound");

        startCoordinates = GetComponent<Transform>().position;

        leftCastRange = new Vector2(startCoordinates.x - range, startCoordinates.y);
        rightCastRange = new Vector2(startCoordinates.x - range, startCoordinates.y); ;

        float leftDistance = transform.position.x - leftBound.position.x;
        float rightDistance = transform.position.x - rightBound.position.x;
        float result = (leftDistance - rightDistance)/2;

        Debug.Log("Position" + transform.position.x);
        Debug.Log("Left Direction: " + leftBound.position.x);
        Debug.Log("Right Direction: " + rightBound.position.x);
        Debug.Log("Result: " + result);

        //moving the enemy to the player
        //can be removed
        if (target == null){
            if (!searchingPlayer){
                searchingPlayer = true;
                //start looking for player here 
                if (SearchForPlayer()){//go towards player
                    searchingPlayer = true;
                }else{//go towards closest edge

                    //check which boundary is closest to the enemy to start from there
                    if ((leftBound.position.x < transform.position.x) && (transform.position.x < result)) {
                        //go to the left most bound
                        walkingToLeftBound = true;
                    } else if ((rightBound.position.x > transform.position.x) && (transform.position.x > result)) {
                        //go to the right most bound
                        walkingToLeftBound = false;
                    } else {//transform is not in the bounds
                        //out of bounds
                        //start searching for new bounds
                        //get new bounds
                        Debug.Log("Target is out of Bounds");
                        return;
                    }
                }
            }
            Debug.Log("Target not found");
            return;
        }
    }


    void Update() {
        //rotate to look at the player
        if (SearchForPlayer()){ //player is in the same platform as skeleton
            searchingPlayer = true;
            animatorSetting();
            //move towards the player
        }else{ // player is on a different platform from user --- patrol function
            searchingPlayer = false;
            patrol();
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
    public bool SearchForPlayer(){
        if (Physics2D.Linecast(transform.position, leftBound.position, playerMask) || Physics2D.Linecast(transform.position, rightBound.position, playerMask)){ //Bound of platform check
            Debug.DrawLine(transform.position, rightBound.position, Color.magenta);
            Debug.DrawLine(transform.position, leftBound.position, Color.magenta);
            return true;
        }else{
            return false;
        }
    }

    public void moveEnemy(Transform objective){
        float step = speed * Time.deltaTime;
        animatorSetting();
        transform.position = Vector2.MoveTowards(transform.position, objective.position, step);
    }

    public bool checkBoundDistance(Transform bound) {
        if((Vector2.Distance(bound.position,transform.position)) <= precisionOfEnemyToBounds) {
            return true;
        }else {
            return false;
        }
    }

    public void patrol() {
        //walk to the bound indicated
        if (walkingToLeftBound) {
            if (checkBoundDistance(leftBound)) {
                //start walking to the right
                walkingToLeftBound = false;
                moveEnemy(rightBound);
            } else {
                //keep walking the same direction
                walkingToLeftBound = true;
                moveEnemy(leftBound);
            }
        } else {
            if (checkBoundDistance(rightBound)) {
                //start walking to the left
                walkingToLeftBound = true;
                moveEnemy(leftBound);
            } else {
                //keep walking the same direction
                walkingToLeftBound = false;
                moveEnemy(rightBound);
            }
        }
    }

    public void animatorSetting() {
        if(!searchingPlayer) {
            //patrolling the platform
            //player is not in sight
            if (walkingToLeftBound) {
                animator.SetInteger("Sdirection", 2);
            } else {
                animator.SetInteger("Sdirection", 1);
            }
        }else {
            //going to chase the player
            //1st come to a stand still then chase
            if (walkingToLeftBound) {
                animator.SetInteger("Sdirection", 3);
            } else {
                animator.SetInteger("Sdirection", 0);
            }

        }
    }
}
