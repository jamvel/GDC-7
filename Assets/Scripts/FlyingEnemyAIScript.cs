using UnityEngine;
using System.Collections;

public class FlyingEnemyAIScript : MonoBehaviour {

    public Transform target;
    public LayerMask playerMask;

    public Transform topRightBound;
    public Transform topLeftBound;
    public Transform bottomRightBound;
    public Transform bottomLeftBound;

    public GameObject fireball; //prefab to fireball
    public Vector2 fireballVector;
    public GameObject emmiterFireball; //actual emmiter

    public float movementSpeed = 0.8f;
    public float projectileRange;
    public float dashRange;
    public float enemyStopDistance;
    public float changeDirectionInSeconds = 2f;
    public float precisionOfEnemyToBounds = 0.26f;
    public Vector2[] possiblePositions;

    public float fireRate;

    private float distanceToPlayer;
    private Rigidbody2D rigidBody;
    private bool searchingPlayer = false;
    private bool isChasing;
    private bool isAttacking;

    private bool isWalkingRight;
    private bool isWalkingLeft;
    private bool isWalkingUp;
    private bool isWalkingDown;

    private float time;


    private Animator animator;
    private float nextFire;
    private Vector2 previousPosition;
    private Vector2 currentPosition;

    // Use this for initialization
    void Start () {
        animator = this.GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        target = GameObject.FindWithTag("Player").transform;
        time = 0;
    }

    // Update is called once per frame
    void Update () {
        distanceToPlayer = Vector2.Distance(target.position, transform.position);
        
        animatorSetting();
        if (SearchForPlayer()) {//player is in moving bounds of the enemy
            if (distanceToPlayer > projectileRange) {
                //out of range
                //roam randomly in box
                isChasing = false;
                isAttacking = false;
                randomMovement();
            } else if ((distanceToPlayer <= projectileRange) && (distanceToPlayer > dashRange)) {
                //look at the player
                //shoot projectiles
                isChasing = false;
                isAttacking = false;
                //dont move and shoot projectile
                //shootProjectile();
            } else if ((distanceToPlayer <= dashRange) && (distanceToPlayer > enemyStopDistance)) {
                //start running after the enemy
                //and dash into him
                isChasing = true;
                isAttacking = false;
                dashToPlayer(target);
            } else if (distanceToPlayer <= enemyStopDistance) {
                isChasing = false;
                isAttacking = true;
                dashToPlayer(target);
            } else {
                Debug.Log(("Undefined State"));
            }
        }else {
            isChasing = false;
            isAttacking = false;
            //patrol and throw bolts
            if (distanceToPlayer > projectileRange) {
                //keep roaming
                Debug.Log("Move to point");
                randomMovement();
            } else if ((distanceToPlayer <= projectileRange) && (distanceToPlayer > dashRange)) {
                //shoot projectile
                //shootProjectile();
            }
        }
    }

    //player is in ghost bounds
    public bool SearchForPlayer() {
        if ((Physics2D.Linecast(transform.position, topLeftBound.position, playerMask)) || (Physics2D.Linecast(transform.position, topRightBound.position, playerMask)) ||
            (Physics2D.Linecast(transform.position, bottomLeftBound.position, playerMask)) || (Physics2D.Linecast(transform.position, bottomRightBound.position, playerMask)) ||
            (Physics2D.Linecast(topRightBound.position, topLeftBound.position, playerMask)) || (Physics2D.Linecast(bottomRightBound.position, topRightBound.position, playerMask))||
            (Physics2D.Linecast(bottomRightBound.position, bottomLeftBound.position, playerMask)) || (Physics2D.Linecast(bottomLeftBound.position, topLeftBound.position, playerMask))) { //Bound of platform check
            
            //Debug.DrawLine(topLeftBound.position, topRightBound.position, Color.magenta);
            //Debug.DrawLine(topRightBound.position, bottomRightBound.position, Color.magenta);
            //Debug.DrawLine(bottomRightBound.position, bottomLeftBound.position, Color.magenta);
            //Debug.DrawLine(bottomLeftBound.position, topLeftBound.position, Color.magenta);
            return true;
        } else {
            return false;
        }
    }

    public void randomMovement() {
        time += Time.deltaTime;
        if (time > changeDirectionInSeconds) {//every second possibly change direction
            time = 0;
            previousPosition = currentPosition;
            currentPosition = nextPoint();
            do {
                currentPosition = nextPoint();
                moveEnemy(currentPosition);
            } while (currentPosition == previousPosition);

        } else {
            moveEnemy(currentPosition);
        }
    }

    public void moveEnemy(Vector2 pos) {
        float step = movementSpeed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, pos, step);
    }

    /*
    if(transform.position == A.position) {
        //go to B, or C or startPosition
    }else if(transform.position == B.position) {
        //go to C, or A or startPosition
    }else if(transform.position == C.position){
        //go to A, or B or startPosition
    }else {
        //go to A, or B or C
    }
    */

    /*
    if (isWalkingLeft) {//is walking left
        if(isWalkingDown) {
            if (checkBoundDistance(bottomLeftBound)) {
                //start walking to the right
                isWalkingDown = true;
                isWalkingLeft = true;
                isWalkingRight = false;
                isWalkingUp = false;
        //        moveEnemy(bottomLeftBound);
            } else {
                //keep walking the same direction
        //        walking = true;
        //        walkingToLeftBound = true;
        //        moveEnemy(leftBound);
            }
        } else {

        }

    }
    //check bound limit
        //if out of bounds go back to bounds

    //change direction --> random
}
*/

    public Vector2 nextPoint() {
        return possiblePositions[Random.Range(0, possiblePositions.Length)];
    }

    public bool checkBoundDistance(Transform bound) {
        if ((Vector2.Distance(bound.position, transform.position)) <= precisionOfEnemyToBounds) {
            return true;
        } else {
            return false;
        }
    }

    public void shootProjectile() {
        if (Time.time > nextFire) {
            nextFire = Time.time + fireRate;

            fireball.GetComponent<Projectile>().velocityVector = fireballVector + emmiterFireball.GetComponent<EmmiterVector>().directionVector; //speed + dir
            Instantiate(fireball, emmiterFireball.GetComponent<Transform>().position, emmiterFireball.GetComponent<Transform>().rotation);
            
            //Shoot = Instantiate(fireball, transform.position, transform.rotation);
            //Shoot.AddForce(transform.forward * 5000);

            //clone = Instantiate(projectile, transform.position, transform.rotation);
            //clone.velocity = transform.TransformDirection(Vector3(0, 0, speed));
        }
    }

    /*
    void FireEnemyBullet() {
        GameObject player = GameObject.Find("Player");
        if (player != null) {

            GameObject bullet = (GameObject)Instantiate(fireball);
            bullet.transform.position = transform.position;

            Vector2 direction = player.transform.position - bullet.transform.position;
           // bullet.GetComponent<FlyingEnemyAIScript>().SetDirection(direction);
        }
    }
    */

    public void dashToPlayer(Transform objective) {
        float step = movementSpeed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, objective.position, step);
    }

    public void rotateGhost() {
        var lookPos = target.position - transform.position;
        //lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        rotation.y = 0;
        rotation.x = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 1);
        //rotation.y = 0;
        //rotation.x = 0;
    }



    public void animatorSetting() {
        var relativePoint = transform.InverseTransformPoint(target.position);
        rotateGhost();
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
