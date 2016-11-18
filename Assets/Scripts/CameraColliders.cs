using UnityEngine;
using System.Collections;

public class CameraColliders : MonoBehaviour {
    public bool isVertical;
    void OnTriggerEnter2D(Collider2D c){
        if (!Camera.main.GetComponent<CameraFollow>().lockCamera) {
            Debug.Log("Anchored to GameObject " + c.name);
            Camera.main.GetComponent<CameraFollow>().Anchor(gameObject.GetComponent<BoxCollider2D>(), c.name);
        }
	}
}
