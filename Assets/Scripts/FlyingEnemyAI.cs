using UnityEngine;
using System.Collections;

/*
 * Enemy AI class
 * This class targets the player only if the player is on the platform that user is on
 * Otherwise if the enemy is not on the platform the enemy will move from one bound to another
 * 
 */
public class FlyingEnemyAI : MonoBehaviour {
    public Transform target;

    public LayerMask playerMask;

    public Transform topRightBound;
    public Transform topLeftBound;
    public Transform bottomRightBound;
    public Transform bottomLeftBound;

    public float width = 5; //Your square width

    public float radius = 1f;
    public float speed = 0.8f;
    public float rangeToAttackProjectile;
    public float rangeToAttackDash;
    public float enemyStopDistance;
    public float precisionOfEnemyToBounds = 0.26f;


    private float xAxis;
    private float yAxis;
    private float distanceToPlayer;

    private Rigidbody2D rigidBody;
    private bool searchingPlayer = false;
    private bool walking = false;
    private bool isChasing = false;
    private bool walkingToLeftBound = true;
    private bool walkingToTopBound = true;
    private bool isAttacking = false;
    
    private Animator animator;

    private Vector2 startCoordinates;
    private Vector2 targetPos;

    void Start() {
        animator = this.GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        target = GameObject.FindWithTag("Player").transform;
        startCoordinates = GetComponent<Transform>().position;

        xAxis = Random.Range(-speed, speed);
        yAxis = Random.Range(-speed, speed);
        //Debug.Log("Distance to player: "+ distanceToPlayer);

        /*
        float leftDistance = transform.position.x - bottomLeftBound.position.x;
        float rightDistance = transform.position.x - bottomRightBound.position.x;
        float topDistance = transform.position.y - topLeftBound.position.y;
        float bottomDistance = transform.position.y - bottomLeftBound.position.y;

        Debug.Log("Position x: " + transform.position.x + " position y: "+ transform.position.y);
        Debug.Log("Top Left Direction x: " + topLeftBound.position.x+ "Top Left Direction y: " + topLeftBound.position.y);
        Debug.Log("Top Right Direction x: " + topRightBound.position.x + "Top Right Direction y: " + topRightBound.position.y);
        Debug.Log("Bottom Left Direction x: " + bottomLeftBound.position.x + "Bottom Left Direction y: " + bottomLeftBound.position.y);
        Debug.Log("Bottom Right Direction x: " + bottomRightBound.position.x + "Bottom Right Direction y: " + bottomRightBound.position.y);
        */
    }

    void Update() {
        var relativePoint = transform.InverseTransformPoint(target.position);
        distanceToPlayer = Vector2.Distance(target.position, transform.position);

        if (SearchForPlayer()) { //player is in the same platform as skeleton
            //move towards the player
            searchingPlayer = true;
            animatorSetting();
            if (distanceToPlayer > rangeToAttackProjectile) {//out of range
                walking = true;
                searchingPlayer = false;
                isAttacking = false;
                isChasing = false;
                patrol();
            } else if ((distanceToPlayer <= rangeToAttackProjectile) && (distanceToPlayer > rangeToAttackDash)) {
                //look at the player
                //do not move
                walking = false;
                isChasing = false;
                isAttacking = false;
                transform.position = Vector2.MoveTowards(transform.position, transform.position, 0);
            } else if ((distanceToPlayer <= rangeToAttackDash) && (distanceToPlayer > enemyStopDistance)) {
                //start running after the enemy
                walking = true;
                isChasing = true;
                isAttacking = false;
                moveEnemy(target);
            } else if (distanceToPlayer <= enemyStopDistance) {
                //come to a stop     
                //start attacking
                walking = false;
                isChasing = false;
                animatorSetting();
                transform.position = Vector2.MoveTowards(transform.position, transform.position, 0);
                attack();
            } else {
                Debug.Log(("Undefined State"));
            }
        } else { // player is on a different platform from user --- patrol function
            searchingPlayer = false;
            patrol();
        }








        /*
        if(inAttackRange()) {
            //start shooting at the enemy / dash to the player
            //dash to player
            moveEnemy(target);
        } else {
            //go patrol the boundaries
            randomMovement();
        }
        */
    }



    public bool SearchForPlayer() {
        if ((Physics2D.Linecast(transform.position, topLeftBound.position, playerMask) || Physics2D.Linecast(transform.position, topRightBound.position, playerMask))&&((Physics2D.Linecast(transform.position, bottomLeftBound.position, playerMask) || Physics2D.Linecast(transform.position, bottomRightBound.position, playerMask)))) { //Bound of platform check
            //Debug.DrawLine(transform.position, rightBound.position, Color.magenta);
            //Debug.DrawLine(transform.position, leftBound.position, Color.magenta);
            return true;
        } else {
            return false;
        }
    }

    public void animatorSetting() {
        if (!searchingPlayer) {//patrolling the platform
            //player is not in sight
            if (walkingToLeftBound) {//walking left
                animator.SetInteger("Gdirection", 2);
            } else {//walking right
                animator.SetInteger("Gdirection", 1);
            }
        } else {//going to do actions as regards to the player position
            var relativePoint = transform.InverseTransformPoint(target.position);
            if (isChasing) {//chasing enemy
                if (relativePoint.x < 0.0) {//chasing to the left
                    animator.SetInteger("Gdirection", 2);
                } else {//chaing to the right
                    animator.SetInteger("Gdirection", 1);
                }
            } else {//looking at enemy // soon to attack
                if (isAttacking) {//attacking 
                    animator.SetBool("IsAttack", true);
                } else {//looking at player
                    animator.SetBool("IsAttack", false);
                    if (relativePoint.x > 0.0) {//looking at the right
                        animator.SetInteger("Gdirection", 0);
                    } else {//looking at the left
                        animator.SetInteger("Gdirection", 3);
                    }
                }
            }
        }
    }

    public void moveEnemy(Transform objective) {
        float step = speed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, objective.position, step);
    }
    
    public void patrol() {
        float rx = Random.Range(-1, 1);
        float rz = Random.Range(-1, 1);

        //targetPos = Vector2.MoveTowards()

        //targetPos = Vector2(transform.position.x+(rx* width), someYValue, transform.position.z+(rx* width));
    }

    public bool inAttackRange() {
        if(distanceToPlayer < rangeToAttackProjectile ) {
            //can start throwing projectile attacks
            return true;
        }else {
            //keep patrolling
            //bouncing between bounds
            return false;
        }
    }

    public bool checkBoundDistance(Transform bound) {
        if ((Vector2.Distance(bound.position, transform.position)) <= precisionOfEnemyToBounds) {
            return true;
        } else {
            return false;
        }
    }

    public void attack() {
        isAttacking = true;
        //deal damage to player
        isAttacking = false;
    }

}