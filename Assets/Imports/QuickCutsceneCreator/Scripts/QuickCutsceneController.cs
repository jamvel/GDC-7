using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using QuickCutscene.Utils;

/// Author: Alex Blaikie
/// Got a question or suggestion? Send a mail over to support@gadget-games.com
/// 
/// If you like this asset, feel free to rate it!

/// <summary>
/// Path type for gizmo/handle drawing
/// </summary>
public enum PathType {None=1, PathOnly=2, PathAndFrustum=3};

/// <summary>
/// Enumerations for the camera speed options, accessed by the QuickTakeCutsceneControllerEditor script
/// </summary>
public enum CameraSpeedOptions {Slow, Medium, Fast, Faster, Custom, Curve, MobileCurve, Instant, Lerp};
public enum CameraRotationSpeedOptions {SuperSlow, Slow, Medium, Fast, Faster, SuperFast, Custom, Instant, FollowTarget, Lerp};

/// <summary>
/// The 'main' script for the QuickTakeCutsceneController
/// This script controls all default actions and movement in the cutscene
/// </summary>
public class QuickCutsceneController : MonoBehaviour {

	//Variables start==========================================

	/// <summary>
	/// The only camera that we will move/use during the cutscene
	/// </summary>
	public Camera mainCutsceneCam;

	/// <summary>
	/// The audio sources that each audio track will play at in 3d space
	/// </summary>
	public AudioSource[] mainAudioPoints;

	/// <summary>
	/// The animator controllers referenced within the cutscene
	/// </summary>
	public Animator[] cutsceneAnimators;

	/// <summary>
	///The variables in the animator controllers (bools only) that we change on entering the cutscene
	/// </summary>
	public string[] cutsceneAnimatorVariables;

	/// <summary>
	///Used by the editor script to remember which animator variable is currently selected
	/// </summary>
	public int[] cutsceneAnimatorVarChoices;

	/// <summary>
	///The target values of the bools we change on entering the cutscene. These are reset at the end of the cutscene.
	/// </summary>
	public bool[] cutsceneAnimatorVariableTargets;

	/// <summary>
	///The main audio track to play at the start of the cutscene
	/// </summary>
	public AudioClip[] cutsceneAudio;

	/// <summary>
	///Scripts to disable whilst in the cutscene
	/// </summary>
	public MonoBehaviour[] disableWhileInCutscene;

	/// <summary>
	///These are the points that we will move and rotate the camera between during the cutscene.
	/// </summary>
	public Transform[] cutsceneCameraPoints;

	/// <summary>
	///Points to use on a bezier path as midpoints
	/// </summary>
	public Transform[] cutsceneMidPoints;

	/// <summary>
	///Points to use on a cubic bezier path as midpoints
	/// </summary>
	public Transform[] cutsceneCubicMidPoints;

	public bool[] curveChoice;

	public bool[] lerpCurveChoice;

	/// <summary>
	///The delay time before each transition that we wait (seconds)
	/// </summary>
	public float[] cutsceneEventKeyTime;

	/// <summary>
	///The time scale that each transition runs on (for slowmo/fastmo) (default is 1)
	/// </summary>
	public float[] cutsceneEventTimescale = new float[1]{1f};

	/// <summary>
	///The zoom choice - do we want to zoom during this event
	/// </summary>
	public bool[] cutsceneEventZoom;

	/// <summary>
	///Zoom amount (in FoV) of the camera over each cutscene event
	/// </summary>
	public float[] cutsceneEventZoomAmount = new float[1]{60f};

	/// <summary>
	//Do we want to use Time.deltaTime in camera movement calcs
	/// </summary>
	public bool useDeltaTime = true;

	private float deltaTimeMultiplier
	{
		get { 
			if(useDeltaTime)
			{					
				return Time.deltaTime;
			} else {
				return 1f;
			}
		}
	}

	/// <summary>
	///Zoom speed
	/// </summary>
	public float[] cutsceneEventZoomSpeed = new float[1]{0.1f};

	/// <summary>
	///The message to broadcast at the start of each respective transition
	/// </summary>
	public string[] broadcastMessageString;

	/// <summary>
	///Bool to control whether we will broadcast a message in each transition
	/// </summary>
	public bool[] broadcastMessageChoice;

