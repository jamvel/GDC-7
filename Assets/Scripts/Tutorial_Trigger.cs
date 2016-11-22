using UnityEngine;
using System.Collections;

public class Tutorial_Trigger : MonoBehaviour {
    public bool triggered = false;
    void OnTriggerEnter2D(Collider2D c) {
        if (!triggered) {
            if(c.tag == "Player") {
                triggered = true;
                Debug.Log("Loading Trigger");
                GameManager.instance.LoadRespawnScene(); //change to boss scene
            }
        }
    }
}
