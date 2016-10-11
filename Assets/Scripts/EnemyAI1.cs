using UnityEngine;
using System.Collections;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]

public class EnemyAI1 : MonoBehaviour
{
    //what to follow ---> player
    public Transform targetTransform;

    //how many times per second to update path
    public float updateRate = 5f;

    //caching
    private Seeker seeker;
    private Rigidbody2D rigidBody;

    //calculated path
    public Path path;

    //AI speed per sec
    //not be be frame rate dependent 
    public float speed = 15f;
    public ForceMode2D fMode;

    [HideInInspector]
    public bool pathEnded = false;

    //max distance from AI to waypoint, for it to continue to the next waypoint
    public float nextDistance = 3;

    //waypoint currently moving towards
    private int currentDistance = 0;

    //boolean to use after player dies
    private bool searchingPlayer = false;
    private bool sDirect = true;
    private Animator animator;
    private Collision coll;

    void Start(){

        seeker = GetComponent<Seeker>();
        rigidBody = GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();

        //assign target body -> player
        if (targetTransform == null){
            if (!searchingPlayer) {
                searchingPlayer = true;
                StartCoroutine(SearchForPlayer());
            }
            return;
        }

        //start a new patch to the target position, return result to the OnPathComplete method
        seeker.StartPath(transform.position, targetTransform.position, OnPathComplete);
        StartCoroutine(UpdatePath());
    }

    IEnumerator SearchForPlayer() {
        GameObject sResult = GameObject.FindGameObjectWithTag("Player");
        if(sResult == null) {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(SearchForPlayer());
        }else {
            targetTransform = sResult.transform;
            searchingPlayer = false;
            StartCoroutine(UpdatePath());
            yield return false;
        }
    }
    
    //no need to update with each frame
    //updated 2 times per second
    IEnumerator UpdatePath(){
        if (targetTransform == null){
            if (!searchingPlayer){
                searchingPlayer = true;
                StartCoroutine(SearchForPlayer());
            }
            yield return false;
        }

        //start a new patch to the target position, return result to the OnPathComplete method
        seeker.StartPath(transform.position, targetTransform.position, OnPathComplete);
        yield return new WaitForSeconds(1f / updateRate);
        StartCoroutine(UpdatePath());
    }

    public void OnPathComplete(Path p){
        if (!p.error){
            path = p;
            currentDistance = 0;

        }else{
            //Debug.Log("Path Error found " + p.error);
        }
    }

    void FixedUpdate(){
        if (targetTransform == null){
            if (!searchingPlayer){
                searchingPlayer = true;
                StartCoroutine(SearchForPlayer());
            }
            return;
        }

        //TODO: call lookAt function here
        if (path == null){
            return;
        }

        if (currentDistance >= path.vectorPath.Count){
            if (pathEnded){
                return;
            }

            //Debug.Log("End of Path Reached");
            pathEnded = true;
            return;
        }
        pathEnded = false;

        //Direction to next waypoint
        Vector3 dir = (path.vectorPath[currentDistance] - transform.position).normalized;
        dir *= speed;// * Time.fixedDeltaTime;



        //Moving the enemy to the direction specified
        rigidBody.AddForce(dir, fMode);
       
        //distance to player
        //modify this part for range shooting
        float distanceToPlayer = Vector3.Distance(transform.position, targetTransform.position);
        //Debug.Log("Player Location" + targetTransform.position);
        //Debug.Log("Enemy Location" + transform.position);
        Debug.Log("Distance To Player" + distanceToPlayer);

        //if (OnCollision(coll)){
            //jump enemy
        //}

        var relativePoint = transform.InverseTransformPoint(targetTransform.position);
        if (relativePoint.x < 0.0){//walk left
            sDirect = true;
            animator.SetInteger("Sdirection", 2);
            // WalkSound();
        }else if (relativePoint.x > 0.0){//walk right
            sDirect = false;
            animator.SetInteger("Sdirection", 1);
            // WalkSound();
        }else{//not moving
            if (sDirect){//look right
                animator.SetInteger("Sdirection", 0);
            }else if (!sDirect){//look left
                animator.SetInteger("Sdirection", 3);
            }
        }



        float dist = Vector3.Distance(transform.position, path.vectorPath[currentDistance]);
        if (dist < nextDistance){
            currentDistance++;
            return;
        }
    }

    void lookAt(){

    }

    void moveToPlayer(){

    }

    bool OnCollision(Collision collision){
        if (collision.gameObject.tag == "Platform"){
            return true;
        }else if(collision.gameObject.tag == "Player"){
            StartCoroutine(SearchForPlayer());
            return true;
        }else {
            return false;
        }

    }

    bool stopAtDistace(int distance) {

        return true;

    }
}
