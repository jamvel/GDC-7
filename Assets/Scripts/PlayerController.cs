using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public float speed = 10f;
	public float jumpVelocity = 10f;
    public LayerMask playerMask;
	public GameObject tagGroundLeft,tagGroundRight;
	public bool airControl = true;

    public AudioClip[] effects;
    public AudioSource source;

    private Transform playerTransform,tagLeftTransform, tagRightTransform;
	private Rigidbody2D playerRigidBody;
	private bool isGround = false;
    private Animator animator;

    void Start(){
		playerTransform = this.GetComponent<Transform> ();
		tagLeftTransform = tagGroundLeft.GetComponent<Transform> ();
		tagRightTransform = tagGroundRight.GetComponent<Transform> ();
		playerRigidBody = this.GetComponent<Rigidbody2D> ();
        animator = this.GetComponent<Animator>();
        source = gameObject.AddComponent<AudioSource>();
        source.clip = effects[0];
    }

	void Update(){
        //Horizontal Movement
        var horizontal = Input.GetAxis("Horizontal");
        Move(horizontal);
        if (horizontal > 0){//walk right
            animator.SetInteger("Direction", 1);
            WalkSound();
        }else if (horizontal < 0){//walk left
            animator.SetInteger("Direction", 2);
            WalkSound();
        }else{//not moving
            if(animator.GetInteger("Direction") == 1) {
                animator.SetInteger("Direction", 0);
            }
            else if(animator.GetInteger("Direction") == 2) {
                animator.SetInteger("Direction", 3);
            }
        }
        
        if (Input.GetButtonDown("Jump")){ //vertical
            if (isGround){
                AudioSource.PlayClipAtPoint(effects[1], transform.position);
                playerRigidBody.velocity = jumpVelocity * Vector2.up;
			}
		}

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
            if (!source.isPlaying){
                source.Play();
            }
        }
    }
}
