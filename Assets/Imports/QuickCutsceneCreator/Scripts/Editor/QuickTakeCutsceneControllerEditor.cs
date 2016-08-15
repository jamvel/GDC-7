using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using QuickCutscene;

/// <summary>
/// This is the custom editor script for the QuickTakeCutsceneController.
/// 
/// This script creates a custom inspector GUI for the QuickTakeCutsceneController
/// </summary>

[CustomEditor(typeof(QuickCutsceneController))]
[CanEditMultipleObjects]
public class QuickTakeCutsceneControllerEditor : Editor {
	
	//Variables start===============================================================
	
	//Quick references
	int numberOfEvents;
	bool[] showEvent;
	int[] choiceIndex;
	Transform[] midPoints;
	Transform[] cubicPoints;
	Transform[] points;
	bool isValid = true;
	bool hasQCC = false;
	
	//For storing animator variables
	Dictionary<int, string[]> aVars = new Dictionary<int, string[]>();
	
	//Tooltips etc
	GUIContent startAudioContent = new GUIContent("Audio sources & clips ", "These audio clips will be played from each audio source at the start of the cutscene");
	GUIContent cameraContent = new GUIContent("Cutscene camera", "The main camera to be moved during the cutscene. No other cameras will be moved."); 
	GUIContent startDisabledScriptContent = new GUIContent("Scripts to disable", "These scripts will be disabled at the start of the cutscene, and re-enabled at the end"); 
	GUIContent animatorContent = new GUIContent("Animators & variables to set", "These booleans will be set at the start of the cutscene, and re-set at the end"); 
	GUIContent nodesContent = new GUIContent("   >Path nodes", "Number of points on the bezier path to generate. Use a low value if you want a fast transition."); 
	GUIContent curveSpeedContent = new GUIContent("   >Time", "How long should it take (seconds) for the camera to move along the path - impacted by path node amount");
	GUIContent delayContent = new GUIContent("Delay (sec)", "Time to wait (in seconds) before performing any actions in this event");
	GUIContent moveContent = new GUIContent("Movement ", "Camera positional movement speed during this event. (Units)");
	GUIContent midPointContent = new GUIContent("   >Midpoint 1: ", "The transform that will be used as a mid-point for this bezier curve. Should be a child of the preceding camera point in this transition.");
	GUIContent midPointTwoContent = new GUIContent("      >Midpoint 2: ", "The transform that will be used as a second mid-point for this bezier curve. Should be a child of the preceding mid point in this transition.");
	GUIContent rotationContent = new GUIContent("Rotation speed ", "Camera rotational speed during this event. (Degrees)");
	GUIContent zoomContent = new GUIContent("Zoom camera ", "Gradually changes the cameras FoV to zoom in/out");
	GUIContent broadcastContent = new GUIContent("Broadcast message", "Calls a desired method on the chosen GameObject");
	GUIContent lerpCurveContent = new GUIContent("Lerp", "Guarantees that this transition will finish in a set amount of time");
	
	/// The cutscene animators.
	SerializedProperty cutsceneAnimators;
	
	/// The cutscene animator variables.
	SerializedProperty cutsceneAnimatorVariables;
	
	/// The cutscene animator variable targets.
	SerializedProperty cutsceneAnimatorVariableTargets;
	
	SerializedProperty cutsceneAnimatorVarChoices;
	
	//The cutscene audio source and audio track variables
	SerializedProperty mainAudioPoints;
	SerializedProperty cutsceneAudio;
	
	/// The scripts to disable whilst in cutscene.
	SerializedProperty disableWhileInCutscene;
	
	//
	//====These below variables are camera point # dependant arrays===
	//
	
	//Messages to broadcast and array to check if we want to broadcast them 
	SerializedProperty broadcastMessageChoice;
	SerializedProperty broadcastMessageString;
	SerializedProperty broadcastMessageTarget;
	
	//Curve nodepoints/speed options
	SerializedProperty curveNodeCount;
	SerializedProperty customCurveMovementSpeed;
	
