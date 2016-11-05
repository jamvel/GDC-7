using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour {

    public Transform leftBound;
    public Transform rightBound;

    public float precisionOfPlatformToBounds = 0.35f;
    public float speedOfPlatform = 1.5f; //speed of projectile

    private bool movingLeft = false;

    
    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (movingLeft) {
            if (checkBoundDistance(leftBound)) {
                //start walking to the right
                movingLeft = false;
                movePlatform(rightBound);
            } else {
                //keep walking the same direction
                movingLeft = true;
                movePlatform(leftBound);
            }
        } else {
            if (checkBoundDistance(rightBound)) {
                //start walking to the left
                movingLeft = true;
                movePlatform(leftBound);
            } else {
                //keep walking the same direction
                movingLeft = false;
                movePlatform(rightBound);
            }
        }



        /*
        if (transform.position == leftBound.position) {
            movingLeft = false;
        } else if(transform.position == rightBound.position){
            movingLeft = true;
        }else {
            if(movingLeft) {
                movePlatform(leftBound);
            }else {
                movePlatform(rightBound);
            }
        }
	    */
    }


    public bool checkBoundDistance(Transform bound) {
        if ((Vector2.Distance(bound.position, transform.position)) <= precisionOfPlatformToBounds) {
            return true;
        } else {
            return false;
        }
    }

    public void movePlatform(Transform objective) {
        float step = speedOfPlatform * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, objective.position, step);
    }

    public void OnCollisionStay2D(Collision2D col) {
        if(col.gameObject.tag == "Player") {
            col.transform.parent = this.gameObject.transform;
        }
    }

    public void OnCollisionExit2D(Collision2D col) {
        col.transform.parent = null;
    }
    
    //need to get collider that player is standing on
    //cannot input them manually - cost inefficient
    //must get current platform user is on
    //check if possible to create a new layer - moving platform
    //easier to define from platform and a moving paltform
    



}
