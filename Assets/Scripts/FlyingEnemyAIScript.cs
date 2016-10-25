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

    public float damp;

    public float movementSpeed = 0.8f;
    public float projectileRange;
    public float dashRange;
    public float enemyStopDistance;
    public float precisionOfEnemyToBounds = 0.26f;

    public float fireRate;

    private float distanceToPlayer;
    private Rigidbody2D rigidBody;
    private bool searchingPlayer = false;
    private bool isChasing;
    private bool isAttacking;

    private Animator animator;
    private float nextFire;

    // Use this for initialization
    void Start () {
        animator = this.GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        target = GameObject.FindWithTag("Player").transform;

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
        //check when to rotate the ghost ...

        var lookPos = target.position - transform.position;
        //lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        rotation.y = 0;
        rotation.x = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 1);
        rotation.y = 0;
        rotation.x = 0;
        //transform.LookAt(target);
        //var lookPos = target.position - transform.position;
        //var rotation = Quaternion.LookRotation(lookPos);
        //lookPos.x = transform.localRotation.x;
        //lookPos.y = Mathf.PI/2.0f;
        //lookPos.z = transform.localRotation.z;
        //transform.localRotation = Quaternion.Euler(lookPos);


        //var lookPos = target.position - transform.position;
        //Debug.Log("Look Position: " + lookPos.x);
        //transform.Rotate(Vector3.forward * -lookPos.x);

        //0 to -90
        //if ((transform.rotation.z >= 0) && (transform.rotation.z < -90 )) {

        //} else {

        //}

        //make enemy rotate to flying enemy

        //var lookPos = target.position - transform.position;
        //lookPos.y = 0;
        //var rotation = Quaternion.LookRotation(lookPos);
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damp);

        //if (target) {
        //    var rotationAngle = Quaternion.LookRotation(target.position - transform.position); // we get the angle has to be rotated
        //    transform.rotation = Quaternion.Slerp(transform.rotation, rotationAngle, Time.deltaTime * damp); // we rotate the rotationAngle 
        //}




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
