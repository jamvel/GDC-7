using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_TutorialTriggerEnter : MonoBehaviour {
    public Image image1;
    public Image image2;
    public Text text1;
    public Text text2;
    public bool fadeIn;

    public bool isDialogue;
    public bool dialogueIn;
    public GameObject dialogue_panel;
    public string dialogue;
    public Image dialogue_img;

    void OnTriggerEnter2D(Collider2D c) {
        if (c.tag == "Player") {

            if (fadeIn) {
                if (image1 != null) {
                    image1.GetComponent<FadeImage>().FadeIn();
                }
                if (text1 != null) {
                    text1.GetComponent<FadeText>().FadeIn();
                }
                if (image2 != null) {
                    image2.GetComponent<FadeImage>().FadeIn();
                }
                if (text2 != null) {
                    text2.GetComponent<FadeText>().FadeIn();
                }

            }
            else {
                if (image1 != null) {
                    image1.GetComponent<FadeImage>().FadeOut();
                }
                if (text1 != null) {
                    text1.GetComponent<FadeText>().FadeOut();
                }
                if (image2 != null) {
                    image2.GetComponent<FadeImage>().FadeOut();
                }
                if (text2 != null) {
                    text2.GetComponent<FadeText>().FadeOut();
                }
            }

        }
    }
}

