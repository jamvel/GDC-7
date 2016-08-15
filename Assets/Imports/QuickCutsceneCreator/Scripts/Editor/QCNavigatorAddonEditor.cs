using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using QuickCutscene.Utils;

/// <summary>
/// This is the custom editor script for the navigator addon of the Quick Cutscene Controller.
/// It handles the inspector GUI. Surprise, surprise.
/// </summary>

[CustomEditor(typeof(QCNavigatorAddon))]
[CanEditMultipleObjects]
public class QCNavigatorAddonEditor : Editor {
	
	//Quick references
	bool isValid;
	int numberOfEvents;
	
	string[] names;
	int[] events;
	Transform lastTrans;//Debugging
	
	//Properties
	SerializedProperty showEvent;//Dropdown thingss	
	SerializedProperty navAgentSpeed;
	SerializedProperty navActions;
	SerializedProperty navAgents;
	SerializedProperty actionStartDelay;
	SerializedProperty navDestinations;
	SerializedProperty startAtTransition;

	GUIContent cacheContent = new GUIContent("Pre-load Navigator paths", "If checked, every path will be generated at the start of the cutscene, to avoid delays in path generation.");
	
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

		navAgentSpeed = serializedObject.FindProperty("navAgentSpeed");
		navActions = serializedObject.FindProperty("navActions");
		navAgents = serializedObject.FindProperty("navAgents");
		actionStartDelay = serializedObject.FindProperty("actionStartDelay");
		navDestinations = serializedObject.FindProperty("navDestinations");
		startAtTransition = serializedObject.FindProperty("startAtTransition");
		showEvent = serializedObject.FindProperty("showEvent");
	}
	
	public override void OnInspectorGUI()
	{
		if(isValid)
		{
			//Updates the object we are editing
			serializedObject.Update();
			
			//Quick reference to the script
			QCNavigatorAddon q = target as QCNavigatorAddon;
			q.cutsceneEventAmount = numberOfEvents;
			
			//Quick references for when we want to change the size of the array
			bool enlarge = false;
			bool decrease = false;		
			
			q.preCalcPaths = EditorGUILayout.Toggle(cacheContent, q.preCalcPaths);

			EditorGUILayout.BeginHorizontal();
			//Animators
			EditorGUILayout.LabelField("Navigation actions:", EditorStyles.boldLabel);	
			//Plus button
			if (GUILayout.Button("+")) {
				enlarge = true;
			}		
			//If we press the plus button, increase the size of the array
			if (enlarge) {
				EnlargeNavActionArray();
				serializedObject.ApplyModifiedProperties();
			}		
			//Minus button
			if (GUILayout.Button("-")) {
				decrease = true;
			}
			//If we press the minus button, decrease array size
			if(decrease) {
				DecreaseNavActionArray();
				serializedObject.ApplyModifiedProperties();
			}				
			EditorGUILayout.EndHorizontal();

			for(int i = 0; i < navActions.arraySize; i++)
			{				
				GUIContent popoutContent = new GUIContent("Action # " + (i+1).ToString());
				q.showEvent[i] = EditorGUILayout.Foldout(q.showEvent[i], popoutContent);
				
				if(q.showEvent[i] == true)
				{
					q.startAtTransition[i] = EditorGUILayout.IntPopup(" Transition to start at", q.startAtTransition[i], names, events);
					
					q.navActions[i] = (NavAgentActions)EditorGUILayout.EnumPopup(" Nav Agent action", q.navActions[i]);
					
					//If we don't need to show a transform location, dont
					if(q.navActions[i] != NavAgentActions.Stop)
					{
						q.navDestinations[i] = (Transform)EditorGUILayout.ObjectField(" Target position", q.navDestinations[i], typeof(Transform), true) as Transform;
					} 
					
					q.navAgents[i] = (NavMeshAgent)EditorGUILayout.ObjectField(" NavMesh Agent", q.navAgents[i], typeof(NavMeshAgent), true) as NavMeshAgent;
					q.actionStartDelay[i] = EditorGUILayout.FloatField(" Delay (sec) ", q.actionStartDelay[i]);
					q.navAgentSpeed[i] = EditorGUILayout.FloatField(" Nav agent speed ", q.navAgentSpeed[i]);
				}		
				
			}

			if (GUI.changed)
			{
				EditorUtility.SetDirty (target);
			}
						
		}
	}
	
	
	//Increase the size of the nav-action related arrays
	void EnlargeNavActionArray()
	{
		int enSpeed = navAgentSpeed.arraySize;
		int enlargednavActions = navActions.arraySize;
		int enlargednavAgents = navAgents.arraySize;
		int enlargedactionStartDelay = actionStartDelay.arraySize;
		int enlargednavDestinations = navDestinations.arraySize;
		int enlargedstartAtTransition = startAtTransition.arraySize;
		int enShowEvent = showEvent.arraySize;
		
		navAgentSpeed.InsertArrayElementAtIndex(enSpeed);
		navActions.InsertArrayElementAtIndex(enlargednavActions);
		navAgents.InsertArrayElementAtIndex(enlargednavAgents);
		actionStartDelay.InsertArrayElementAtIndex(enlargedactionStartDelay);
		navDestinations.InsertArrayElementAtIndex(enlargednavDestinations);
		startAtTransition.InsertArrayElementAtIndex(enlargedstartAtTransition);		
		showEvent.InsertArrayElementAtIndex(enShowEvent);
		
	}
	
	//Decrease size of the navaction related arrays
	void DecreaseNavActionArray()
	{
		navAgentSpeed.arraySize --;
		showEvent.arraySize --;
		navActions.arraySize --;
		navAgents.arraySize --;
		actionStartDelay.arraySize --;
		navDestinations.arraySize --;
		startAtTransition.arraySize --;
	}
	
}
