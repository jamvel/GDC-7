using UnityEngine;
using System.Collections;

public class ChestOpner : MonoBehaviour {

    public Transform leftMost;
    public Transform rightMost;
    public LayerMask playerMask;

    private Animator animator;
    private bool isOpen = false;
    
	// Use this for initialization
	void Start () {
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update () {
        //check if in range to open
        if(SearchForPlayer()) {
            if (Input.GetKey(KeyCode.F)) {
                animator.SetBool("Opened", true);
            } else {
                animator.SetBool("Opened", false);
            }
        }
    }

    //check if player is in the same platform as the platform
    public bool SearchForPlayer() {
        if ((Physics2D.Linecast(transform.position, leftMost.position, playerMask) || Physics2D.Linecast(transform.position, rightMost.position, playerMask))) { //Bound of platform check
            return true;
        } else {
            return false;
        }
    }

    /*
        public void OnTriggerStay2D() {
            animator.SetBool("Opened", true);
        }
    */
}
