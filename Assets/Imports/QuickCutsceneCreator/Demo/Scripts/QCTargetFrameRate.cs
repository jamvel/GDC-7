using UnityEngine;
using System.Collections;

public class QCTargetFrameRate : MonoBehaviour {

	[SerializeField] int targetFramerate;

	[SerializeField] bool useUpdate;

	// Use this for initialization
	void Start () {
		Time.captureFramerate = targetFramerate;

		if(!useUpdate)
		{
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		Time.captureFramerate = targetFramerate;
	}
}
