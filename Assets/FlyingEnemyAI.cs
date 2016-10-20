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

    public Transform topRightBound;
    public Transform topLeftBound;
    public Transform bottomRightBound;
    public Transform bottomLeftBound;

    public float speed = 0.8f;
    public float rangeToAttack;
    public float precisionOfEnemyToBounds = 0.26f;

    private float xAxis;
    private float yAxis;
    private float time;
    private float distanceToPlayer;

    private Rigidbody2D rigidBody;
    private bool walkingToLeftBound = true;
    private bool walkingToTopBound = true;
    private bool isAttacking = false;

    private Animator animator;

    private Vector2 startCoordinates;

    void Start() {
        animator = this.GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        target = GameObject.FindWithTag("Player").transform;
        startCoordinates = GetComponent<Transform>().position;

        xAxis = Random.Range(-speed, speed);
        yAxis = Random.Range(-speed, speed);
        //Debug.Log("Distance to player: "+ distanceToPlayer);

        //transform.localRotation = Quaternion.Euler(0, angle, 0);

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
        //time += Time.deltaTime;
        var relativePoint = transform.InverseTransformPoint(target.position);
        //x^2 + y^2
        distanceToPlayer = Vector2.Distance(target.position, transform.position);
        Debug.Log("Distance to player: " + distanceToPlayer);

        if(inAttackRange()) {
            //start shooting at the enemy / dash to the player
            //dash to player
            moveEnemy(target);
        } else {
            //go patrol the boundaries
        }


        if (walkingToLeftBound) {//walking to the left
            if(walkingToTopBound) {

            }else {

            }
        }else {//walking to the right
            if(walkingToTopBound) {

            }else {

            }
        }
        

    }

    public void moveEnemy(Transform objective) {
        float step = speed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, objective.position, step);
    }
    
    public bool inAttackRange() {
        if(distanceToPlayer < rangeToAttack ) {
            //can start attacking
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