	/// <summary>
	///The message target
	/// </summary>
	public GameObject[] broadcastMessageTarget;
	 
	/// <summary>
	///Controls the update() function
	/// </summary>
	public bool doUpdate = false;

	/// <summary>
	///Controls if we want camera shake during an event
	/// </summary>
	public bool[] doShake;	

	/// <summary>
	///The amount of camera shake for each shake option
	/// </summary>
	public float[] cameraShakeAmount = new float[1]{1f};
	private float cameraShakeTime = 0.05f;

	/// <summary>
	///The targets we want to smoothly follow
	/// </summary>
	public Transform[] smoothFollowTarget;
	public bool[] smoothFollow;

	/// <summary>
	///Camera movement speed options
	/// </summary>
	public CameraSpeedOptions[] cutsceneCameraSpeedOptions;

	/// <summary>
	///Custom movement and rotational speeds/times
	/// </summary>
	public float[] customMovementSpeed = new float[1]{1f};

	/// <summary>
	///Custom movement and rotational speeds/times
	/// </summary>
	public float[] customRotationSpeed = new float[1]{1f};

	/// <summary>
	///Custom movement and rotational speeds/times
	/// </summary>
	public float[] customCurveMovementSpeed = new float[1]{1f};

	/// <summary>
	///Curve length
	/// </summary>
	public int[] curveNodeCount = new int[1]{144};

	/// <summary>
	///Camera rotational speed options
	/// </summary>
	public CameraRotationSpeedOptions[] cutsceneCameraRotationSpeedOptions;

	/// <summary>
	/// Returns true if we are currently playing/in the cutscene
	/// </summary>
	public bool playingCutscene = false;

	/// <summary>
	///Gizmo path drawing
	/// </summary>
	public PathType showPathType;

	//Quick References
	private float currentZoomSpeed;
	private float initialZoom;
	private float currentZoomTarget;
	private Transform mainCamT;
	private bool mobileCurve = false;

	/// <summary>
	/// The current bezier path that we are moving on
	/// </summary>
	public Vector3[] currentNodes;

	private CameraSpeedOptions currentSpeedSetting;
	private CameraRotationSpeedOptions currentRotationalSpeedSetting;
	private Vector3 currentPositionTarget;
	private Quaternion currentRotationTarget;
	private Transform currentSmoothFollowTarget;
	private float currentCameraSpeed;
	private float currentCameraRotationSpeed;
	private int currentEvent = 0;
	private float initialTimescale;
	private float initialFixedDeltaTime;
	private bool instantTransition = false;
	private int currentNodePosition = 0;
	private Vector3[] currentDebugnodes;

	//Lerp related
	private float t = 0f;
	private bool currentUseLerp = false;
	private Vector3 lerpStartPoint;
	private float currentLerpRate;

	//Rotational lerp related
	private float r = 0f;
	private bool currentUseRotationLerp = false;
	private Quaternion rotationLerpStartPoint;
	private float currentRotationLerpRate;

	// ---- YOU CAN EDIT THE VARIABLE VALUES BELOW --//
	//Movement speed variables
	private float slowCameraSpeed = 0.4f;
	private float mediumCameraSpeed = 1.9f;
	private float fastCameraSpeed = 2.9f;
	private float fasterCameraSpeed = 4.1f;
	private float instantCameraSpeed = 9999f;

	//Rotational speed variables, in degrees
	private float superSlowCameraRotationSpeed = 2f;	
	private float slowCameraRotationSpeed = 7f;
	private float mediumCameraRotationSpeed = 15f;
	private float fastCameraRotationSpeed = 20f;
	private float fasterCameraRotationSpeed = 25f;
	private float superFastCameraRotationSpeed = 35f;

	//Camera shake timer (how often a 'shake' happens)
	private float initialCameraShakeTime = 0.05f;

	// ---- YOU CAN EDIT THE VARIABLE VALUES ABOVE --//

	//Variables end===============================================

	void Start()
	{
		SetCameraPointReferences();
		SetInitialReferences();
	}

