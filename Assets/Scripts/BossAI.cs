using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour {
    public Transform target;
    public Transform rightBound;
    public Transform leftBound;
    public LayerMask playerMask;

    public float speed = 1.0f;
    public float inChasingDistance = 0.75f;
    public float enemyStopDistance = 0.45f;
    public float fireTime = 5f;//number of frames to wait before shooting again
    public float projectileSpeed = 2f;

    public float stop = 0.4f;

    private float waitBetweenAttack = 300f;
    private float currentWaitTime;
    private int waitFrameAttack;
    private int waitFrameMagic = 60;

    public float enemyWalkAgain = 0.6f;
    public float enemyShootRange = 3f;

    private EmmiterVector ev_fireball;
    public GameObject fireball; //prefab to fireball
    public Vector2 fireballVector;
    public GameObject emmiterFireball; //actual emmiter

    public AudioClip[] bossEffects;
    private AudioSource bosswalk, bosssword;
    public bool enableAudio = true;

    private float distanceToPlayer = 0;

    private Rigidbody2D rigidBody;
    private bool searchingPlayer = false;
    private bool walkingToLeftBound = true;
    private bool isWalking = true;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isMagic = false;

    private Animator animator;

    //private Vector2 startCoordinates;
    private Vector2 currentPosition;
    private Vector2 higherLeftBound;
    private Vector2 higherRightBound;
    private GameObject emitter;


    void Start() {
        currentWaitTime = 300;//on detaction attack player immediately
        animator = this.GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        target = GameObject.FindWithTag("Player").transform;

        higherLeftBound = new Vector2(leftBound.position.x, leftBound.position.y + 0.5f);
        higherRightBound = new Vector2(rightBound.position.x, rightBound.position.y + 0.5f);
        ev_fireball = emmiterFireball.GetComponent<EmmiterVector>();
        emitter = this.gameObject.transform.GetChild(0).gameObject;

        //startCoordinates = GetComponent<Transform>().position;

        if (bossEffects.Length > 0 && enableAudio == true) {
            bosswalk = gameObject.AddComponent<AudioSource>();
            bosswalk.clip = bossEffects[0];
            bosswalk.spatialBlend = 1;
            bosssword = gameObject.AddComponent<AudioSource>();
            bosssword.clip = bossEffects[1];
        } else if (bossEffects.Length == 0) {
            enableAudio = false;
        }
    }


    void Update() {
        distanceToPlayer = Vector3.Distance(transform.position, target.position);
        if (enableAudio) {
            if (bosswalk.isPlaying && (!isWalking)) {
                bosswalk.Stop(); //stop walking sound if stopped moving
            }
        }

        if (SearchForPlayer()) { //player is in the same platform as skeleton
            //to change which bound enemy should go to once player is out of range
            var relativePoint = transform.InverseTransformPoint(target.position);
            if (relativePoint.x < 0.0) {//chasing to the left
                walkingToLeftBound = true;
            }else {
                walkingToLeftBound = false;
            }
            //move towards the player
            searchingPlayer = true;
            animatorSetting();

            if (distanceToPlayer > inChasingDistance) {//out of range
                //patrol the platform
                currentWaitTime = 300;
                searchingPlayer = false;
                isChasing = false;

                if (isAttacking) {//check if enemy was attacking
                    waitFrameAttack++;
                    transform.position = Vector2.MoveTowards(transform.position, currentPosition, 0);
                    if (waitFrameAttack > 55) { //wait for 50 frames, then move again towards the enemy
                        isAttacking = false;
                        isWalking = false;
                        waitFrameAttack = 0;
                    }
                } else {
                    //shoot while patrolling on same platform
                    waitFrameMagic++;
                    //if (distanceToPlayer < enemyPatrolRange) {//in shooting range                 
                    if (!isMagic) {
                        if (waitFrameMagic > fireTime) {
                            waitFrameMagic = 0;
                            isMagic = true;
                            isWalking = false;
                            shootProjectile();
                        }
                        isMagic = false;
                        isWalking = true;
                        patrol();
                    }
                    //} else {//keep walking until reaching shooting range or player is on same playform
                    //    isMagic = false;
                    //    isWalking = true;
                    //    patrol();
                    //}
                }
            } else if ((distanceToPlayer <= inChasingDistance) && (distanceToPlayer > enemyStopDistance)) {
                //start running after the enemy
                currentWaitTime = 275;
                isChasing = true;
                searchingPlayer = true;
                if (isAttacking) {//check if enemy was attacking
                    transform.position = Vector2.MoveTowards(transform.position, currentPosition, 0);
                    if (distanceToPlayer > enemyWalkAgain) { //check how far the player got to walk again
                        isAttacking = false;
                        isWalking = false;
                    }
                } else {
                    isWalking = true;
                    moveEnemy(target);
                }
            } else if (distanceToPlayer <= enemyStopDistance) {
                //come to a stop     
                //start attacking
                positionToAttackIn();
                if (currentWaitTime >= waitBetweenAttack) {
                    isWalking = false;
                    isChasing = false;
                    isAttacking = true;
                    searchingPlayer = true;
                    attack();
                    currentWaitTime = 0;
                } else {//wait for next attack
                    isWalking = false;
                    isChasing = false;
                    //isAttacking = true;
                    searchingPlayer = true;
                    currentWaitTime++;
                }
            } else {
                isWalking = false;
                isChasing = false;
                isAttacking = false;
                searchingPlayer = false;
                Debug.Log(("Undefined State"));
            }
        } else { // player is on a different platform from user --- patrol function
            searchingPlayer = false;
            isChasing = false;
            isAttacking = false;
            waitFrameMagic++;
            if (distanceToPlayer < enemyShootRange) {//in shooting range                 
                if (!isMagic) {
                    if (waitFrameMagic > fireTime) {
                        waitFrameMagic = 0;
                        isMagic = true;
                        isWalking = false;
                        shootProjectile();
                    }
                    isMagic = false;
                    isWalking = true;
                    patrol();
                }
            } else {//keep walking until reaching shooting range or player is on same playform
                isMagic = false;
                isWalking = true;
                patrol();
            }
        }
        animatorSetting();
    }

    public void shootProjectile() {
        var lookPosX = target.position.x - transform.position.x;
        var lookPosY = target.position.y - transform.position.y;
        var relativePoint = transform.InverseTransformPoint(target.position);

        if (walkingToLeftBound) {
            ev_fireball.directionVector = new Vector2(lookPosX, lookPosY);
            fireball.GetComponent<Projectile>().velocityVector = projectileSpeed * ev_fireball.directionVector; //speed * dir       
            if (relativePoint.x < 0.0) {//chasing to the left
                Instantiate(fireball, emmiterFireball.GetComponent<Transform>().position, Quaternion.Euler(0, 180, -rotateBolt()));
            } else {//chaing to the right
                Instantiate(fireball, emmiterFireball.GetComponent<Transform>().position, Quaternion.Euler(0, 0, rotateBolt()));
            }
        } else {
            ev_fireball.directionVector = new Vector2(lookPosX, lookPosY);
            fireball.GetComponent<Projectile>().velocityVector = ev_fireball.magnitude * ev_fireball.directionVector; //speed * dir
            if (relativePoint.x < 0.0) {//chasing to the left
                Instantiate(fireball, emmiterFireball.GetComponent<Transform>().position, Quaternion.Euler(180, 180, rotateBolt()));
            } else {//chaing to the right
                Instantiate(fireball, emmiterFireball.GetComponent<Transform>().position, Quaternion.Euler(0, 0, rotateBolt()));
            }

        }
    }

    public float rotateBolt() {
        var lookPos = target.position - transform.position;
        var rotation = Quaternion.LookRotation(lookPos);
        rotation.y = 0;
        rotation.x = 0;
        float angle = rotation.eulerAngles.z;
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 1);
        return angle;
    }

    public void positionToAttackIn() {
        currentPosition = transform.position;
    }

    //check if player is in the same platform as the enemy
    public bool SearchForPlayer() {
        if ((Physics2D.Linecast(transform.position, leftBound.position, playerMask) || Physics2D.Linecast(transform.position, rightBound.position, playerMask)) ||
            (Physics2D.Linecast(transform.position, higherLeftBound, playerMask) || Physics2D.Linecast(transform.position, higherRightBound, playerMask))) { //Bound of platform check
            return true;
        } else {
            return false;
        }
    }

    public void moveEnemy(Transform objective) {
        WalkSound();
        float step = speed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, objective.position, step);
    }

    public bool checkBoundDistance(Transform bound) {
        if ((Vector2.Distance(bound.position, transform.position)) <= stop) {
            return true;
        } else {
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

    public void attack() {
        if (enableAudio && !bosssword.isPlaying) {
            bosssword.Play();
        }
        //Debug.Log("Attacking");
        //check if collider hit with player
        //deal damage to player

        //wait for seconds to make another sword swing
    }

    IEnumerator wait() {
        Debug.Log("Before :" + Time.time);
        yield return new WaitForSeconds(waitBetweenAttack);
        Debug.Log("After :" + Time.time);
    }

    public void animatorSetting() {
        var relativePoint = transform.InverseTransformPoint(target.position);
        if (!searchingPlayer) {//patrolling the platform
            //player is not on same platform
            animator.SetBool("IsAttack", false);
            if(isMagic) {//shoot bolts to player
                animator.SetBool("IsMagic", true);
            } else {//just patrol the area
                animator.SetBool("IsMagic", false);
                if (walkingToLeftBound) {//walking left
                    animator.SetInteger("Sdirection", 2);
                } else {//walking right
                    animator.SetInteger("Sdirection", 1);
                }
            }
        } else {//going to do actions as regards to the player position
            if (isChasing) {//chasing enemy
                if (relativePoint.x < 0.0) {//chasing to the left
                    animator.SetInteger("Sdirection", 2);
                } else {//chaing to the right
                    animator.SetInteger("Sdirection", 1);
                }
            } else {//looking at enemy // soon to attack 



                //might need to change states below
                if (isAttacking) {//attacking 
                    animator.SetBool("IsAttack", true);
                } else {//looking at player
                    animator.SetBool("IsAttack", false);
                    if (relativePoint.x > 0.0) {//looking at the right
                        animator.SetInteger("Sdirection", 0);
                    } else {//looking at the left
                        animator.SetInteger("Sdirection", 3);
                    }
                }
            }
        }
    }

    public void WalkSound() {
        if (enableAudio) {
            if (!bosswalk.isPlaying) {
                bosswalk.Play();
            }
        }
    }
}

