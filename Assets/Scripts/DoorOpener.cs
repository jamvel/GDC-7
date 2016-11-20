using UnityEngine;
using System.Collections;

public class DoorOpener : MonoBehaviour {

    public Transform leftMost;
    public Transform rightMost;
    public LayerMask playerMask;

    private Animator animator;
    private bool isOpen = false;
    public AudioClip open;
    private AudioSource door;
    // Use this for initialization
    void Start() {
        animator = this.GetComponent<Animator>();
        animator.SetInteger("Opened", 0);
        door = gameObject.AddComponent<AudioSource>();
        door.clip = open;
    }


    // Update is called once per frame
    void Update() {
        //check if in range to open
        if (SearchForPlayer() && !isOpen) {
            if (Input.GetKey(KeyCode.DownArrow)) {
                door.Play();
                isOpen = true;
                animator.SetInteger("Opened", 1);
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
}
