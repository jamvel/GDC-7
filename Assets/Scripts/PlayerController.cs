using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
	public float speed = 10f;
	public float jumpVelocity = 10f;
    public LayerMask playerMask;
	public GameObject tagGroundLeft,tagGroundRight;
	public bool airControl = true;
    public bool invertedControls = false;
    public bool isFalling = false;

	public AudioClip[] effects;
    private AudioSource walk, sword, ballfire;
    public bool enableAudio = true;

	public GameObject fireball; //prefab to fireball
	public GameObject emmiterFireball; //actual emmiter

    private EmmiterVector ev_fireball;
    private Transform playerTransform,tagLeftTransform, tagRightTransform;
	private Rigidbody2D playerRigidBody;
    public bool isGround = false;
    private bool toLand = false;
    private bool direct = true; // flag used to check which direction the player is looking at (left or right horizontally)
    private Animator animator;
    private Player player;

    void Start(){
		playerTransform = this.GetComponent<Transform> ();
		tagLeftTransform = tagGroundLeft.GetComponent<Transform> ();
		tagRightTransform = tagGroundRight.GetComponent<Transform> ();
		playerRigidBody = this.GetComponent<Rigidbody2D> ();
        animator = this.GetComponent<Animator>();
        ev_fireball = emmiterFireball.GetComponent<EmmiterVector>();
        player = this.gameObject.GetComponent<Player>();

        if (effects.Length > 0 && enableAudio == true){
			walk = gameObject.AddComponent<AudioSource>();
			walk.clip = effects[0];
			walk.volume = 0.5f;
			sword = gameObject.AddComponent<AudioSource>();
			sword.clip = effects[3];
            ballfire = gameObject.AddComponent<AudioSource>();
            ballfire.clip = effects[4];
            ballfire.volume = 0.1f;
        }
        else if(effects.Length == 0){
			enableAudio = false;
		}
        
    }

	void Update(){
        //check for ground every frame
        if (Physics2D.Linecast(playerTransform.position, tagLeftTransform.position, playerMask) ||
            Physics2D.Linecast(playerTransform.position, tagRightTransform.position, playerMask)) { //Ground Check
            isGround = true;
        } else {
            isGround = false;
        }

        //Horizontal Movement
        if (playerRigidBody.velocity.y < -0.5) {
            isFalling = true;
        } else {
            isFalling = false;
        }

        if (!isGround){
            toLand = true;
        }

        if (toLand && isGround){
            if (enableAudio){
                AudioSource.PlayClipAtPoint(effects[2], transform.position,0.25f);
            }
            toLand = false;
        }
        var horizontal = Input.GetAxis("Horizontal");
       
        if (horizontal > 0){//walk right
            Move(horizontal);
            if (invertedControls) {
                direct = false;
                animator.SetInteger("Direction", 2);
            }else {
                direct = true;
                animator.SetInteger("Direction", 1);
            }
            WalkSound();
        }else if (horizontal < 0){//walk left
            Move(horizontal);
            if (invertedControls) {
                direct = true;
                animator.SetInteger("Direction", 1);
            }
            else {
                direct = false;
                animator.SetInteger("Direction", 2);
            }
            WalkSound();
        }else{//not moving
            playerRigidBody.velocity = new Vector2(0, playerRigidBody.velocity.y); //update to fix player moving slowly without input
            if (direct){//look right
                animator.SetInteger("Direction", 0);
            }else if (!direct) {//look left
                animator.SetInteger("Direction", 3);
            }
        }

		if(enableAudio){
			if ((walk.isPlaying && horizontal == 0) || (walk.isPlaying && !isGround) /*|| sword.isPlaying*/){
				walk.Stop(); //stop walking sound if stopped moving or in air
			}
		}

        if (Input.GetButtonDown("Jump")){ //vertical
            if (isGround){
				if(enableAudio){
					AudioSource.PlayClipAtPoint(effects[1], transform.position);
				}
                animator.SetBool("IsJump", true);
                playerRigidBody.velocity = jumpVelocity * Vector2.up;
			}
		}else {

            if ((Physics2D.Linecast(playerTransform.position, tagLeftTransform.position, playerMask))){
                if ((Physics2D.Linecast(playerTransform.position, tagRightTransform.position, playerMask))) { //Ground Check
                    animator.SetBool("IsJump", false);
                } else {
                    animator.SetBool("IsJump", false);
                }
            } else if ((Physics2D.Linecast(playerTransform.position, tagRightTransform.position, playerMask))) { //Ground Check
                    if ((Physics2D.Linecast(playerTransform.position, tagLeftTransform.position, playerMask))) {
                        animator.SetBool("IsJump", false);
                    } else {
                        animator.SetBool("IsJump", false);
                    }
            }else {
                animator.SetBool("IsJump", true);
            }
        }

		if(Input.GetButton("Fire1")){
            animator.SetBool("IsAttack", true);
            if (enableAudio && !sword.isPlaying){
                sword.volume = 0.35f;
                sword.Play();
            }
		}else{
            animator.SetBool("IsAttack", false);
        }

        //not sure when to include the statement below
        //to go back to the walking animation
        //animator.SetBool("IsAttack", false);

		if(Input.GetButtonDown("Fire2")){
            //Debug.Log ("Projectile Fire");
            //animator.SetBool("IsShoot", true);
            //fireball.GetComponent<Projectile> ().velocityVector = ev_fireball.magnitude * ev_fireball.directionVector; //speed * dir
            //Debug.Log(ev_fireball.magnitude* ev_fireball.directionVector);
            if (player.fireball_1) {
                if (direct) {
                    ballfire.Play();
                    ev_fireball.directionVector = new Vector2(1, 0);
                    fireball.GetComponent<Projectile>().velocityVector = ev_fireball.magnitude * ev_fireball.directionVector; //speed * dir
                    Debug.Log(fireball.GetComponent<Projectile>().velocityVector);
                    Instantiate(fireball, emmiterFireball.GetComponent<Transform>().position, Quaternion.Euler(0, 0, 0));
                }
                else {
                    ballfire.Play();
                    ev_fireball.directionVector = new Vector2(-1, 0);
                    fireball.GetComponent<Projectile>().velocityVector = ev_fireball.magnitude * ev_fireball.directionVector; //speed * dir
                    Debug.Log(fireball.GetComponent<Projectile>().velocityVector);
                    Instantiate(fireball, emmiterFireball.GetComponent<Transform>().position, Quaternion.Euler(0, 180, 0));
                }
            }
           
		}else {
            //animator.SetBool("IsShoot", false);
        }
    }

    /*
    public void FixedUpdate (){
        //Debug.DrawLine(playerTransform.position,tagLeftTransform.position);
		//Debug.DrawLine(playerTransform.position,tagRightTransform.position);
        if (Physics2D.Linecast(playerTransform.position, tagLeftTransform.position, playerMask) ||
            Physics2D.Linecast(playerTransform.position, tagRightTransform.position, playerMask)) { //Ground Check
            isGround = true;
        } else {
            isGround = false;
        }
        //OnTriggerStay();
        //OnTriggerExit();
	}
    */

    public void Move(float horizontalInput){
        if (!airControl && !isGround){
            return;
		}
        if (invertedControls) {
            horizontalInput = -horizontalInput;
        }
        Vector2 moveVelocity = playerRigidBody.velocity;
		moveVelocity.x = horizontalInput * speed;
		playerRigidBody.velocity = moveVelocity;
	}

    public void playerDeath() {
        playerRigidBody.velocity = new Vector2(0, 0);
    }

    public void WalkSound(){
		if(enableAudio){
			if (isGround){
				if (!walk.isPlaying){
					walk.Play();
				}
			}
		}
    }
}
