using UnityEngine;
using System.Collections;

public class ChestOpener : MonoBehaviour {

    public Transform leftMost;
    public Transform rightMost;
    public LayerMask playerMask;

    private Animator animator;
    private bool isOpen = false;
    
	// Use this for initialization
	void Start () {
        animator = this.GetComponent<Animator>();
        animator.SetInteger("Opened", 0);
    }

    // Update is called once per frame
    void Update () {
        //check if in range to open
        if (SearchForPlayer()) {
            if (!isOpen){
                if (Input.GetKey(KeyCode.DownArrow)) {
                    isOpen = true;
                    animator.SetInteger("Opened", 1);
                }
            }else{
                animator.SetInteger("Opened", 2);
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
