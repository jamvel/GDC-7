using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public float speed = 10f;
	public float jumpVelocity = 10f;
    public LayerMask playerMask;
	public GameObject tagGroundLeft,tagGroundRight;
	public bool airControl = true;

	private Transform playerTransform,tagLeftTransform, tagRightTransform;
	private Rigidbody2D playerRigidBody;
	private bool isGround = false;

	void Start(){
		playerTransform = this.GetComponent<Transform> ();
		tagLeftTransform = tagGroundLeft.GetComponent<Transform> ();
		tagRightTransform = tagGroundRight.GetComponent<Transform> ();
		playerRigidBody = this.GetComponent<Rigidbody2D> ();
	}

	void Update(){
		//Horizontal Movement
		Move (Input.GetAxisRaw ("Horizontal"));
		if(Input.GetButtonDown("Jump")){ //vertical
            if (isGround){
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
}