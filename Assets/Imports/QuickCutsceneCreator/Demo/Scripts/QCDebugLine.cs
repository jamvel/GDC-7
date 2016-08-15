using UnityEngine;
using System.Collections;

/// <summary>
/// Disables the renderer component of the object it is attached to, when not in the editor
/// </summary>

public class QCDebugLine : MonoBehaviour {

	void Awake()
	{
		if(!Application.isEditor || Application.isPlaying)
		{
			if(GetComponent<Renderer>())
			GetComponent<Renderer>().enabled = false;
		}
	}
}