	//Targets for smooth follow
	SerializedProperty smoothFollowTarget;
	SerializedProperty smoothFollow;
	
	//Zoom amount for each event
	SerializedProperty cutsceneEventZoom;
	SerializedProperty cutsceneEventZoomAmount;
	SerializedProperty cutsceneEventZoomSpeed;
	
	/// The cutscene event key time.
	SerializedProperty cutsceneEventKeyTime;
	
	//The timescale event modifier
	SerializedProperty cutsceneEventTimescale;
	
	/// The camera shake option for each event.
	SerializedProperty doShake;
	
	/// The cutscene camera speed options.
	SerializedProperty cutsceneCameraSpeedOptions;
	SerializedProperty customRotationSpeed;
	SerializedProperty customMovementSpeed;
	
	/// The cutscene camera rotation speed options.
	SerializedProperty cutsceneCameraRotationSpeedOptions;
	
	// The cutscene path midpoints
	SerializedProperty cutsceneMidPoints;
	SerializedProperty cutsceneCubicMidPoints;
	
	//Choice of whether we want to curve during that transition
	SerializedProperty curveChoice;
	
	//And lerp
	SerializedProperty lerpCurveChoice;
	
	//Camera shake amount
	SerializedProperty cameraShakeAmount;
	
	//Variables end=====================================================================
	
	
	//Error handling and reference 'gettting' 
	void OnEnable()
	{
		
		//Gets the number of events/transitions
		if(Selection.activeTransform)
		{
			Transform t = Selection.activeTransform;
			
			List<Transform> cPoints = new List<Transform>();
			cPoints.Add(t);
			
			foreach(Transform trans in Selection.activeGameObject.transform)
			{
				cPoints.Add(trans);
			}
			
			points = cPoints.ToArray();
			numberOfEvents = points.Length;
			
			
			//And create the grand-child list of transforms (bezier curve midpoints)
			List<Transform> mPoints = new List<Transform>();
			for(int i = 0; i < numberOfEvents; i++)
			{
				foreach(Transform cT in points[i])
				{
					mPoints.Insert(i, cT);
				}
			}
			
			midPoints = mPoints.ToArray();
			
			//Get cubic (grand-grand-child) midpoints
			List<Transform> cubePoints = new List<Transform>();
			foreach(Transform tran in midPoints)
			{
				if(tran.parent != t)
				{
					if(tran.childCount > 0)
					{					
						cubePoints.Add(tran.GetChild(0));
					}
				} else {
					cubePoints.Add(tran);
				}
			}
			
			cubicPoints = cubePoints.ToArray();
			
			if(t.GetComponent<QuickCutsceneController>() == null)
			{
				hasQCC = false;
				Debug.LogWarning("When editing the cutscene, ensure that you always have the parent transform (with the Quick Cutscene Controller component on it) selected.");
			} else {
				hasQCC = true;
			}
			
		} else {
			Debug.Log ("You can only edit the Cutscene as an in-scene object");
		}
		
		if(numberOfEvents < 3 && hasQCC == true)
		{
			Debug.Log("Not enough camera points found, adding camera points.");
			isValid = false;
			
			for(int i = numberOfEvents; i < 3; i++)
			{
				GameObject g = new GameObject();
				g.transform.position = (Selection.activeTransform.position + Random.insideUnitSphere * 5f);
				g.transform.rotation = Selection.activeTransform.rotation;
				g.transform.parent = Selection.activeTransform;
				string n = ("CameraPoint_" + i.ToString());
				g.name = n;
			}
			
			this.OnEnable();
		} else {
			if(hasQCC == true)
			{				
				isValid = true;
			}
		}
		
		//Get proper references for variables primarily for use in setting array sizes 
		
		lerpCurveChoice = serializedObject.FindProperty("lerpCurveChoice");
		cutsceneCubicMidPoints = serializedObject.FindProperty("cutsceneCubicMidPoints");
		customCurveMovementSpeed = serializedObject.FindProperty("customCurveMovementSpeed");
		customMovementSpeed = serializedObject.FindProperty("customMovementSpeed");
		customRotationSpeed = serializedObject.FindProperty("customRotationSpeed");
		cameraShakeAmount = serializedObject.FindProperty("cameraShakeAmount");
		cutsceneEventZoomSpeed = serializedObject.FindProperty("cutsceneEventZoomSpeed");
		cutsceneEventZoomAmount = serializedObject.FindProperty("cutsceneEventZoomAmount");
		cutsceneEventZoom = serializedObject.FindProperty("cutsceneEventZoom");
		curveNodeCount = serializedObject.FindProperty("curveNodeCount");
		curveChoice = serializedObject.FindProperty("curveChoice");
		cutsceneMidPoints = serializedObject.FindProperty("cutsceneMidPoints");
		mainAudioPoints = serializedObject.FindProperty("mainAudioPoints");
		cutsceneAudio = serializedObject.FindProperty("cutsceneAudio");
		broadcastMessageChoice = serializedObject.FindProperty("broadcastMessageChoice");
		
		broadcastMessageString = serializedObject.FindProperty("broadcastMessageString");
		
		broadcastMessageTarget = serializedObject.FindProperty("broadcastMessageTarget");
		
		smoothFollow = serializedObject.FindProperty("smoothFollow");
		
		smoothFollowTarget = serializedObject.FindProperty("smoothFollowTarget");
		
		cutsceneAnimators = serializedObject.FindProperty("cutsceneAnimators");
		
		cutsceneAnimatorVariables = serializedObject.FindProperty("cutsceneAnimatorVariables");
		
		cutsceneAnimatorVarChoices = serializedObject.FindProperty("cutsceneAnimatorVarChoices");
		
		cutsceneAnimatorVariableTargets = serializedObject.FindProperty("cutsceneAnimatorVariableTargets");
		
		disableWhileInCutscene = serializedObject.FindProperty("disableWhileInCutscene");
		
		cutsceneEventKeyTime = serializedObject.FindProperty("cutsceneEventKeyTime");
		
		doShake = serializedObject.FindProperty("doShake");
		
		cutsceneCameraSpeedOptions = serializedObject.FindProperty("cutsceneCameraSpeedOptions");
		
		cutsceneCameraRotationSpeedOptions = serializedObject.FindProperty("cutsceneCameraRotationSpeedOptions");
		
		cutsceneEventTimescale = serializedObject.FindProperty("cutsceneEventTimescale");
		
		//Error handling
		if(cutsceneEventKeyTime.isArray == false || doShake.isArray == false || cutsceneCameraSpeedOptions.isArray == false
		   || cutsceneCameraRotationSpeedOptions.isArray == false || smoothFollowTarget.isArray == false || cutsceneMidPoints.isArray == false || curveNodeCount.isArray == false
		   )
		{
			// You shouldn't expect to see this.
			Debug.LogError("A property is not an array!");
		}
		
		//Array size handling/creation
		List <bool> boolList = new List<bool>();
		for(int i = 0; i < (numberOfEvents-1); i++)
		{
			boolList.Add(false);
			SetArraySize(numberOfEvents-2);
			serializedObject.ApplyModifiedProperties();
			//Debug.Log(cutsceneCameraSpeedOptions.arraySize);
		}
		
		//Create the bool array that handles showing each dropdown transition
		showEvent = boolList.ToArray();		
	}
	
