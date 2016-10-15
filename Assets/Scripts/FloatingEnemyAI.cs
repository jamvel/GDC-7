using UnityEngine;
using System.Collections;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]

public class FloatingEnemyAI : MonoBehaviour {
    public Transform target;

    public float updateRate = 5f;

    private Seeker seeker;
    private Rigidbody2D rigidBody;

    public Path path;

    public float speed = 0.5f;
    public ForceMode2D fMode;

    [HideInInspector]
    public bool pathEnded = false;
    public float nextDistance = 3;

    private int currentDistance = 0;

    private bool searchingPlayer = false;
    private bool sDirect = true;
    private Animator animator;
    private Collision coll;

    void Start() {
        seeker = GetComponent<Seeker>();
        rigidBody = GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();

        if (target == null) {
            if (!searchingPlayer) {
                searchingPlayer = true;
                StartCoroutine(SearchForPlayer());
            }
            return;
        }

        //start a new patch to the target position, return result to the OnPathComplete method
        seeker.StartPath(transform.position, target.position, OnPathComplete);
        StartCoroutine(UpdatePath());
    }

    void FixedUpdate() {
        if (target == null) {
            if (!searchingPlayer) {
                searchingPlayer = true;
                StartCoroutine(SearchForPlayer());
            }
            return;
        }

        //TODO: call lookAt function here
        if (path == null) {
            return;
        }

        if (currentDistance >= path.vectorPath.Count) {
            if (pathEnded) {
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
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        //Debug.Log("Player Location" + targetTransform.position);
        //Debug.Log("Enemy Location" + transform.position);
        //Debug.Log("Distance To Player" + distanceToPlayer);

        var relativePoint = transform.InverseTransformPoint(target.position);
        if (relativePoint.x < 0.0) {//walk left
            sDirect = true;
            animator.SetInteger("Gdirection", 2);
            // WalkSound();
        } else if (relativePoint.x > 0.0) {//walk right
            sDirect = false;
            animator.SetInteger("Gdirection", 1);
            // WalkSound();
        } else {//not moving
            if (sDirect) {//look right
                animator.SetInteger("Gdirection", 0);
            } else if (!sDirect) {//look left
                animator.SetInteger("Gdirection", 3);
            }
        }

        float dist = Vector3.Distance(transform.position, path.vectorPath[currentDistance]);
        if (dist < nextDistance) {
            currentDistance++;
            return;
        }
    }

    IEnumerator SearchForPlayer() {
        GameObject sResult = GameObject.FindGameObjectWithTag("Player");
        if (sResult == null) {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(SearchForPlayer());
        } else {
            target = sResult.transform;
            searchingPlayer = false;
            StartCoroutine(UpdatePath());
            yield return false;
        }
    }

    //no need to update with each frame
    //updated 2 times per second
    IEnumerator UpdatePath() {
        if (target == null) {
            if (!searchingPlayer) {
                searchingPlayer = true;
                StartCoroutine(SearchForPlayer());
            }
            yield return false;
        }

        //start a new patch to the target position, return result to the OnPathComplete method
        seeker.StartPath(transform.position, target.position, OnPathComplete);
        yield return new WaitForSeconds(1f / updateRate);
        StartCoroutine(UpdatePath());
    }

    public void OnPathComplete(Path p) {
        if (!p.error) {
            path = p;
            currentDistance = 0;

        } else {
            //Debug.Log("Path Error found " + p.error);
        }
    }




    void lookAt() {

    }

    void moveToPlayer() {

    }
    
    bool stopAtDistace(int distance) {

        return true;

    }
}
