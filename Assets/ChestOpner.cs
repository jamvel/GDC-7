using UnityEngine;
using System.Collections;

public class ChestOpner : MonoBehaviour {

    private Animator animator;
    private bool isOpen = false;
    
	// Use this for initialization
	void Start () {
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update () {


    }
    public void OnTriggerStay2D() {
        animator.SetBool("Opened", true);
    }
}