	void OnSceneGUI()
	{
		if(isValid)
		{
			//Quick reference to the Quicktakecutscenecontroller script
			QuickCutsceneController q = target as QuickCutsceneController;
			
			q.SetCameraPointReferences();
			
			//The size of the gizmos to draw for each camera point
			float gizmoSize = 0.4f;
			
			EditorGUI.BeginChangeCheck();
			
			//Only draw gizmos if we want to
			if(q.showPathType == PathType.PathOnly || q.showPathType == PathType.PathAndFrustum) 
			{			
				//Draw circles for each main cam point
				for(int i = 1; i < q.cutsceneCameraPoints.Length; i++)
				{
					Handles.color = Color.green;
					Undo.RecordObject(q.cutsceneCameraPoints[i], "Move");
					
					q.cutsceneCameraPoints[i].position = Handles.FreeMoveHandle(q.cutsceneCameraPoints[i].position, Quaternion.identity,
					                                                            gizmoSize,
					                                                            Vector3.zero,
					                                                            Handles.SphereCap);
				}
				
				//Draw for each mid point
				for(int i = 0; i < q.cutsceneCameraPoints.Length-2; i++)
				{
					Handles.color = Color.yellow;
					
					if(q.curveChoice[i] == true)
					{			
						Undo.RecordObject(q.cutsceneMidPoints[i+1], "Move");
						q.cutsceneMidPoints[i+1].position = Handles.FreeMoveHandle(q.cutsceneMidPoints[i+1].position, Quaternion.identity,
						                                                           gizmoSize * 0.9f,
						                                                           Vector3.zero,
						                                                           Handles.SphereCap);
					}
				}
				
				//And draw for each cubic mid point, and a path
				for(int i = 0; i < q.cutsceneCameraPoints.Length-2; i++)
				{
					Handles.color = Color.magenta;
					
					if(q.curveChoice[i] == true && q.cutsceneCameraSpeedOptions[i] == CameraSpeedOptions.Curve)//Only draw this midpoint if it is a cubic curve
					{		
						Undo.RecordObject(q.cutsceneCubicMidPoints[i+1], "Move");	
						q.cutsceneCubicMidPoints[i+1].position = Handles.FreeMoveHandle(q.cutsceneCubicMidPoints[i+1].position, Quaternion.identity,
						                                                                gizmoSize * 0.8f,
						                                                                Vector3.zero,
						                                                                Handles.SphereCap);
					}					
					
					Handles.color = Color.green;					
				}
			}
			
		}
		
		if(EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(target, "Changes");
			
			EditorUtility.SetDirty(target);
		}
		
	}
	
