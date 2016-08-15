using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using QuickCutscene.Utils;

/// <summary>
/// This is the custom editor script for the legacy animation addon of the Quick Cutscene Controller.
/// It handles animation variables/names/gui/etc
/// </summary>

[CustomEditor(typeof(QCLegacyAnimationAddon))]
[CanEditMultipleObjects]
public class QCLegacyAnimationAddonEditor : Editor {

	//Quick references
	bool isValid;
	//QuickCutsceneController localCutscene;
	int numberOfEvents;
	Transform lastTrans;
	string[] names;
	int[] events;

	//Properties
	SerializedProperty legacyAnimators;
	SerializedProperty usedAnimationChoices;
	SerializedProperty usedAnimations;
	SerializedProperty animOptions;
	SerializedProperty startAtTransition;

	GUIContent warningContent = new GUIContent("NOTE: This script is still a WIP", "Because of the way Unity handles legacy animation states & clips, it 'scrambles' the clip order at runtime, so they cannot be generically accessed via their index." + 
	                                           " This is why the selected clips will change at runtime :|");

	//Error handling and reference 'gettting' 
	void OnEnable()
	{

		if(Selection.activeTransform)
		{
			//localCutscene = Selection.activeGameObject.GetComponent<QuickCutsceneController>();

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

			//Debug.Log("Cutscene has " + numberOfEvents + " events");

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

		startAtTransition = serializedObject.FindProperty("startAtTransition");
		animOptions = serializedObject.FindProperty("animOptions");
		legacyAnimators = serializedObject.FindProperty("legacyAnimators");
		usedAnimationChoices = serializedObject.FindProperty("usedAnimationChoices");
		usedAnimations = serializedObject.FindProperty("usedAnimations");
	}

	public override void OnInspectorGUI()
	{
		if(isValid)
		{
			//Updates the object we are editing
			serializedObject.Update();

			//Quick reference to the script
			QCLegacyAnimationAddon q = target as QCLegacyAnimationAddon;
			q.cutsceneEventAmount = numberOfEvents;

			//Quick references for when we want to change the size of the animator array
			bool enlarge = false;
			bool decrease = false;		

			EditorGUILayout.LabelField(warningContent, EditorStyles.boldLabel);

			EditorGUILayout.BeginHorizontal();
			//Animators
			EditorGUILayout.LabelField("Legacy animation actions:", EditorStyles.boldLabel);	
			//Plus button
			if (GUILayout.Button("+")) {
				enlarge = true;
			}		
			//If we press the plus button, increase the size of the array
			if (enlarge) {
				EnlargeAnimationArray();
				serializedObject.ApplyModifiedProperties();
			}		
			//Minus button
			if (GUILayout.Button("-")) {
				decrease = true;
			}
			//If we press the minus button, decrease array size
			if(decrease) {
				DecreaseAnimationArray();
				serializedObject.ApplyModifiedProperties();
			}				
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Animation,   " + "   Clip,   " + "   Action,   " + "   Transition " );
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();

			for(int i = 0; i < legacyAnimators.arraySize; ++i)
			{	
				q.legacyAnimators[i] = EditorGUILayout.ObjectField(q.legacyAnimators[i], typeof(Animation), true) as Animation;
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical();
			//Animators
			for(int i = 0; i < legacyAnimators.arraySize; ++i)
			{	
				if(q.legacyAnimators[i] != null)
				{
					List <string> animationVars = new List<string>(); 
					foreach(AnimationState acp in q.legacyAnimators[i])
					{
						animationVars.Add(acp.name);
					}
					
					string[] aClips = animationVars.ToArray();

					q.usedAnimationChoices[i] = EditorGUILayout.Popup(q.usedAnimationChoices[i], aClips);
					q.usedAnimations[i] = aClips[q.usedAnimationChoices[i]];
				}					
			}		
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical();
			for(int i = 0; i < legacyAnimators.arraySize; ++i)
			{
				q.animOptions[i] = (LegacyAnimationOptions)EditorGUILayout.EnumPopup(q.animOptions[i]);
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical();
			for(int i = 0; i < legacyAnimators.arraySize; ++i)
			{
				q.startAtTransition[i] = EditorGUILayout.IntPopup(q.startAtTransition[i], names, events);
			}
			EditorGUILayout.EndVertical();

			
			EditorGUILayout.EndHorizontal();

			if (GUI.changed)
			{
				EditorUtility.SetDirty (target);
			}
			
			//And apply
			serializedObject.ApplyModifiedProperties();
		}
	}
	
	//Increase the size of the Animation related arrays
	void EnlargeAnimationArray()
	{
		int enlargedAnimator = legacyAnimators.arraySize;
		int enlargedAnimatorVars = usedAnimationChoices.arraySize;
		int enlargedAnimVarTargets = usedAnimations.arraySize;
		int enlargedOptions = animOptions.arraySize;
		int enlargedStart = startAtTransition.arraySize;

		startAtTransition.InsertArrayElementAtIndex(enlargedStart);
		animOptions.InsertArrayElementAtIndex(enlargedOptions);
		legacyAnimators.InsertArrayElementAtIndex(enlargedAnimator);
		usedAnimationChoices.InsertArrayElementAtIndex(enlargedAnimatorVars);
		usedAnimations.InsertArrayElementAtIndex(enlargedAnimVarTargets);		
	}
	
	//Decrease size of the Animation related arrays
	void DecreaseAnimationArray()
	{
		startAtTransition.arraySize --;
		animOptions.arraySize --;
		legacyAnimators.arraySize --;
		usedAnimationChoices.arraySize --;
		usedAnimations.arraySize --;
	}


}
