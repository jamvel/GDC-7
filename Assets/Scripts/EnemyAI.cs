﻿using UnityEngine;
using System.Collections;

/*
 * Enemy AI class
 * This class targets the player only if the player is on the platform that user is on
 * Otherwise if the enemy is not on the platform the enemy will move from one bound to another
 * 
 */
public class EnemyAI : MonoBehaviour {
    public Transform target;
    public Transform rightBound;
    public Transform leftBound;
    public LayerMask playerMask;

    public float speed = 1.0f;
    public float inChasingDistance = 0.75f;
    public float enemyStopDistance = 0.45f;

    public float stop = 0.4f;
    public float precisionOfEnemyToBounds = 0.35f;
    public float timeToWait = 2f;

    private float waitBetweenAttack = 300f;
    private float currentWaitTime;

    public float enemyWalkAgain = 0.6f;

    public AudioClip[] effects;
    private AudioSource skelwalk, skelsword;
    public bool enableAudio = true;

    private float distanceToPlayer = 0;

    private Rigidbody2D rigidBody;
    private bool searchingPlayer = false;
    private bool walkingToLeftBound = true;
    private bool isWalking = true;
    private bool isChasing = false;
    private bool isAttacking = false;

    private Animator animator;

    //private Vector2 startCoordinates;
    private Vector2 currentPosition;
    private Vector2 higherLeftBound;
    private Vector2 higherRightBound;
    private bool coStarted = false;

    void Start() {
        currentWaitTime = 300;//on detaction attack player immediately
        animator = this.GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        target = GameObject.FindWithTag("Player").transform;

        higherLeftBound = new Vector2(leftBound.position.x, leftBound.position.y + 0.5f);
        higherRightBound = new Vector2(rightBound.position.x, rightBound.position.y + 0.5f);

        //startCoordinates = GetComponent<Transform>().position;

        if (effects.Length > 0 && enableAudio == true) {
            skelwalk = gameObject.AddComponent<AudioSource>();
            skelwalk.clip = effects[0];
            skelwalk.spatialBlend = 1;
            skelsword = gameObject.AddComponent<AudioSource>();
            skelsword.clip = effects[1];
        } else if (effects.Length == 0) {
            enableAudio = false;
        }
    }


    void Update() {
        distanceToPlayer = Vector3.Distance(transform.position, target.position);
        if (enableAudio) {
            if (skelwalk.isPlaying && (!isWalking)) {
                skelwalk.Stop(); //stop walking sound if stopped moving
            }
        }

        if (SearchForPlayer()) { //player is in the same platform as skeleton
            var relativePoint = transform.InverseTransformPoint(target.position);
            if (relativePoint.x < 0.0) {//chasing to the left
                walkingToLeftBound = true;
            } else {
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
                    transform.position = Vector2.MoveTowards(transform.position, currentPosition, 0);
                    if (!coStarted) {
                        StartCoroutine(WaitSeconds());
                        coStarted = true;
                        animatorSetting();
                    }

                } else {
                    isWalking = true;
                    isAttacking = false;
                    patrol();
                }
            } else if ((distanceToPlayer <= inChasingDistance) && (distanceToPlayer > enemyStopDistance)) {
                //start running after the enemy
                currentWaitTime = 275;
                isChasing = true;
                searchingPlayer = true;
                if (isAttacking) {//check if enemy was attacking
                    transform.position = Vector2.MoveTowards(transform.position, currentPosition, 0);
                    if (distanceToPlayer > enemyWalkAgain) { //check how far the player got to walk again
                        if (!coStarted) {
                            StartCoroutine(WaitSeconds());
                            coStarted = true;
                            animatorSetting();
                        }
                    }

                } else {
                    isWalking = true;
                    moveEnemy(target);
                }
            } else if (distanceToPlayer <= enemyStopDistance) {
                //come to a stop     
                //start attacking
                positionToAttackIn();
                coStarted = false;
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
                //Debug.Log(("Undefined State"));
            }
        } else { // player is on a different platform from user --- patrol function
            searchingPlayer = false;
            isChasing = false;
            if (isAttacking) {//check if enemy was attacking
                transform.position = Vector2.MoveTowards(transform.position, currentPosition, 0);
                if (!coStarted) {
                    StartCoroutine(WaitSeconds());
                    coStarted = true;
                    animatorSetting();
                }

            } else {
                isWalking = true;
                isAttacking = false;
                animatorSetting();
                patrol();
            }
        }
        animatorSetting();
    }

    IEnumerator WaitSeconds() {
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
        isWalking = false;
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
        if ((Vector2.Distance(bound.position, transform.position)) <= precisionOfEnemyToBounds) {
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
        if (enableAudio && !skelsword.isPlaying) {
            skelsword.Play();
        }
        //Debug.Log("Attacking");
        //check if collider hit with player
        //deal damage to player

        //wait for seconds to make another sword swing
    }

    IEnumerator wait() {
        //Debug.Log("Before :" + Time.time);
        yield return new WaitForSeconds(waitBetweenAttack);
        //Debug.Log("After :" + Time.time);
    }

    public void animatorSetting() {
        if (!searchingPlayer) {//patrolling the platform
            //player is not in sight
            animator.SetBool("IsAttack", false);

            if (walkingToLeftBound) {//walking left
                animator.SetInteger("Sdirection", 2);
            } else {//walking right
                animator.SetInteger("Sdirection", 1);
            }
        } else {//going to do actions as regards to the player position
            var relativePoint = transform.InverseTransformPoint(target.position);
            if (isChasing) {//chasing enemy
                if (relativePoint.x < 0.0) {//chasing to the left
                    animator.SetInteger("Sdirection", 2);
                } else {//chaing to the right
                    animator.SetInteger("Sdirection", 1);
                }
            } else {//looking at enemy // soon to attack
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
            if (!skelwalk.isPlaying) {
                skelwalk.Play();
            }
        }
    }
}