	//Sets the references we need for all camera points/paths/etc at the start of the cutscene
	public void SetCameraPointReferences()
	{
		//Get the camera points by adding every transform that is an immediate child of this gameobject
		List<Transform> cPoints = new List<Transform>();
		cPoints.Add(transform);
		
		foreach(Transform trans in gameObject.transform)
		{
			cPoints.Add(trans);
		}
		
		cutsceneCameraPoints = cPoints.ToArray();
		
		//And create the grand-child list of transforms (bezier curve midpoints)
		List<Transform> gcPoints = new List<Transform>();
		for(int i = 0; i < cutsceneCameraPoints.Length; i++)
		{
			foreach(Transform gT in cutsceneCameraPoints[i])
			{	
				gcPoints.Insert(i, gT); //transform.IsChildOf
			}
		}
		
		cutsceneMidPoints = gcPoints.ToArray();
		
		//Get cubic (grand-grand-child) midpoints
		List<Transform> cubePoints = new List<Transform>();
		foreach(Transform t in cutsceneMidPoints)
		{
			if(t.parent != transform)
			{
				if(t.childCount > 0)
				{					
					cubePoints.Add(t.GetChild(0));
				} else {
					cubePoints.Add(t);
				}
			} else {
				cubePoints.Add(t);
			}
		}
		
		cutsceneCubicMidPoints = cubePoints.ToArray();

		//Error handling
		if(cutsceneCameraPoints.Length == 1)
		{
			Debug.LogWarning("No child transforms (camera points) found in '" + gameObject.name + "'");
		}
		
		//If we dont already have a camera set, just set it to the main camera
		if(!mainCutsceneCam)
		{
			mainCutsceneCam = Camera.main;
		}
		
		mainCamT = mainCutsceneCam.transform;		
		
		//Reference the cameras initial fov
		initialZoom = mainCutsceneCam.fieldOfView; 

	}

	//Sets some references on Start(). Separated to ensure OnDrawGizmos doesn't call this
	//which causes unwanted behaviour.
	void SetInitialReferences()
	{
		//Set time scale reference
		initialTimescale = Time.timeScale;
		initialFixedDeltaTime = Time.fixedDeltaTime;
	}
	
	// Update controls the calls to move and rotate the camera each frame
	void Update () 
	{
		//Only move the camera when we want to
		if(doUpdate == true)
		{	
			if(instantTransition)
			{
				mainCamT.position = currentPositionTarget;
				mainCamT.rotation = currentRotationTarget;
			} else {
				//Move & rotate the camera
				if(smoothFollow[currentEvent] == false)
				{
					if(curveChoice[currentEvent] == false)
					{				
						CameraMoveTowards();
						CameraRotateTowards();
					} else {
						CameraCurveTowards();
					}
				} else
				{
					if(curveChoice[currentEvent] == false)
					{					
						CameraFollow();
					} else {
						CameraCurveFollowTowards();
					}
				}
			}

			if(cutsceneEventZoom[currentEvent] == true)
			{
				CameraZoom();
			}

			//If the camera has reached its current destination, we can proceed
			if(mainCamT.position == currentPositionTarget)
			{
				doUpdate = false;
			}
		}

	}

	[ContextMenu("Start")]
	public void ActivateCutscene()
	{
		playingCutscene = true;

		//Start from the first point
		mainCamT.position = cutsceneCameraPoints[currentEvent+1].position;
		mainCamT.rotation = cutsceneCameraPoints[currentEvent+1].rotation;

		//Play each cutscene audio track on the corresponding audio source
		for(int i = 0; i < mainAudioPoints.Length; i++)
		{
			if(mainAudioPoints[i] != null && cutsceneAudio[i] != null)
			{
				mainAudioPoints[i].clip = cutsceneAudio[i];
				mainAudioPoints[i].Play();
			} else {
				Debug.LogWarning("No audio clip/source found");
			}
		}

		//Disable scripts we don't want active during the cutscene
		foreach(MonoBehaviour m in disableWhileInCutscene)
		{
			if(m)
			{				
				m.enabled = false;
			} else if(m == null){
				Debug.LogWarning("Cutscene script " + m + " was not set");
				EndCutscene();
			}
		}

		//Set Animator variables, handle null values
		for(int i = 0; i < cutsceneAnimators.Length; i++)
		{
			if(cutsceneAnimators[i] != null)
			{
				SetAnimatorVariable(cutsceneAnimatorVariables[i], cutsceneAnimatorVariableTargets[i], cutsceneAnimators[i]);
			} else {
				Debug.LogWarning("Cutscene animator " + cutsceneAnimators[i] + " did not have a value set");
				EndCutscene();
			}
		}

		//Broadcast "OnCutsceneStart" to this gameObject. Because we only do this at specific intervals, it is fine to use over delegates/events.
		BroadcastMessage("OnCutsceneStart", SendMessageOptions.DontRequireReceiver);

		//Start playing the cutscene
		StartCoroutine("PlayCutscene");

	}

