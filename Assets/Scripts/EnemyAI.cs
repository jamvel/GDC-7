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

    //public float updateRate = 5f;
    public float speed = 0.8f;
    public float inChasingDistance = 1f;
    public float inSightDistance = 2f;
    public float enemyStopDistance = 0.45f;

    public float stop = 0.4f;
    public float precisionOfEnemyToBounds = 0.26f;

	public AudioClip[] effects;
	private AudioSource skelwalk, skelsword;
	public bool enableAudio = true;

    private float distanceToPlayer = 0;
    
    private Rigidbody2D rigidBody;
    private bool searchingPlayer = false;
    private bool walkingToLeftBound = true;
    private bool walking = true;
    private bool isChasing = false;
    private bool isAttacking = false;

    private Animator animator;

    private Vector2 startCoordinates;
    
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

        float leftDistance = transform.position.x - leftBound.position.x;
        float rightDistance = transform.position.x - rightBound.position.x;
        float result = (leftDistance - rightDistance)/2;

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

		if(effects.Length > 0 && enableAudio == true){
			skelwalk = gameObject.AddComponent<AudioSource>();
			skelwalk.clip = effects[0];
            skelwalk.spatialBlend = 1;
            skelsword = gameObject.AddComponent<AudioSource>();
            skelsword.clip = effects[1];
        }
        else if(effects.Length == 0){
			enableAudio = false;
		}
    }


    void Update() {
        distanceToPlayer = Vector3.Distance(transform.position, target.position);
        if(enableAudio){
			if (skelwalk.isPlaying && (!walking)){
				skelwalk.Stop(); //stop walking sound if stopped moving
			}
		}

        if (SearchForPlayer()){ //player is in the same platform as skeleton
            //move towards the player
            searchingPlayer = true;
            animatorSetting();
            if (distanceToPlayer > inSightDistance) {//out of range
                walking = true;
                searchingPlayer = false;
                isAttacking = false;
                isChasing = false;
                patrol();
            } else if ((distanceToPlayer <= inSightDistance) && (distanceToPlayer > inChasingDistance)) {
                //look at the player
                //do not move
                walking = false;
                isChasing = false;
                isAttacking = false;
                transform.position = Vector2.MoveTowards(transform.position, transform.position, 0);
            } else if((distanceToPlayer <= inChasingDistance) && (distanceToPlayer > enemyStopDistance)) {
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
    }

    //check if player is in the same platform as the enemy
    public bool SearchForPlayer(){
        if (Physics2D.Linecast(transform.position, leftBound.position, playerMask) || Physics2D.Linecast(transform.position, rightBound.position, playerMask)){ //Bound of platform check
            //Debug.DrawLine(transform.position, rightBound.position, Color.magenta);
            //Debug.DrawLine(transform.position, leftBound.position, Color.magenta);
            return true;
        }else{
            return false;
        }
    }

    public void moveEnemy(Transform objective){
        WalkSound();
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
                walking = true;
                walkingToLeftBound = false;
                moveEnemy(rightBound);
            } else {
                //keep walking the same direction
                walking = true;
                walkingToLeftBound = true;
                moveEnemy(leftBound);
            }
        } else {
            if (checkBoundDistance(rightBound)) {
                //start walking to the left
                walking = true;
                walkingToLeftBound = true;
                moveEnemy(leftBound);
            } else {
                //keep walking the same direction
                walking = true;
                walkingToLeftBound = false;
                moveEnemy(rightBound);
            }
        }
    }

    public void attack() {
        isAttacking = true;
        animatorSetting();
        if (enableAudio && !skelsword.isPlaying){
            skelsword.Play();
        }

        //yield return new WaitForSeconds(attackLag);
        //deal damage to player
//        isAttacking = false;
    }

    public void animatorSetting() {
        if(!searchingPlayer) {//patrolling the platform
            //player is not in sight
            if (walkingToLeftBound) {//walking left
                animator.SetInteger("Sdirection", 2);
            } else {//walking right
                animator.SetInteger("Sdirection", 1);
            }
        }else {//going to do actions as regards to the player position
            var relativePoint = transform.InverseTransformPoint(target.position);
            if (isChasing) {//chasing enemy
                if (relativePoint.x < 0.0) {//chasing to the left
                    animator.SetInteger("Sdirection", 2);
                } else {//chaing to the right
                    animator.SetInteger("Sdirection", 1);
                }
            }else {//looking at enemy // soon to attack
                if(isAttacking) {//attacking 
                    animator.SetBool("IsAttack", true);
                }else {//looking at player
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
		
	public void WalkSound(){
		if(enableAudio){
			if (!skelwalk.isPlaying){
				skelwalk.Play();
			}
		}
	}
}