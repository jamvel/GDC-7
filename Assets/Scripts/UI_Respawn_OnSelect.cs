using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class UI_Respawn_OnSelect : MonoBehaviour,ISelectHandler {
    public int perk_id;
    public int curse_id;
    public bool perk_tier;
    public bool isReroll;

    public AudioClip trans;
    private AudioSource transition;
    //Do this when the selectable UI object is selected.

    void Start(){
        transition = gameObject.AddComponent<AudioSource>();
        transition.clip = trans;
    }

    public void OnSelect(BaseEventData eventData) {
        transition.Play();
        if (isReroll) {
            UI_Respawn.instance.OnSelectedReroll();
        }else {
            UI_Respawn.instance.ChangePerk(perk_id, perk_tier);
            UI_Respawn.instance.ChangeCurse(curse_id);
        }
        
    }
}