	public RuntimeAnimatorController GetEffectiveController(Animator animator)
	{
		RuntimeAnimatorController controller = animator.runtimeAnimatorController;
		
		AnimatorOverrideController overrideController = controller as AnimatorOverrideController;
		while (overrideController != null)
		{
			controller = overrideController.runtimeAnimatorController;
			overrideController = controller as AnimatorOverrideController;
		}
		
		return controller;
	}
	
	public override void OnInspectorGUI()
	{
		if(isValid)
		{
			//Quick references for when we want to change the size of the animator array
			bool enlarge = false;
			bool decrease = false;
			
			//Quick references for when we want to change the size of the scripts to disable array
			bool enlargeScripts = false;
			bool decreaseScripts = false;
			
			//Quick reference for controlling the audio component arrays in the cutscene
			bool enlargeAudio = false;
			bool decreaseAudio = false;
			
			//Updates the object we are editing
			serializedObject.Update();
			
			//Quick reference to the Quicktakecutscenecontroller script
			QuickCutsceneController q = target as QuickCutsceneController;
			
			EditorGUILayout.BeginHorizontal();
			//Button to manually call the StartCutscene function, only if the game is in play mode
			if(GUILayout.Button("Play")){
				
				if(Application.isPlaying){
					q.ActivateCutscene();
				}
				
				if(!Application.isPlaying){
					Debug.Log("You can only play the cutscene when the game is running");
				}			
			}
			//Button to manually call the StartCutscene function, only if the game is in play mode
			if(GUILayout.Button("Stop")){
				
				if(Application.isPlaying){
					q.EndCutscene();
				}
				
				if(!Application.isPlaying){
					Debug.Log("You can only play/stop the cutscene when the game is running");
				}			
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			
			if(GUILayout.Button("Toggle Path (" + q.showPathType.GetHashCode() + ")")){
				q.ToggleShowPath();
			}
			
			if(GUILayout.Button("Add camera point"))
			{
				GameObject g = new GameObject();
				g.transform.position = (Selection.activeTransform.position + Random.insideUnitSphere * 5f);
				g.transform.rotation = Selection.activeTransform.rotation;
				g.transform.parent = Selection.activeTransform;
				string n = ("CameraPoint_" + numberOfEvents.ToString());
				g.name = n;
				Undo.RegisterCreatedObjectUndo(g, n);
				this.OnEnable();
			}
			
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			//Camera selector
			EditorGUILayout.LabelField(cameraContent);
			q.mainCutsceneCam = EditorGUILayout.ObjectField(q.mainCutsceneCam, typeof(Camera), true) as Camera;		
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Space();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(startAudioContent);
			//Plus button
			if (GUILayout.Button("+")) {
				enlargeAudio = true;
			}	
			//If we press the plus button, increase the size of the array
			if (enlargeAudio) {
				EnlargeAudioArray();
				serializedObject.ApplyModifiedProperties();
			}	
			//Minus button
			if (GUILayout.Button("-")) {
				decreaseAudio = true;
			}
			//If we press the minus button, decrease array size
			if(decreaseAudio) {
				DecreaseAudioArray();
				serializedObject.ApplyModifiedProperties();
			}	
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();
			for(int i = 0; i < q.mainAudioPoints.Length; ++i)
			{
				q.mainAudioPoints[i] = EditorGUILayout.ObjectField(q.mainAudioPoints[i], typeof(AudioSource), true) as AudioSource;
			}
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginVertical();
			for(int i = 0; i < q.cutsceneAudio.Length; ++i)
			{
				q.cutsceneAudio[i] = EditorGUILayout.ObjectField(q.cutsceneAudio[i], typeof(AudioClip), true) as AudioClip;
			}
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Space();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(startDisabledScriptContent);
			//Plus button
			if (GUILayout.Button("+")) {
				enlargeScripts = true;
			}	
			//If we press the plus button, increase the size of the array
			if (enlargeScripts) {
				EnlargeDisabledScriptArray();
				serializedObject.ApplyModifiedProperties();
			}	
			//Minus button
			if (GUILayout.Button("-")) {
				decreaseScripts = true;
			}
			//If we press the minus button, decrease array size
			if(decreaseScripts) {
				DecreaseDisabledScriptArray();
				serializedObject.ApplyModifiedProperties();
			}	
			EditorGUILayout.EndHorizontal();
			
			//Disabled scripts
			for(int i = 0; i < q.disableWhileInCutscene.Length; ++i)
			{
				q.disableWhileInCutscene[i] = EditorGUILayout.ObjectField(q.disableWhileInCutscene[i], typeof(MonoBehaviour), true) as MonoBehaviour;
			}
			EditorGUILayout.Space();
			
			EditorGUILayout.BeginHorizontal();
			//Animators
			EditorGUILayout.LabelField(animatorContent);	
			//Plus button
			if (GUILayout.Button("+")) {
				enlarge = true;
			}		
			//If we press the plus button, increase the size of the array
			if (enlarge) {
				EnlargeAnimatorArray();
				serializedObject.ApplyModifiedProperties();
			}		
			//Minus button
			if (GUILayout.Button("-")) {
				decrease = true;
			}
			//If we press the minus button, decrease array size
			if(decrease) {
				DecreaseAnimatorArray();
				serializedObject.ApplyModifiedProperties();
			}				
			EditorGUILayout.EndVertical();
			
			
			EditorGUILayout.BeginHorizontal();	
			EditorGUILayout.BeginVertical();	
			for(int i = 0; i < q.cutsceneAnimators.Length; ++i)
			{	
				q.cutsceneAnimators[i] = EditorGUILayout.ObjectField(q.cutsceneAnimators[i], typeof(Animator), true) as Animator;
			}
			EditorGUILayout.EndHorizontal();
			
			//Animators
			EditorGUILayout.BeginVertical();	
			for(int i = 0; i < q.cutsceneAnimators.Length; ++i)
			{	
				if(q.cutsceneAnimators[i] != null)
				{
					UnityEditor.Animations.AnimatorController animatorController = (UnityEditor.Animations.AnimatorController)GetEffectiveController(q.cutsceneAnimators[i]) as UnityEditor.Animations.AnimatorController;	
					int currentHash = animatorController.GetHashCode();
					
					string[] temp = null;
					
					//If we dont have it, add it
					if(aVars.TryGetValue(currentHash, out temp))
					{
						q.cutsceneAnimatorVarChoices[i] = EditorGUILayout.Popup(q.cutsceneAnimatorVarChoices[i], temp);
						q.cutsceneAnimatorVariables[i] = temp[q.cutsceneAnimatorVarChoices[i]];
					}
					else
					{
						List <string> animatorVars = new List<string>(); 					
						int count = 0;
						for(int n = 0; n < animatorController.parameters.Length; n++)
						{
							if(animatorController.parameters[n].type == UnityEngine.AnimatorControllerParameterType.Bool)
							{
								//animatorVars[count] = animatorController.GetParameter(n).ToString();
								UnityEngine.AnimatorControllerParameter acp = animatorController.parameters[n];
								animatorVars.Add (acp.name);
								count++;
							}
						}
						
						temp = animatorVars.ToArray();
						aVars.Add(currentHash, animatorVars.ToArray());
						
						q.cutsceneAnimatorVarChoices[i] = EditorGUILayout.Popup(q.cutsceneAnimatorVarChoices[i], temp);
						q.cutsceneAnimatorVariables[i] = temp[q.cutsceneAnimatorVarChoices[i]];
					}
					
				}		
				
			}
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginVertical();	
			for(int i = 0; i < q.cutsceneAnimators.Length; i++)
			{
				q.cutsceneAnimatorVariableTargets[i] = EditorGUILayout.Toggle(q.cutsceneAnimatorVariableTargets[i]);
			}
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Space();
			
			//Use delta time
			//EditorGUILayout.BeginHorizontal();
			//q.useDeltaTime = EditorGUILayout.Toggle(deltaTimeContent, q.useDeltaTime);
			//EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Space();
			
			//This for loop controls the display of each camera transition event, and the variables relating to it.
			for(int i = 0; i < (numberOfEvents-2); i++)
			{
				//Handle error which occurs when we undo a deleted CP
				if(!points[i+2])
				{
					this.OnEnable();
				} else {
					GUIContent popoutContent = new GUIContent("Camera transition " + (i+1) + " --> " + (i+2) + "     (" + points[i+1].name + " -> " + points[i+2].name + ")");
					showEvent[i] = EditorGUILayout.Foldout(showEvent[i], popoutContent);//"Camera transition " + (i+1) + " --> " + (i+2));
				}
				
				if(showEvent[i] == true)
				{
					if(i >= q.cutsceneEventKeyTime.Length)
					{
						//Debug.Log("Refreshing Editor GUI");
						//EditorGUIUtility.ExitGUI();
						this.OnEnable();
					}
					q.cutsceneEventKeyTime[i] = EditorGUILayout.FloatField(delayContent, q.cutsceneEventKeyTime[i]);					
					
					//EditorGUILayout.Space();
					
					q.cutsceneCameraSpeedOptions[i] = (CameraSpeedOptions)EditorGUILayout.EnumPopup(moveContent, q.cutsceneCameraSpeedOptions[i]);
					
					if(q.cutsceneCameraSpeedOptions[i] == CameraSpeedOptions.Curve || q.cutsceneCameraSpeedOptions[i] == CameraSpeedOptions.MobileCurve)
					{				
						q.curveChoice[i] = true;
						
						EditorGUILayout.BeginHorizontal();
						
						q.lerpCurveChoice[i] = EditorGUILayout.Toggle(lerpCurveContent, q.lerpCurveChoice[i]);
						
						EditorGUILayout.EndHorizontal();
						
						if(q.lerpCurveChoice[i])
						{
							q.customCurveMovementSpeed[i] = EditorGUILayout.Slider("   >Time (sec)", q.customCurveMovementSpeed[i], 0.0001f, 120f);
						} else {
							moveContent = new GUIContent("Movement ", "Movement speed options. Mobile curves require 1 mid-point, normal curves require 2 mid-points.");
							EditorGUILayout.BeginHorizontal();
							q.curveNodeCount[i] = EditorGUILayout.IntSlider(nodesContent, q.curveNodeCount[i], 10, 1000);						
							EditorGUILayout.EndHorizontal();
							q.customCurveMovementSpeed[i] = EditorGUILayout.Slider(curveSpeedContent, q.customCurveMovementSpeed[i], 0.0001f, 120f);
						}
						
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(midPointContent);
						EditorGUILayout.LabelField(midPoints[i+1].name);
						EditorGUILayout.EndHorizontal();
						if(q.cutsceneCameraSpeedOptions[i] == CameraSpeedOptions.Curve)
						{							
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField(midPointTwoContent);
							EditorGUILayout.LabelField(cubicPoints[i+1].name);
							EditorGUILayout.EndHorizontal();
						}
					} else {
						q.curveChoice[i] = false;
						moveContent = new GUIContent("Movement ", "Camera positional movement speed during this event. (Units)");
					}
					
					//Custom movement speed
					if(q.cutsceneCameraSpeedOptions[i] == CameraSpeedOptions.Custom)
					{
						q.customMovementSpeed[i] = EditorGUILayout.FloatField("   >Movement speed", q.customMovementSpeed[i]);
					}
					
					if(q.cutsceneCameraSpeedOptions[i] == CameraSpeedOptions.Lerp)
					{
						q.customMovementSpeed[i] = EditorGUILayout.FloatField("   >Movement time", q.customMovementSpeed[i]);
					}
					
					//EditorGUILayout.Space();
					
					//Rotation speed
					q.cutsceneCameraRotationSpeedOptions[i] = (CameraRotationSpeedOptions)EditorGUILayout.EnumPopup(rotationContent, q.cutsceneCameraRotationSpeedOptions[i]);
					if(q.cutsceneCameraRotationSpeedOptions[i] == CameraRotationSpeedOptions.FollowTarget)
					{
						q.smoothFollowTarget[i] = (Transform)EditorGUILayout.ObjectField("   >Follow target", q.smoothFollowTarget[i], typeof(Transform), true) as Transform;
					}
					
					//Custom rotation speed
					if(q.cutsceneCameraRotationSpeedOptions[i] == CameraRotationSpeedOptions.Custom)
					{
						q.customRotationSpeed[i] = EditorGUILayout.FloatField("   >Rotation speed", q.customRotationSpeed[i]);
					}
					
					//Custom rotation speed
					if(q.cutsceneCameraRotationSpeedOptions[i] == CameraRotationSpeedOptions.Lerp)
					{
						q.customRotationSpeed[i] = EditorGUILayout.FloatField("   >Rotation time", q.customRotationSpeed[i]);
					}
					
					//EditorGUILayout.Space();
					
					//Camera shake
					q.doShake[i] = EditorGUILayout.Toggle("Shake camera ", q.doShake[i]);
					if(q.doShake[i])
					{
						q.cameraShakeAmount[i] = EditorGUILayout.Slider("   >Shake intensity", q.cameraShakeAmount[i], 0.1f, 5f);
					}
					
					//EditorGUILayout.Space();
					
					//time scale and broadcastmessage
					q.cutsceneEventTimescale[i] = EditorGUILayout.Slider("Time scale", q.cutsceneEventTimescale[i], 0f, 2f);
					
					//EditorGUILayout.Space();
					
					q.broadcastMessageChoice[i] = EditorGUILayout.Toggle(broadcastContent, q.broadcastMessageChoice[i]);
					if(q.broadcastMessageChoice[i] == true)
					{
						EditorGUILayout.BeginVertical();
						q.broadcastMessageString[i] = EditorGUILayout.TextField("   >Method name",q.broadcastMessageString[i]);
						q.broadcastMessageTarget[i] = EditorGUILayout.ObjectField("   >Target", q.broadcastMessageTarget[i], typeof(GameObject), true) as GameObject;
						EditorGUILayout.EndVertical();
					}
					
					//EditorGUILayout.Space();
					
					q.cutsceneEventZoom[i] = EditorGUILayout.Toggle(zoomContent, q.cutsceneEventZoom[i]);
					if(q.cutsceneEventZoom[i] == true){
						q.cutsceneEventZoomAmount[i] = EditorGUILayout.Slider("   >Field of View",q.cutsceneEventZoomAmount[i], 1f, 144f);
						q.cutsceneEventZoomSpeed[i] = EditorGUILayout.Slider("   >Zoom speed",q.cutsceneEventZoomSpeed[i], 0.001f, 40f);
					}
					
					
					//EditorGUILayout.EndVertical();
					
				}
			}
			
			if (GUI.changed)
			{
				EditorUtility.SetDirty (target);
			}
			
			//And apply
			serializedObject.ApplyModifiedProperties();
		}
	}
	//Set the camera-point dependant arrays to their correct sizes
	void SetArraySize(int size)
	{
		lerpCurveChoice.arraySize = size;
		cutsceneCubicMidPoints.arraySize = size;
		customCurveMovementSpeed.arraySize = size;
		customRotationSpeed.arraySize = size;
		customMovementSpeed.arraySize = size;
		cameraShakeAmount.arraySize = size;
		curveNodeCount.arraySize = size;
		curveChoice.arraySize = size;
		cutsceneCameraSpeedOptions.arraySize = size;
		cutsceneCameraRotationSpeedOptions.arraySize = size;
		doShake.arraySize = size;		
		cutsceneEventTimescale.arraySize = size;
		cutsceneEventKeyTime.arraySize = size;
		smoothFollowTarget.arraySize = size;
		smoothFollow.arraySize = size;
		broadcastMessageChoice.arraySize = size;
		broadcastMessageString.arraySize = size;
		broadcastMessageTarget.arraySize = size;
		cutsceneEventZoom.arraySize = size;
		cutsceneEventZoomAmount.arraySize = size;
		cutsceneEventZoomSpeed.arraySize = size;
		
		serializedObject.ApplyModifiedProperties();
	}
	
	//Increase the size of the animator related arrays
	void EnlargeAnimatorArray()
	{
		int enlargedAnimator = cutsceneAnimators.arraySize;
		int enlargedAnimatorVars = cutsceneAnimatorVariables.arraySize;
		int enlargedAnimVarTargets = cutsceneAnimatorVariableTargets.arraySize;
		int enlargedAnimVarChoices = cutsceneAnimatorVarChoices.arraySize;
		
		cutsceneAnimators.InsertArrayElementAtIndex(enlargedAnimator);
		cutsceneAnimatorVariables.InsertArrayElementAtIndex(enlargedAnimatorVars);
		cutsceneAnimatorVariableTargets.InsertArrayElementAtIndex(enlargedAnimVarTargets);		
		cutsceneAnimatorVarChoices.InsertArrayElementAtIndex(enlargedAnimVarChoices);
	}
	
	//Decrease size of the animator related arrays
	void DecreaseAnimatorArray()
	{
		cutsceneAnimators.arraySize --;
		cutsceneAnimatorVariables.arraySize --;
		cutsceneAnimatorVariableTargets.arraySize --;
		cutsceneAnimatorVarChoices.arraySize --;
	}
	
	//Enlarge the "scripts to disable in cutscene" array
	void EnlargeDisabledScriptArray()
	{
		int enlargedScript = disableWhileInCutscene.arraySize;
		
		disableWhileInCutscene.InsertArrayElementAtIndex(enlargedScript);
	}
	
	void DecreaseDisabledScriptArray()
	{
		disableWhileInCutscene.arraySize --;
	}
	
	//Enlarge the cutscene audio sources & clips array
	void EnlargeAudioArray()
	{
		int enlargedAudio = mainAudioPoints.arraySize;
		int enlargedAudioClips = cutsceneAudio.arraySize;
		
		mainAudioPoints.InsertArrayElementAtIndex(enlargedAudio);
		cutsceneAudio.InsertArrayElementAtIndex(enlargedAudioClips);
	}
	
	void DecreaseAudioArray()
	{
		mainAudioPoints.arraySize --;
		cutsceneAudio.arraySize --;
	}
	
}
