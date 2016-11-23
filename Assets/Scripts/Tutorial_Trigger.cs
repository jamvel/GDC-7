using UnityEngine;
using System.Collections;

public class Tutorial_Trigger : MonoBehaviour {
    public bool triggered = false;
    public Camera camera1,camera2;
    public GameObject canvas;
    void OnTriggerEnter2D(Collider2D c) {
        if (!triggered) {
            if(c.tag == "Player") {
                triggered = true;
                Debug.Log("Loading Trigger");
                canvas.SetActive(false);
                camera1.gameObject.SetActive(false);
                camera2.gameObject.SetActive(true);
                GameManager.instance.LoadLevelScene();
            }
        }
    }
}
