using UnityEngine;
using System.Collections;

public class CameraColliders : MonoBehaviour {
    public bool isVertical;
    void OnTriggerEnter2D(Collider2D c){
		Debug.Log ("Anchored to GameObject "+c.name);
        if (isVertical) {
            Camera.main.GetComponent<CameraFollow>().Anchor(gameObject.GetComponent<BoxCollider2D>(), "vertical");
        }
        else {
            Camera.main.GetComponent<CameraFollow>().Anchor(gameObject.GetComponent<BoxCollider2D>(), "horizontal");
        }
	}
}
