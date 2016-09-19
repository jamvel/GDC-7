using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(Bound))]
public class BoundEditor : Editor{
	Bound b;
	public override void OnInspectorGUI(){
		GUILayout.BeginHorizontal();
		GUILayout.Label(" X Size ");
		b.x = EditorGUILayout.FloatField(b.x, GUILayout.Width(50));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label(" Y Size ");
		b.y = EditorGUILayout.FloatField(b.y, GUILayout.Width(50));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label(" Scale ");
		b.scale = EditorGUILayout.FloatField(b.scale, GUILayout.Width(50));
		GUILayout.EndHorizontal();

		SceneView.RepaintAll ();
	}

	public void OnEnable()
	{
		b = (Bound)target;
	}
}