	//This coroutine handles the cutscene whilst active. It will call itself to
	//proceed to the next 'event' in the cutscene after the preceding event is finished
	IEnumerator PlayCutscene()
	{
		//Make sure currentEvent is valid, if not, output to debug and end the cutscene
		if(cutsceneEventKeyTime.Length < currentEvent)
		{
			Debug.LogWarning("Camera event time " + cutsceneEventKeyTime[currentEvent] + " did not have a value set.");
			EndCutscene();
		}

		//Broadcast the message at the start of this event if we want to
		if(broadcastMessageChoice[currentEvent])
		{
			//Debug.Log("Trying to send message");
			if(broadcastMessageTarget[currentEvent] != null && broadcastMessageString[currentEvent] != "")
			{				
				broadcastMessageTarget[currentEvent].BroadcastMessage(broadcastMessageString[currentEvent], SendMessageOptions.DontRequireReceiver);
			} else {
				Debug.LogWarning("No target/method name for BroadcastMessage set");
			}
		}

		//Wait desired amount before moving
		yield return new WaitForSeconds(cutsceneEventKeyTime[currentEvent]);		
		
		//Broadcast that we have reached a transition. Because we only do this at specific intervals, it is fine to use over delegates/events.
		BroadcastMessage("OnCutsceneEnterTransition", currentEvent+1, SendMessageOptions.DontRequireReceiver);

		//Set our current positional, rotational and speed targets, with error handling
		if(cutsceneCameraPoints[currentEvent+2] != null)
		{		
			currentPositionTarget = cutsceneCameraPoints[currentEvent+2].position;
			currentRotationTarget = cutsceneCameraPoints[currentEvent+2].rotation;
		} else { //If something is wrong, end the cutscene
			Debug.LogWarning("Camera point " + cutsceneCameraPoints[currentEvent+2] + " did not have a value set.");
			EndCutscene();
		}

		//Set our movement and rotational speeds
		currentSpeedSetting = cutsceneCameraSpeedOptions[currentEvent];
		currentRotationalSpeedSetting = cutsceneCameraRotationSpeedOptions[currentEvent];

		//Check if we want to follow a target during this transition
		if(currentRotationalSpeedSetting == CameraRotationSpeedOptions.FollowTarget)
		{
			//Set the current position we want to follow with the camera
			currentSmoothFollowTarget = smoothFollowTarget[currentEvent];

			//Handle null references
			if(!currentSmoothFollowTarget)
			{
				Debug.LogWarning("No smooth follow target set for event " + (currentEvent+1));
				EndCutscene();
			}
		}

		//Set the cameras movement and rotational/follow speed targets
		SetCameraMovement();

		//Make sure we are still in the cutscene before we change the time scales
		if(playingCutscene)
		{
			//Set the time scale to be used for this event
			Time.timeScale = cutsceneEventTimescale[currentEvent];
			//Sets the fixed timestep as well. 0.02 is the default physics timestep
			Time.fixedDeltaTime = (cutsceneEventTimescale[currentEvent] * 0.02f);
		}

		SetAudioPitch();

		//Start updating in Update()
		doUpdate = true;			

		//Start camera shake if we want it
		if(doShake[currentEvent] == true)
		{
			cameraShakeTime = initialCameraShakeTime / cameraShakeAmount[currentEvent];
			StartCoroutine ("CameraShake");
		}

		//Control zooming
		if(cutsceneEventZoom[currentEvent] == true)
		{
			currentZoomSpeed = cutsceneEventZoomSpeed[currentEvent];
			SetZoomTarget();
		}

		//Start curved movement if we want it
		if(curveChoice[currentEvent] == true && currentUseLerp == false)
		{
			if(currentEvent < cutsceneCameraPoints.Length)
			{			
				Vector3[] path = new Vector3[curveNodeCount[currentEvent]];

				//Check what curve we want (mobile is quadratic/lower cost, other is cubic)
				if(mobileCurve)
				{
					for(int i = 0; i < curveNodeCount[currentEvent]; i++)
					{
						path[i] = 
							QCBezier.Bezier2( (float)i/curveNodeCount[currentEvent], cutsceneCameraPoints[currentEvent+1].position,cutsceneMidPoints[currentEvent+1].position,
							                 cutsceneCameraPoints[currentEvent+2].position);
					}
				} else {
					for(int i = 0; i < curveNodeCount[currentEvent]; i++)
					{
						path[i] = 
							QCBezier.Bezier3( (float)i/curveNodeCount[currentEvent], cutsceneCameraPoints[currentEvent+1].position,cutsceneMidPoints[currentEvent+1].position,
							                 cutsceneCubicMidPoints[currentEvent+1].position,
							                 cutsceneCameraPoints[currentEvent+2].position);
					}
				}

				currentNodes = path;

				//currentCameraSpeed = customCurveMovementSpeed[currentEvent] / currentNodes.Length;//Set this for lerping the value
			} else {
				Debug.LogWarning("Curve point not found, you cannot start a curve at the last camera point");
			}

		}

		//Don't progress further until current update has finished
		while(doUpdate == true)
		{	
			yield return null;
		}

		//If we have gone through all the camera points, finish the cutscene
		if(currentEvent == cutsceneCameraPoints.Length -3)
		{			
			currentEvent++;
			EndCutscene();
		} else { //If not, proceed to the next event
			currentEvent++;
			StartCoroutine("PlayCutscene");
		}
	}

