using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using QuickCutscene.Utils;

/// <summary>
/// This is the custom editor script for the animation addon of the Quick Cutscene Controller.
/// It handles the inspector GUI. Surprise, surprise.
/// </summary>

[CustomEditor(typeof(QCAnimationAddon))]
[CanEditMultipleObjects]
public class QCAnimationAddonEditor : Editor {	
	
	//Quick references
	bool isValid;
	int numberOfEvents;
	
	string[] names;
	int[] events;
	Transform lastTrans;//Debugging	
	
	Dictionary<int, string[]> midAniVars = new Dictionary<int, string[]>();
	
	//Mid-cutscene animator changes
	/// The cutscene animators.	
	SerializedProperty animStartAtTransition;	
	SerializedProperty midCutsceneAnimators;	
	/// The cutscene animator variables.
	SerializedProperty midCutsceneAnimatorVariables;	
	/// The cutscene animator variable targets.
	SerializedProperty midCutsceneAnimatorVariableTargets;	
	SerializedProperty midCutsceneAnimatorVarChoices;
	SerializedProperty actionStartDelay;
	SerializedProperty showEvent;
	
	//Error handling and reference 'gettting' 
	void OnEnable()
	{		
		if(Selection.activeTransform)
		{			
			numberOfEvents = 0;
			
			List<string> eventNames = new List<string>();
			List<int> eventsNums = new List<int>();
			
			foreach(Transform trans in Selection.activeGameObject.transform)
			{
				if(numberOfEvents != 0)
				{					
					eventNames.Add("Transition# " + numberOfEvents.ToString());
				} else {
					eventNames.Add("Start");
				}
				eventsNums.Add(numberOfEvents);
				numberOfEvents ++;
				lastTrans = trans;
			}			
			
			eventNames.Add("End");
			eventsNums.Add(numberOfEvents);
			
			events = eventsNums.ToArray();
			names = eventNames.ToArray();
			
			isValid = true;
		} else {
			Debug.LogWarning("You can only edit the Cutscene as an in-scene object");
			EditorGUIUtility.ExitGUI();
			isValid = false;
		}
		
		//Debugging
		if(numberOfEvents == -1)
		{
			Debug.Log(lastTrans.name);
		}
		
		if(numberOfEvents == 1)
		{
			Debug.LogWarning("You must have at least two camera points, as child transforms to this object");
			isValid = false;
		} else {
			isValid = true;
		}
		
		showEvent = serializedObject.FindProperty("showEvent");
		animStartAtTransition = serializedObject.FindProperty("animStartAtTransition");
		midCutsceneAnimators = serializedObject.FindProperty("midCutsceneAnimators");
		midCutsceneAnimatorVariables = serializedObject.FindProperty("midCutsceneAnimatorVariables");
		actionStartDelay = serializedObject.FindProperty("actionStartDelay");
		midCutsceneAnimatorVariableTargets = serializedObject.FindProperty("midCutsceneAnimatorVariableTargets");
		midCutsceneAnimatorVarChoices = serializedObject.FindProperty("midCutsceneAnimatorVarChoices");
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
			//Updates the object we are editing
			serializedObject.Update();
			
			//Quick reference to the script
			QCAnimationAddon q = target as QCAnimationAddon;
			q.cutsceneEventAmount = numberOfEvents;
			
			//Quick references for when we want to change the size of the array
			bool enlarge = false;
			bool decrease = false;		
			
			EditorGUILayout.BeginHorizontal();
			//Animators
			EditorGUILayout.LabelField("Animator variable actions:", EditorStyles.boldLabel);	
			//Plus button
			if (GUILayout.Button("+")) {
				enlarge = true;
			}		
			//If we press the plus button, increase the size of the array
			if (enlarge) {
				EnlargeMidAnimatorArray();
				serializedObject.ApplyModifiedProperties();
			}		
			//Minus button
			if (GUILayout.Button("-")) {
				decrease = true;
			}
			//If we press the minus button, decrease array size
			if(decrease) {
				DecreaseMidAnimatorArray();
				serializedObject.ApplyModifiedProperties();
			}				
			EditorGUILayout.EndHorizontal();
			
			for(int i = 0; i < animStartAtTransition.arraySize; i++)
			{				
				GUIContent popoutContent = new GUIContent("Action # " + (i+1).ToString());
				q.showEvent[i] = EditorGUILayout.Foldout(q.showEvent[i], popoutContent);
				
				if(q.showEvent[i] == true)
				{
					q.animStartAtTransition[i] = EditorGUILayout.IntPopup(" Transition to start at", q.animStartAtTransition[i], names, events);
					
					
					q.midCutsceneAnimators[i] = EditorGUILayout.ObjectField(" Animator", q.midCutsceneAnimators[i], typeof(Animator), true) as Animator;
					
					//Show animator options
					EditorGUILayout.BeginVertical();
					//Display stuff
					EditorGUILayout.BeginHorizontal();
					if(q.midCutsceneAnimators[i] != null)
					{
						UnityEditor.Animations.AnimatorController animatorController = (UnityEditor.Animations.AnimatorController)GetEffectiveController(q.midCutsceneAnimators[i]) as UnityEditor.Animations.AnimatorController;	
						int currentHash = animatorController.GetHashCode();
						
						string[] temp = null;
						
						//If we dont have it, add it
						if(midAniVars.TryGetValue(currentHash, out temp))
						{
							q.midCutsceneAnimatorVarChoices[i] = EditorGUILayout.Popup(" Variables & target value", q.midCutsceneAnimatorVarChoices[i], temp);
							q.midCutsceneAnimatorVariables[i] = temp[q.midCutsceneAnimatorVarChoices[i]];
							q.midCutsceneAnimatorVariableTargets[i] = EditorGUILayout.Toggle(q.midCutsceneAnimatorVariableTargets[i]);
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
							midAniVars.Add(currentHash, animatorVars.ToArray());
							
							q.midCutsceneAnimatorVarChoices[i] = EditorGUILayout.Popup(" Variables & value", q.midCutsceneAnimatorVarChoices[i], temp);
							q.midCutsceneAnimatorVariables[i] = temp[q.midCutsceneAnimatorVarChoices[i]];
							q.midCutsceneAnimatorVariableTargets[i] = EditorGUILayout.Toggle(q.midCutsceneAnimatorVariableTargets[i]);
						}
						
					}
					EditorGUILayout.EndHorizontal();
					
					EditorGUILayout.EndVertical();
					
					q.actionStartDelay[i] = EditorGUILayout.FloatField(" Delay (sec) ", q.actionStartDelay[i]);
				}		
				
			}
			
			if (GUI.changed)
			{
				EditorUtility.SetDirty (target);
			}
			
		}
	}
	
	//Increase the size of the mid-scene animator related arrays
	void EnlargeMidAnimatorArray()
	{
		int enlargedAnimator = midCutsceneAnimators.arraySize;
		int enlargedAnimatorVars = midCutsceneAnimatorVariables.arraySize;
		int enlargedAnimVarTargets = midCutsceneAnimatorVariableTargets.arraySize;
		int enlargedAnimVarChoices = midCutsceneAnimatorVarChoices.arraySize;
		int enlargedstartAtInts = animStartAtTransition.arraySize;
		int enlargedStartDelay = actionStartDelay.arraySize;
		int eShowEvent = showEvent.arraySize;
		
		showEvent.InsertArrayElementAtIndex(eShowEvent);
		actionStartDelay.InsertArrayElementAtIndex(enlargedStartDelay);
		animStartAtTransition.InsertArrayElementAtIndex(enlargedstartAtInts);
		midCutsceneAnimators.InsertArrayElementAtIndex(enlargedAnimator);
		midCutsceneAnimatorVariables.InsertArrayElementAtIndex(enlargedAnimatorVars);
		midCutsceneAnimatorVariableTargets.InsertArrayElementAtIndex(enlargedAnimVarTargets);		
		midCutsceneAnimatorVarChoices.InsertArrayElementAtIndex(enlargedAnimVarChoices);
	}
	
	void DecreaseMidAnimatorArray()
	{
		showEvent.arraySize --;
		actionStartDelay.arraySize --;
		animStartAtTransition.arraySize --;
		midCutsceneAnimators.arraySize --;
		midCutsceneAnimatorVariables.arraySize --;
		midCutsceneAnimatorVariableTargets.arraySize --;
		midCutsceneAnimatorVarChoices.arraySize --;
	}
	
}
