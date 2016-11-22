using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueTrigger : MonoBehaviour {
    public bool dialogueIn;
    public GameObject dialogue_panel;
    public string dialogue;
    public Image dialogue_img; //to set
    public float timer = 0f;
    public bool isTriggered = false;

    void OnTriggerEnter2D(Collider2D c) {
        if (!isTriggered) {
            if (c.tag == "Player") {
                isTriggered = true;
                if (timer > 0f) {
                    StartCoroutine(DialogueTimer(timer));
                }else {
                    if (dialogueIn) {
                        dialogue_panel.transform.FindChild("Text").GetComponent<Text>().text = dialogue;
                        dialogue_panel.SetActive(true);
                    }
                    else {
                        dialogue_panel.SetActive(false);
                    }
                }
            }
        }
    }

    IEnumerator DialogueTimer(float time) {
        dialogue_panel.transform.FindChild("Text").GetComponent<Text>().text = dialogue;
        dialogue_panel.SetActive(true);
        yield return new WaitForSeconds(time);
        dialogue_panel.SetActive(false);
    }
}