	//This function ends the cutscene
	public void EndCutscene()
	{
		StopCoroutine("PlayCutscene");
		Time.timeScale = initialTimescale;
		Time.fixedDeltaTime = initialFixedDeltaTime;
		currentNodePosition = 0;
		playingCutscene = false;
		doUpdate = false;
		currentEvent = 0;
		SetAudioPitch();
		StopAudio();
		mainCutsceneCam.fieldOfView = initialZoom;

		//Reset Animator variables
		for(int i = 0; i < cutsceneAnimators.Length; i++)
		{
			//Error handling
			if(cutsceneAnimators[i])
			{				
				SetAnimatorVariable(cutsceneAnimatorVariables[i], !cutsceneAnimatorVariableTargets[i], cutsceneAnimators[i]);
			}
		}

		//Reset disabled scripts to initial state
		foreach(MonoBehaviour m in disableWhileInCutscene)
		{
			//Error handling
			if(m)
			{				
				m.enabled = true;
			}
		}		
		
		//Broadcast that we have ended the cutscene. Because we only do this at specific intervals, it is fine to use over delegates/events.
		BroadcastMessage("OnCutsceneEnd", SendMessageOptions.DontRequireReceiver);
	}

	//Rotate the camera towards the next camera position
	private void CameraCurveTowards()
	{
		CameraRotateTowards();
		CurveMove();
	}

	//Rotate by following a target with LookAt()
	private void CameraCurveFollowTowards()
	{
		if(currentSmoothFollowTarget != null)
		{
			mainCamT.LookAt(currentSmoothFollowTarget.position);
		}

		CurveMove();
	}	

