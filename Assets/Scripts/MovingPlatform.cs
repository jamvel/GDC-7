using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour {

    public Transform leftBound;
    public Transform rightBound;

    public float distanceToMove = 0f;
    public float speedOfPlatform = 1f; //speed of projectile

    private bool movingLeft = false;

    
    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
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
	
	}

    public void movePlatform(Transform objective) {
        float step = speedOfPlatform * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, objective.position, step);
    }

}
