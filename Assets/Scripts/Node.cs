using UnityEngine;
using System.Collections;

public class Node : MonoBehaviour {
    public GameObject toNode; //stores where player goes to if player hits node trigger
    public bool isWest;
    private Transform camSpawn;

    void Start() {
        camSpawn = toNode.transform.FindChild("Camera_Spawn");
    }

    void OnTriggerEnter2D(Collider2D c) {
        if(c.gameObject.tag == "Player") {
            //c.gameObject.transform.position = new Vector2(toNode.transform.position.x - 0.2f, toNode.transform.position.y);
            //cam.GetComponent<CameraFollow>().UnAnchorCamera();
            Camera.main.GetComponent<CameraFollow>().MoveCameraTo(new Vector3(camSpawn.position.x, camSpawn.position.y, -10));
            if (toNode.GetComponent<Node>().isWest) {
                c.gameObject.transform.position = new Vector3(toNode.transform.position.x + 0.6f, toNode.transform.position.y, toNode.transform.position.z);
            }else {
                c.gameObject.transform.position = new Vector3(toNode.transform.position.x - 0.6f, toNode.transform.position.y, toNode.transform.position.z);
            }
            
            
            
            
        }
    }
}
