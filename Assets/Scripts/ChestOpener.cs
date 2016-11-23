using UnityEngine;
using System.Collections;

public class ChestOpener : MonoBehaviour {

    public int numberOfCoins = 1;
    public Transform leftMost;
    public Transform rightMost;
    public LayerMask playerMask;
    public GameObject coin;

    private EmmiterVector emmiterCoin;
    private Animator animator;
    private bool isOpen = false;
    public AudioClip open;
    private AudioSource chest;
    // Use this for initialization
    void Start () {
        animator = this.GetComponent<Animator>();
        animator.SetInteger("Opened", 0);
        chest = gameObject.AddComponent<AudioSource>();
        emmiterCoin = this.GetComponent<EmmiterVector>();
        chest.clip = open;
    }

    // Update is called once per frame
    void Update () {
        //check if in range to open
        if (SearchForPlayer() && !isOpen) {
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
                chest.Play();
                isOpen = true;
                animator.SetInteger("Opened", 1);
                Vector3 coinPos = emmiterCoin.GetComponent<Transform>().position;
                coinPos.x = coinPos.x + 0.5f;//+1
                for (int i = 0; i < numberOfCoins; i++) {
                    Instantiate(coin, coinPos, emmiterCoin.GetComponent<Transform>().rotation);
                    coinPos.x = coinPos.x + (0.25f * -1);
                }
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