	//Moves along a bezier path over a period of time
	private void CurveMove()
	{
		if(currentUseLerp) //If we are lerping, we need to create the points as we go
		{
			t += deltaTimeMultiplier / currentCameraSpeed;

			if(mobileCurve)
			{
				mainCamT.position = QCBezier.Bezier2( t, cutsceneCameraPoints[currentEvent+1].position,cutsceneMidPoints[currentEvent+1].position,
				                                     cutsceneCameraPoints[currentEvent+2].position);
			} else {
				mainCamT.position = 
					QCBezier.Bezier3( t, cutsceneCameraPoints[currentEvent+1].position,cutsceneMidPoints[currentEvent+1].position,
					                 cutsceneCubicMidPoints[currentEvent+1].position,
					                 cutsceneCameraPoints[currentEvent+2].position);
			}

			if(t >= 1)
			{
				doUpdate = false;
				t = 0f;
			}

		} else {

			if(currentNodePosition < currentNodes.Length)
			{			
				mainCamT.position = Vector3.MoveTowards(mainCamT.position, currentNodes[currentNodePosition], customCurveMovementSpeed[currentEvent] * Time.deltaTime);
				if(mainCamT.position == currentNodes[currentNodePosition])
				{
					currentNodePosition += 1;
				}
			} else {
				doUpdate = false;
				currentNodePosition = 0;
			}

		}
	}

	//Moves camera to a location
	private void CameraMoveTowards()
	{
		if(currentUseLerp)
		{			
			t += deltaTimeMultiplier / currentCameraSpeed;
			mainCamT.position = Vector3.Lerp(lerpStartPoint, currentPositionTarget, t);
		} else {				
			mainCamT.position = Vector3.MoveTowards(mainCamT.position, currentPositionTarget, currentCameraSpeed * deltaTimeMultiplier);
		}
	}

	//Rotates camera steadily
	private void CameraRotateTowards()
	{
		if(currentUseRotationLerp)
		{
			r += deltaTimeMultiplier / currentCameraRotationSpeed;
			mainCamT.rotation = Quaternion.Lerp(rotationLerpStartPoint, currentRotationTarget, r);
		} else {
			mainCamT.rotation = Quaternion.RotateTowards(mainCamT.rotation, currentRotationTarget, currentCameraRotationSpeed * deltaTimeMultiplier);
		}
	}

	//Used for making the camera's rotation 'focus' on a target whilst moving
	private void CameraFollow()
	{
		CameraMoveTowards();

		if(currentSmoothFollowTarget != null)
		{
			mainCamT.LookAt(currentSmoothFollowTarget.position);
		}
	}
	
	//Zooms the camera
	private void CameraZoom()
	{
		mainCutsceneCam.fieldOfView = Mathf.MoveTowards(mainCutsceneCam.fieldOfView, currentZoomTarget, currentZoomSpeed);	
	}

	//This function sets the cameras zoom target as a FOV value
	private void SetZoomTarget()
	{
		currentZoomTarget = cutsceneEventZoomAmount[currentEvent];//((initialZoom * cutsceneEventZoomAmount[currentEvent]) / 100f);
	}

