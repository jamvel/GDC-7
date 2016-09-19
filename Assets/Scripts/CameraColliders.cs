using UnityEngine;
using System.Collections;

public class CameraColliders : MonoBehaviour {
	void OnTriggerEnter2D(Collider2D c){
		Debug.Log ("Anchored to GameObject "+c.name);
		Camera.main.GetComponent<CameraFollow> ().Anchor(gameObject.GetComponent<BoxCollider2D>(),"vertical");

		//check collider name or tag to identify if vertical or horizontal

	}
}
