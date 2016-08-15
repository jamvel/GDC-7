using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(QCDebugLine))]
[CanEditMultipleObjects]
public class QCDebugLineEditor : Editor {

	public override void OnInspectorGUI()
	{
		EditorGUILayout.LabelField("Disables the renderer on a GameObject when outside the editor");
	}

}
