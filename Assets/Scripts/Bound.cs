using UnityEngine;
using System.Collections;

public class Bound : MonoBehaviour {
	public float x = 1f;
	public float y = 1f;
	public float scale = 3f; // to put in level manager
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDrawGizmos(){
		//draw centre is always (0,0)
		float xS = x * scale;
		float yS = y * scale;

		Gizmos.color = Color.red;

		Gizmos.DrawLine (new Vector2 (-xS,yS), new Vector2 (xS, yS));
		Gizmos.DrawLine (new Vector2 (xS, yS), new Vector2 (xS, -yS));
		Gizmos.DrawLine (new Vector2 (xS, -yS), new Vector2 (-xS, -yS));
		Gizmos.DrawLine (new Vector2 (-xS, -yS), new Vector2 (-xS, yS));

	}
}
