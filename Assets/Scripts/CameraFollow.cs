using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	public float vAnchorColliderOffsetX = 0f;
	public float hAnchorColliderOffsetY = 0f;
	public float hCameraSnapOffset = 2.5f;
	public float vCameraSnapOffset = 2.5f;
	public float dampTime = 0.15f;
    public float dampTime2 = 0.45f;
	public float verticalOffset = 0f;
    public float deltaY = 1f;
    public bool lockCamera = false;
    public Transform target;
    public Transform westWall, eastWall;
   

	[HideInInspector]public bool isAnchored = false;

    private Transform tagAnchor;
	private Vector3 velocity = Vector3.zero;
	private GameObject parentObject,westCameraObject,eastCameraObject,northCameraObject,southCameraObject;
	private GameObject anchors,westAnchor, eastAnchor, northAnchor, southAnchor;
	private Rigidbody2D rb;
	private BoxCollider2D westCameraBoxCollider,eastCameraBoxCollider,northCameraBoxCollider, southCameraBoxCollider;
	private Vector2 relativePosition;
    private Quaternion currentRotation;
    private bool anchorHorziontal = false;
    private bool anchorVertical = false;// true - horizontal , false - vertical
    private PlayerController pc;
    private float lastVerticalOffset;
    private RaycastHit2D hit;
    private LayerMask hitMask;
    private Vector3 hitVector;
    private Transform refGround;
    private GameObject platform;

    void Awake(){
		//init checks
		if(LayerMask.NameToLayer("Camera") == -1){
			Debug.LogError ("Layer 'Camera' not found");
		}
		// set the camera to the correct orthographic size
		// (so scene pixels are 1:1)
		//float s_baseOrthographicSize = Screen.height / 64.0f / 2.0f;
		//Camera.main.orthographicSize = s_baseOrthographicSize;

	}

	void Start(){
        pc = target.GetComponent<PlayerController>();
        hitMask = pc.playerMask;

        currentRotation = this.transform.rotation;

        tagAnchor = target.transform.Find("tag_anchor");
		tagAnchor.position = new Vector2 (target.position.x+vAnchorColliderOffsetX, target.position.y+hAnchorColliderOffsetY);

        

        /*
		 * 
		 * Camera Anchors
		 * 
		 */

        anchors = new GameObject ("AnchorsParent");
		anchors.transform.parent = Camera.main.transform;

		westAnchor = new GameObject ("WestAnchor");
		westAnchor.transform.parent = anchors.transform;
		relativePosition = Camera.main.WorldToScreenPoint(new Vector2 (target.position.x, target.position.y));
		westAnchor.transform.position = Camera.main.ScreenToWorldPoint (new Vector2 (0, relativePosition.y));

		eastAnchor = new GameObject ("EastAnchor");
		eastAnchor.transform.parent = anchors.transform;
		eastAnchor.transform.position = Camera.main.ScreenToWorldPoint (new Vector2 (Screen.width, relativePosition.y));

		northAnchor = new GameObject ("NorthAnchor");
		northAnchor.transform.parent = anchors.transform;
		relativePosition = Camera.main.WorldToScreenPoint(new Vector2 (target.position.x, target.position.y));
		northAnchor.transform.position = Camera.main.ScreenToWorldPoint (new Vector2 (relativePosition.x, Screen.height));

		southAnchor = new GameObject ("SouthAnchor");
		southAnchor.transform.parent = anchors.transform;
		southAnchor.transform.position = Camera.main.ScreenToWorldPoint (new Vector2 (relativePosition.x, 0));


        //Camera Object Parents
        parentObject = new GameObject("Parent");
		parentObject.transform.parent = Camera.main.transform;

		/**
		* 
		* West Camera Object 
		* 
		*/

		westCameraObject = new GameObject ("WestCameraObject");
		westCameraObject.layer = LayerMask.NameToLayer("Camera");
		westCameraObject.transform.parent = parentObject.transform;
		westCameraObject.transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (0, Screen.height/2, 0));

		rb = westCameraObject.AddComponent<Rigidbody2D> ();
		rb.isKinematic = true;
		rb.constraints = RigidbodyConstraints2D.FreezeRotation;

		westCameraObject.AddComponent<CameraColliders> ();
        westCameraObject.GetComponent<CameraColliders>().isVertical = true;

        westCameraBoxCollider = westCameraObject.AddComponent<BoxCollider2D> ();
		westCameraBoxCollider.size = new Vector2 (0.1f, Camera.main.orthographicSize * 2f-0.3f);
		westCameraBoxCollider.offset = new Vector2 (-hCameraSnapOffset, 0);
		westCameraBoxCollider.isTrigger = true;
		westCameraBoxCollider.enabled = true;



		/**
		 * 
		 * 
		 * East Camera Object 
		 * 
		 */
		eastCameraObject = new GameObject ("EastCameraObject");
		eastCameraObject.layer = LayerMask.NameToLayer("Camera");
		eastCameraObject.transform.parent = parentObject.transform;
		eastCameraObject.transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width, Screen.height/2, 0));

		rb = eastCameraObject.AddComponent<Rigidbody2D> ();
		rb.isKinematic = true;
		rb.constraints = RigidbodyConstraints2D.FreezeRotation;

		eastCameraObject.AddComponent<CameraColliders> ();
        eastCameraObject.GetComponent<CameraColliders>().isVertical = true;

        eastCameraBoxCollider = eastCameraObject.AddComponent<BoxCollider2D> ();
		eastCameraBoxCollider.size = new Vector2 (0.1f, Camera.main.orthographicSize * 2f-0.3f);
		eastCameraBoxCollider.offset = new Vector2 (hCameraSnapOffset, 0);
		eastCameraBoxCollider.isTrigger = true;
		eastCameraBoxCollider.enabled = true;


		/**
		 * 
		 * 
		 * North Camera Object 
		 * 
		 */
		northCameraObject = new GameObject ("NorthCameraObject");
		northCameraObject.layer = LayerMask.NameToLayer ("Camera");
		northCameraObject.transform.parent = parentObject.transform;
		northCameraObject.transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width / 2, Screen.height, 0));
        northCameraObject.transform.rotation = Quaternion.Euler(0, 0, 90);

        rb = northCameraObject.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        northCameraObject.AddComponent<CameraColliders>();
        northCameraObject.GetComponent<CameraColliders>().isVertical = false;

        northCameraBoxCollider = northCameraObject.AddComponent<BoxCollider2D>();
        northCameraBoxCollider.size = new Vector2(0.1f, Camera.main.orthographicSize * 2f * Camera.main.aspect-0.3f);
        northCameraBoxCollider.offset = new Vector2(vCameraSnapOffset+0.1f, 0);
        northCameraBoxCollider.isTrigger = true;
        northCameraBoxCollider.enabled = true;

        /**
		* 
		* 
		* South Camera Object 
		* 
		*/
        southCameraObject = new GameObject("SouthCameraObject");
        southCameraObject.layer = LayerMask.NameToLayer("Camera");
        southCameraObject.transform.parent = parentObject.transform;
        southCameraObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height/2-Screen.height/2, 0));
        southCameraObject.transform.rotation = Quaternion.Euler(0, 0, 90);

        rb = southCameraObject.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        southCameraObject.AddComponent<CameraColliders>();
        southCameraObject.GetComponent<CameraColliders>().isVertical = false;

        southCameraBoxCollider = southCameraObject.AddComponent<BoxCollider2D>();
        southCameraBoxCollider.size = new Vector2(0.1f, Camera.main.orthographicSize * 2f * Camera.main.aspect-0.3f);
        southCameraBoxCollider.offset = new Vector2(vCameraSnapOffset - 0.1f, 0);
        southCameraBoxCollider.isTrigger = true;
        southCameraBoxCollider.enabled = true;
        southCameraObject.SetActive(false);

        transform.position = new Vector3(tagAnchor.position.x, tagAnchor.position.y + verticalOffset, transform.position.z);
        lastVerticalOffset = verticalOffset;

    }

    // Update is called once per frame
    void Update (){
        hit = Physics2D.Raycast(target.position, -Vector2.up,Mathf.Infinity,hitMask);
        if (hit.transform != null) { //update hit vector if tag is not lava or spikes
            hitVector = new Vector3(hit.point.x, hit.point.y, transform.position.z);
            //Debug.Log(hit.transform.gameObject);
        }

        //set active horizontal objects north or south
        if(this.transform.rotation != currentRotation) {
            currentRotation = this.transform.rotation;
            SetActiveHorizontalObjects();
        }

        if(lastVerticalOffset != verticalOffset) {
            transform.position = new Vector3(tagAnchor.position.x, tagAnchor.position.y + verticalOffset, transform.position.z);
            lastVerticalOffset = verticalOffset;
        }

        //Update Anchor Positions
        relativePosition = Camera.main.WorldToScreenPoint(new Vector2 (target.position.x+vAnchorColliderOffsetX, target.position.y+hAnchorColliderOffsetY));
		tagAnchor.position = new Vector2 (target.position.x+vAnchorColliderOffsetX, target.position.y+hAnchorColliderOffsetY);

		westAnchor.transform.position = Camera.main.ScreenToWorldPoint (new Vector2 (0, relativePosition.y));
		eastAnchor.transform.position = Camera.main.ScreenToWorldPoint (new Vector2 (Screen.width, relativePosition.y));
		northAnchor.transform.position = Camera.main.ScreenToWorldPoint (new Vector2 (relativePosition.x, Screen.height));
		southAnchor.transform.position = Camera.main.ScreenToWorldPoint (new Vector2 (relativePosition.x, 0));

        Debug.DrawLine(tagAnchor.position,westAnchor.transform.position);
		Debug.DrawLine(tagAnchor.position,eastAnchor.transform.position);
		Debug.DrawLine(tagAnchor.position,northAnchor.transform.position);
		Debug.DrawLine(tagAnchor.position,southAnchor.transform.position);

        if (refGround == null) {
            refGround = hit.transform;
        }

        if (hit.transform != null) { //update platform if tag is not lava or spikes
            platform = hit.transform.gameObject;
        }

        if (!(anchorVertical || anchorHorziontal) && !lockCamera) {
            if (transform.position != hitVector && hit.transform != null) {
                Vector3 delta = hitVector - transform.position;
                if (!pc.isGround && !pc.isFalling) {
                    Vector3 destination = new Vector3(hitVector.x + delta.x, transform.position.y, transform.position.z);
                    transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
                }else{
                    if(hit.transform.gameObject.tag == "Lava") {
                        if (hit.distance < 2.0f) {
                            Vector3 destination = new Vector3(hitVector.x + delta.x, hitVector.y + delta.y + verticalOffset, transform.position.z);
                            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
                            refGround = hit.transform;
                        }
                        else { }
                    }else {
                        if(platform.GetComponent<MovingPlatform>() != null) {
                            Vector3 destination = new Vector3(hitVector.x + delta.x, hitVector.y + delta.y + verticalOffset, transform.position.z);
                            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity,dampTime2);
                            refGround = hit.transform;
                        }else {
                            Vector3 destination = new Vector3(hitVector.x + delta.x, hitVector.y + delta.y + verticalOffset, transform.position.z);
                            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
                            refGround = hit.transform;
                        }
                        
                    }
                    
                   // Debug.Log(refGround.transform.position.y);
                }
            }

        }

        if (anchorVertical || anchorHorziontal){
            if(anchorVertical && anchorHorziontal) {
                if (this.transform.rotation.z == 0) {
                    if (HAnchorSide() == true) { //west anchor
                        if (Vector2.Distance(tagAnchor.position, westAnchor.transform.position) > Vector2.Distance(target.position, eastAnchor.transform.position)) {
                            Debug.Log("Unanchored");
                            DampOutOnUnAnchor();
                            anchorVertical = false;
                        }
                    }
                    else { //east anchor
                        if (Vector2.Distance(tagAnchor.position, westAnchor.transform.position) < Vector2.Distance(target.position, eastAnchor.transform.position)) {
                            Debug.Log("Unanchored");
                            DampOutOnUnAnchor();
                            anchorVertical = false;
                        }
                    }
                }else { //rotation z == 180
                    if (HAnchorSide() == true) { //east anchor
                        if (Vector2.Distance(tagAnchor.position, eastAnchor.transform.position) > Vector2.Distance(target.position, westAnchor.transform.position)) {
                            Debug.Log("Unanchored");
                            DampOutOnUnAnchor();
                            anchorVertical = false;
                        }
                    }
                    else { //west anchor
                        if (Vector2.Distance(tagAnchor.position, eastAnchor.transform.position) < Vector2.Distance(target.position, westAnchor.transform.position)) {
                            Debug.Log("Unanchored");
                            DampOutOnUnAnchor();
                            anchorVertical = false;
                        }
                    }
                }
            

                if (this.transform.rotation.z == 0) { //north anchor
                    if (Vector2.Distance(tagAnchor.position, southAnchor.transform.position) < 0.6f) {
                        Debug.Log("Unanchored");
                        anchorHorziontal = false;
                    }
                }
                else {
                    if (Vector2.Distance(tagAnchor.position, northAnchor.transform.position) < 0.6f) {
                        Debug.Log("Unanchored");
                        anchorHorziontal = false;
                    }
                }


            }else{
                if (anchorVertical == true) {
                    if (transform.position != hitVector && hit.transform != null) {
                        Vector3 delta = hitVector - transform.position;
                        if (!pc.isGround && !pc.isFalling) {
                            Vector3 destination = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
                        }
                        else {
                            if (hit.transform.gameObject.tag == "Lava") {
                                if (hit.distance < 2.0f) {
                                    Vector3 destination = new Vector3(transform.position.x, hitVector.y + delta.y + verticalOffset, transform.position.z);
                                    transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
                                    refGround = hit.transform;
                                }
                                else { }
                            }
                            else {
                                if (platform.GetComponent<MovingPlatform>() != null) {
                                    Vector3 destination = new Vector3(transform.position.x, hitVector.y + delta.y + verticalOffset, transform.position.z);
                                    transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime2);
                                    refGround = hit.transform;
                                }
                                else {
                                    Vector3 destination = new Vector3(transform.position.x, hitVector.y + delta.y + verticalOffset, transform.position.z);
                                    transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
                                    refGround = hit.transform;
                                }

                            }

                        }
                    }

                    if (this.transform.rotation.z == 0) {
                        if (HAnchorSide() == true) { //west anchor
                            if (Vector2.Distance(tagAnchor.position, westAnchor.transform.position) > Vector2.Distance(target.position, eastAnchor.transform.position)) {
                                Debug.Log("Unanchored");
                                DampOutOnUnAnchor();
                                anchorVertical = false;
                            }
                        }
                        else { //east anchor
                            if (Vector2.Distance(tagAnchor.position, westAnchor.transform.position) < Vector2.Distance(target.position, eastAnchor.transform.position)) {
                                Debug.Log("Unanchored");
                                DampOutOnUnAnchor();
                                anchorVertical = false;
                            }
                        }
                    }else { //rotation z == 180
                        if (HAnchorSide() == true) { //east anchor
                            if (Vector2.Distance(tagAnchor.position, eastAnchor.transform.position) > Vector2.Distance(target.position, westAnchor.transform.position)) {
                                Debug.Log("Unanchored");
                                DampOutOnUnAnchor();
                                anchorVertical = false;
                            }
                        }
                        else { //west anchor
                            if (Vector2.Distance(tagAnchor.position, eastAnchor.transform.position) < Vector2.Distance(target.position, westAnchor.transform.position)) {
                                Debug.Log("Unanchored");
                                DampOutOnUnAnchor();
                                anchorVertical = false;
                            }
                        }
                    }
                }

                if (anchorHorziontal == true) {
                    if (transform.position != hitVector && hit.transform != null) {
                        Vector3 delta = hitVector - transform.position;
                        if (!pc.isGround && !pc.isFalling) {
                            Vector3 destination = new Vector3(hitVector.x + delta.x, transform.position.y, transform.position.z);
                            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
                        }
                        else {
                            if (hit.transform.gameObject.tag == "Lava") {
                                if (hit.distance < 2.0f) {
                                    Vector3 destination = new Vector3(hitVector.x + delta.x, transform.position.y, transform.position.z);
                                    transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
                                    refGround = hit.transform;
                                }
                                else { }
                            }
                            else {
                                if (platform.GetComponent<MovingPlatform>() != null) {
                                    Vector3 destination = new Vector3(hitVector.x + delta.x, transform.position.y, transform.position.z);
                                    transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime2);
                                    refGround = hit.transform;
                                }
                                else {
                                    Vector3 destination = new Vector3(hitVector.x + delta.x, transform.position.y, transform.position.z);
                                    transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
                                    refGround = hit.transform;
                                }

                            }

                        }
                    }

                    if (this.transform.rotation.z == 0) { //north anchor
                        if (Vector2.Distance(tagAnchor.position, southAnchor.transform.position) < 1.5f) {
                            Debug.Log("Unanchored");
                            anchorHorziontal = false;
                        }
                    } else { //south anchor
                        if (Vector2.Distance(tagAnchor.position, northAnchor.transform.position) < 1.5f) {
                            Debug.Log("Unanchored");
                            anchorHorziontal = false;
                        }
                    }
                }
            }
        }
	}

	public void Anchor(BoxCollider2D temp , string anchorType){
        if (Vector2.Distance(this.gameObject.transform.position, target.transform.position) > 4.0f || Vector2.Distance(this.gameObject.transform.position, target.transform.position) < -4.0f) {
            return;
        }
        if (anchorType == "horizontal"){
            this.anchorHorziontal = true;
		}else if(anchorType == "vertical"){
            this.anchorVertical = true;
		}else{
			Debug.LogError("Incorrect anchor type found in Anchor method parameters");
		}

	}

    public void MoveCameraTo(Vector3 newpos) {
        lockCamera = true;
        this.gameObject.transform.position = newpos;
        lockCamera = false;
    }

    public void UnAnchorCamera() {
        if(anchorVertical== true) {
            anchorVertical = false;
        }
    }

	private bool HAnchorSide(){ //true - west , false - east
		float distanceToWest = Vector2.Distance (target.position, westWall.position);
		float distanceToEast = Vector2.Distance (target.position,eastWall.position);
		if(distanceToWest > distanceToEast){
			return false;
		}else{
			return true;
		}
	}


    private void DampOutOnUnAnchor(){
		Vector3 point = Camera.main.WorldToViewportPoint (target.position);
		Vector3 delta = target.position - Camera.main.ViewportToWorldPoint (new Vector3 (0.5f, 0.5f, point.z));
		Vector3 destination = transform.position + delta;
		transform.position = Vector3.SmoothDamp (transform.position, destination, ref velocity, 0.9f);
	}


    private void SetActiveHorizontalObjects() {
        if (transform.rotation.z == 0) {
            northCameraObject.SetActive(true);
            southCameraObject.SetActive(false);
        }
        else { // z == +- 180
            northCameraObject.SetActive(false);
            southCameraObject.SetActive(true);
        }
    }

    private void autoFix() {
        /*if (westCameraBoxCollider.IsTouching(westWall.gameObject.GetComponent<BoxCollider2D>())) {
            Vector3.SmoothDamp(Camera.main.transform.position, new Vector3(Camera.main.transform.position.x + 0.05f, Camera.main.transform.position.y, Camera.main.transform.position.z) , ref velocity, 0.1f);
        }

        if (eastCameraBoxCollider.IsTouching(eastWall.gameObject.GetComponent<BoxCollider2D>())) {
            Vector3.SmoothDamp(Camera.main.transform.position, new Vector3(Camera.main.transform.position.x - 0.05f, Camera.main.transform.position.y, Camera.main.transform.position.z), ref velocity, 0.1f);
        }*/

    }
}