	//This function sets the cameras movement values according to the current speed setting
	private void SetCameraMovement()
	{
		switch(currentSpeedSetting)
		{
			case CameraSpeedOptions.Slow:
			instantTransition = false;
			currentCameraSpeed = slowCameraSpeed;
			currentUseLerp = false;
			break;

			case CameraSpeedOptions.Medium:
			instantTransition = false;
			currentCameraSpeed = mediumCameraSpeed;
			currentUseLerp = false;
			break;

			case CameraSpeedOptions.Fast:
			instantTransition = false;
			currentCameraSpeed = fastCameraSpeed;
			currentUseLerp = false;
			break;

			case CameraSpeedOptions.Faster:
			instantTransition = false;
			currentCameraSpeed = fasterCameraSpeed;
			currentUseLerp = false;
			break;

			case CameraSpeedOptions.Custom:
			instantTransition = false;
			currentCameraSpeed = customMovementSpeed[currentEvent];
			currentUseLerp = false;
			break;

			case CameraSpeedOptions.Curve:
			instantTransition = false;
			currentCameraSpeed = customCurveMovementSpeed[currentEvent];
			currentUseLerp = lerpCurveChoice[currentEvent];
			
			if(currentUseLerp)
			{					
				t = 0f;
				curveChoice[currentEvent] = true;
			}
			mobileCurve = false;
			break;

			case CameraSpeedOptions.MobileCurve:
			instantTransition = false;
			currentCameraSpeed = customCurveMovementSpeed[currentEvent];
			currentUseLerp = lerpCurveChoice[currentEvent];

			if(currentUseLerp)
			{					
				t = 0f;
				curveChoice[currentEvent] = true;
			}

			mobileCurve = true;
			break;

			case CameraSpeedOptions.Instant:
			currentCameraSpeed = instantCameraSpeed;				
			currentUseLerp = false;
			instantTransition = true;
			break;

			case CameraSpeedOptions.Lerp:
			lerpStartPoint = mainCamT.position;
			currentCameraSpeed = customMovementSpeed[currentEvent];
			t = 0f;
			currentUseLerp = true;
			break;
		}

		switch(currentRotationalSpeedSetting)
		{
			case CameraRotationSpeedOptions.FollowTarget:
			instantTransition = false;
			smoothFollow[currentEvent] = true;
			break;

			case CameraRotationSpeedOptions.SuperSlow:
			currentCameraRotationSpeed = superSlowCameraRotationSpeed;
			instantTransition = false;
			smoothFollow[currentEvent] = false;
			break;

			case CameraRotationSpeedOptions.Slow:
			currentCameraRotationSpeed = slowCameraRotationSpeed;
			instantTransition = false;
			smoothFollow[currentEvent] = false;
			break;

			case CameraRotationSpeedOptions.Medium:
			currentCameraRotationSpeed = mediumCameraRotationSpeed;
			instantTransition = false;
			smoothFollow[currentEvent] = false;
			break;

			case CameraRotationSpeedOptions.Fast:
			currentCameraRotationSpeed = fastCameraRotationSpeed;
			instantTransition = false;
			smoothFollow[currentEvent] = false;
			break;

			case CameraRotationSpeedOptions.Faster:
			currentCameraRotationSpeed = fasterCameraRotationSpeed;
			instantTransition = false;
			smoothFollow[currentEvent] = false;
			break;

			case CameraRotationSpeedOptions.SuperFast:
			currentCameraRotationSpeed = superFastCameraRotationSpeed;
			instantTransition = false;
			smoothFollow[currentEvent] = false;
			break;

			case CameraRotationSpeedOptions.Custom:
			instantTransition = false;
			currentCameraRotationSpeed = customRotationSpeed[currentEvent];
			break;

			case CameraRotationSpeedOptions.Instant:
			currentCameraRotationSpeed = instantCameraSpeed;
			instantTransition = true;
			smoothFollow[currentEvent] = false;
			break;

			case CameraRotationSpeedOptions.Lerp:
			rotationLerpStartPoint = mainCamT.rotation;
			currentCameraRotationSpeed = customRotationSpeed[currentEvent];
			r = 0f;
			currentUseRotationLerp = true;
			break;
		}
	}

	//This method controls the setting of variables in the target animators
	private void SetAnimatorVariable(string targetVariable, bool targetValue, Animator targetAnimator)
	{
		targetAnimator.SetBool(targetVariable, targetValue);
		//Debug.Log("Set value successfully");
	}

	//Randomly shakes the camera
	IEnumerator CameraShake()
	{
		var randX = Random.Range(-0.05f * cameraShakeAmount[currentEvent], 0.05f * cameraShakeAmount[currentEvent]);
		
		var randY = Random.Range(-0.05f * cameraShakeAmount[currentEvent], 0.05f * cameraShakeAmount[currentEvent]);
		
		var randZ = Random.Range(-0.05f * cameraShakeAmount[currentEvent], 0.05f * cameraShakeAmount[currentEvent]);
		
		mainCamT.position += new Vector3(randX,randY,randZ);
		
		yield return new WaitForSeconds(cameraShakeTime);

		//If we still want to camera shake, shake
		if(doShake[currentEvent] == true)
		{			
			StartCoroutine("CameraShake");
		}
	}

	//Sets audio pitch for all audio sources to match the current time scale (slowmo/spedup audio)
	public void SetAudioPitch()
	{
		AudioSource[] aSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
		foreach (AudioSource aSource in aSources) {
			aSource.pitch = Time.timeScale;
		}
	}

	public void StopAudio()
	{
		for(int i = 0; i < mainAudioPoints.Length; i++)//foreach(AudioSource a in mainAudioPoints)		
		{
			if(mainAudioPoints[i])
			{
				mainAudioPoints[i].Stop();
			} else {
				Debug.LogWarning("Couldn't find audio source");
			}
		}
	}

