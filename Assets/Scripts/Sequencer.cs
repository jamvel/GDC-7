using UnityEngine;
using System.Collections;

/*
* Controls on and off of the game objects
*/

public class Sequencer : MonoBehaviour {

    public int intervalNumber = 0; //current interval sequence
    public int totalNumberOfIntervals = 1; //total number of objects in sequence
    public float waitSeconds = 1f;
    public float waitRate = 1f;
    public int numberOfProjectiles = 1; // number of projectiles
    public float speedOfProjectile = 1f; //speed of projectile
    public bool x = false; //x axis or y axis
    public AudioClip[] tools;
    public AudioSource tool;

    public GameObject arrow;
    public Vector2 arrowVector;
    private EmmiterVector emmiterArrow;

    private bool on = false; //check if object should be on
    private Animator animator;
    private Rigidbody2D rigidBody;
    private int currentInterval = 0;


    void Start() {
        animator = this.GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        animator.SetBool("On", false);
        InvokeRepeating("sequence", waitSeconds, waitRate);
        emmiterArrow = this.GetComponent<EmmiterVector>();
        tool = gameObject.AddComponent<AudioSource>();
    }

    void sequence() {
        currentInterval++; //current interval
        if (currentInterval > totalNumberOfIntervals) { //check if current interval is out of bounds
            currentInterval = 1;
        }
        
        if (currentInterval == intervalNumber) { //shoot / go up
            if(numberOfProjectiles >= 1) {
                //call shoot function
                tool.clip = tools[0];
                tool.spatialBlend = 1;
                tool.Play();
                shoot();
            }
            on = true;
            //Debug.Log("Going Up");
        } else {//wait
            on = false;
            //Debug.Log("Waiting");
        }
        tool.clip = tools[0];
        tool.spatialBlend = 1;
        tool.volume = 0.125f;
        tool.Play();
        animator.SetBool("On", on);
    }

    void shoot() {
        //create direction to give to the projectiles and damage ....   
        if(x) { //if projectile is going up/down
            //Debug.Log("Shooting North/ South");
        }else { //projectile is going left/right
            //Debug.Log("Shooting Left/ Right");
            //rigidBody.velocity = new Vector2(speedOfProjectile, rigidBody.velocity.y);

            arrow.GetComponent<Projectile>().velocityVector = arrowVector + emmiterArrow.GetComponent<EmmiterVector>().directionVector; //speed + dir
            Instantiate(arrow, emmiterArrow.GetComponent<Transform>().position, emmiterArrow.GetComponent<Transform>().rotation);
        }
        
        //Debug.Log("Shooting");
    }
}
