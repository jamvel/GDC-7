using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public float speed = 10f;
	public float jumpVelocity = 10f;
    public LayerMask playerMask;
	public GameObject tagGroundLeft,tagGroundRight;
	public bool airControl = true;

    public AudioClip[] effects;
    public AudioSource walk, sword;

    private Transform playerTransform,tagLeftTransform, tagRightTransform;
	private Rigidbody2D playerRigidBody;
	private bool isGround = false;
    private bool direct = true; // flag used to check which direction the player is looking at (left or right horizontally)
    private Animator animator;


    void Start(){
		playerTransform = this.GetComponent<Transform> ();
		tagLeftTransform = tagGroundLeft.GetComponent<Transform> ();
		tagRightTransform = tagGroundRight.GetComponent<Transform> ();
		playerRigidBody = this.GetComponent<Rigidbody2D> ();
        animator = this.GetComponent<Animator>();
        walk = gameObject.AddComponent<AudioSource>();
        walk.clip = effects[0];
        walk.volume = 0.5f;
        sword = gameObject.AddComponent<AudioSource>();
        sword.clip = effects[2];
    }

	void Update(){
        //Horizontal Movement
        var horizontal = Input.GetAxis("Horizontal");
        Move(horizontal);
        if (horizontal > 0){//walk right
            direct = true;
            animator.SetInteger("Direction", 1);
            WalkSound();
        }else if (horizontal < 0){//walk left
            direct = false;
            animator.SetInteger("Direction", 2);
            WalkSound();
        }else{//not moving
            if (direct){//look right
                animator.SetInteger("Direction", 0);
            }else if (!direct) {//look left
                animator.SetInteger("Direction", 3);
            }
        }

        if ((walk.isPlaying && horizontal == 0) || (walk.isPlaying && !isGround) || sword.isPlaying){
            walk.Stop(); //stop walking sound if stopped moving or in air
        }

        if (Input.GetButtonDown("Jump")){ //vertical
            if (isGround){
                AudioSource.PlayClipAtPoint(effects[1], transform.position);
                playerRigidBody.velocity = jumpVelocity * Vector2.up;
			}
		}

		if(Input.GetButtonDown("Fire1")){
            animator.SetBool("IsAttack", true);
            if (!sword.isPlaying){
                sword.Play();
            }
            Debug.Log ("attack");
		}

        //not sure when to include the statement below
        //to go back to the walking animation
        //animator.SetBool("IsAttack", false);
    }

	void FixedUpdate (){
		Debug.DrawLine(playerTransform.position,tagLeftTransform.position);
		Debug.DrawLine(playerTransform.position,tagRightTransform.position);
		if(Physics2D.Linecast(playerTransform.position,tagLeftTransform.position,playerMask) || 
			Physics2D.Linecast(playerTransform.position,tagRightTransform.position,playerMask)){ //Ground Check
			isGround = true;
		}else{
			isGround = false;
		}
	}

	public void Move(float horizontalInput){
		if(!airControl && !isGround){
            return;
		}
        Vector2 moveVelocity = playerRigidBody.velocity;
		moveVelocity.x = horizontalInput * speed;
		playerRigidBody.velocity = moveVelocity;
	}

   public void WalkSound(){
        if (isGround){
            if (!walk.isPlaying){
                walk.Play();
            }
        }
    }
}