	//Toggles path type, 1 is off, 2 is path only, 3 is path + frustrums
	public void ToggleShowPath()
	{
		if((int)showPathType < 3)
		{
			showPathType++;
		} else {
			showPathType = PathType.None;
		}
	}

	//Used to set a bezier path for the gizmo drawing
	void SetPathFromIndex(bool mobilePath, int currentIndex)
	{
		Vector3[] debugPath = new Vector3[curveNodeCount[currentIndex]];

		if(mobilePath)
		{

			for(int i = 0; i < curveNodeCount[currentIndex]; i++)
			{
				if(debugPath.Length > i)
				debugPath[i] = 
					QCBezier.Bezier2( (float)i/curveNodeCount[currentIndex], cutsceneCameraPoints[currentIndex+1].position,cutsceneMidPoints[currentIndex+1].position,
					                 cutsceneCameraPoints[currentIndex+2].position);

			}

		} else {

			for(int i = 0; i < curveNodeCount[currentIndex]; i++)
			{
				if(debugPath.Length > i)
				debugPath[i] = 
					QCBezier.Bezier3( (float)i/curveNodeCount[currentIndex], cutsceneCameraPoints[currentIndex+1].position,cutsceneMidPoints[currentIndex+1].position,
					                 cutsceneCubicMidPoints[currentIndex+1].position,
					                 cutsceneCameraPoints[currentIndex+2].position);
			}

		}

		currentDebugnodes = debugPath;

	}

	//Handles drawing gizmos/path/etc in the editor
	void OnDrawGizmos()
	{
		SetCameraPointReferences();

		//Only draw gizmos if we want to
		if(showPathType == PathType.PathOnly || showPathType == PathType.PathAndFrustum) 
		{
			Gizmos.color = Color.green;
						
			//Draw for each mid point, and draw a path
			for(int i = 0; i < cutsceneCameraPoints.Length-2; i++)
			{
				Gizmos.color = Color.yellow;

				if(curveChoice[i] == true)
				{			
					if(curveChoice[i] == true && cutsceneCameraSpeedOptions[i] == CameraSpeedOptions.MobileCurve)//Only draw this if it is a bezier curve point
					{
						SetPathFromIndex(true, i);
						
						Vector3 lastVec = cutsceneCameraPoints[i+1].position;
						
						foreach(Vector3 nextVec in currentDebugnodes)
						{
							Gizmos.DrawLine(lastVec, nextVec);
							lastVec = nextVec;
						}
					}
				}
			}
			
			//And draw for each cubic mid point, and a path
			for(int i = 0; i < cutsceneCameraPoints.Length-2; i++)
			{
				Gizmos.color = Color.magenta;

				if(curveChoice[i] == true && cutsceneCameraSpeedOptions[i] == CameraSpeedOptions.Curve)//Only draw this midpoint if it is a cubic curve
				{	
					SetPathFromIndex(false, i);
					
					Vector3 lastVec = cutsceneCameraPoints[i+1].position;
					
					foreach(Vector3 nextVec in currentDebugnodes)
					{
						Gizmos.DrawLine(lastVec, nextVec);
						lastVec = nextVec;
					}
				}

				Gizmos.color = Color.green;
				
				if(curveChoice[i] == false)
				{
					Gizmos.DrawLine(cutsceneCameraPoints[i+1].position, cutsceneCameraPoints[i+2].position);
				}
				

			}

			//Draw frustrums
			for(int i = 0; i < cutsceneCameraPoints.Length-1; i++)
			{
				//Set fov if it is custom
				float fov = initialZoom;

				if(i > 0 && i < cutsceneEventZoom.Length)
				{
					if(cutsceneEventZoom[i-1])
					{					
						fov = cutsceneEventZoomAmount[i-1];
					}
				}

				if(showPathType == PathType.PathAndFrustum)
				{					
					float max = 3f;
					float min = 0.1f;
					
					Matrix4x4 temp = Gizmos.matrix;
					Gizmos.matrix = Matrix4x4.TRS(cutsceneCameraPoints[i+1].position, cutsceneCameraPoints[i+1].rotation, Vector3.one);
					Gizmos.DrawFrustum(Vector3.zero, fov, max, min, 1);
					Gizmos.matrix = temp;
				}
			}
		}
	}

}