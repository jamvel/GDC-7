using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Respawn : MonoBehaviour {
    public Button option1;
    public Button option2;

    public EventSystem evtSystemRef;

    int selectedId = 0;


    void Update() {
        if(evtSystemRef.currentSelectedGameObject == null) {
            evtSystemRef.SetSelectedGameObject(option1.gameObject);
            select(0);
        }
    }
    public void select(int id) {
        selectedId = id;
        Debug.Log(selectedId);
    }
}
