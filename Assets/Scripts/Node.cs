using UnityEngine;
using System.Collections;

public class Node : MonoBehaviour {
    public int levelID; //store level id for gamemanager to turn off/on levels approprately
    public GameObject[] toNode; //stores where player goes to if player hits node trigger
    public bool isWest;
    public bool isNorth; //if touches north collider
    [HideInInspector]public Transform camSpawn;
    [HideInInspector]public Transform playerSpawn;

    void Start() {
        camSpawn = toNode[GameManager.instance.nodeIndex].transform.FindChild("Camera_Spawn");
        playerSpawn = toNode[GameManager.instance.nodeIndex].transform.FindChild("Player_Spawn");
    }

    void OnTriggerEnter2D(Collider2D c) {
        if(c.gameObject.tag == "Player") {
            Camera.main.GetComponent<CameraFollow>().lockCamera = true;

            if (toNode[GameManager.instance.nodeIndex].GetComponent<Node>().isWest) {
                c.gameObject.transform.position = new Vector3(playerSpawn.position.x, playerSpawn.position.y, toNode[GameManager.instance.nodeIndex].transform.position.z);
            }
            else {
                c.gameObject.transform.position = new Vector3(playerSpawn.position.x, playerSpawn.position.y, toNode[GameManager.instance.nodeIndex].transform.position.z);
            }
            Camera.main.GetComponent<CameraFollow>().MoveCameraTo(new Vector3(camSpawn.position.x, camSpawn.position.y, -10));
            Camera.main.GetComponent<CameraFollow>().anchorVertical = true;
            if (toNode[GameManager.instance.nodeIndex].GetComponent<Node>().isWest) {
                Camera.main.GetComponent<CameraFollow>().westCameraCollider = true;
            }else {
                Camera.main.GetComponent<CameraFollow>().eastCameraCollider = true;
            }

            if (toNode[GameManager.instance.nodeIndex].GetComponent<Node>().isNorth) {
                Camera.main.GetComponent<CameraFollow>().anchorHorziontal = true;
                Camera.main.GetComponent<CameraFollow>().northCameraCollider = true;
            }
            Camera.main.GetComponent<CameraFollow>().lockCamera = false;
        }
    }
}
