using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeText : MonoBehaviour {
    public float FadeRate;
    private Text text;
    private float targetAlpha;
    // Use this for initialization
    void Start() {
        this.text = this.GetComponent<Text>();
        if (this.text == null) {
            Debug.LogError("Error: No text on " + this.name);
        }
        this.targetAlpha = this.text.color.a;
    }

    // Update is called once per frame
    void Update() {
        Color curColor = this.text.color;
        float alphaDiff = curColor.a - this.targetAlpha;
        if (alphaDiff > 0.001f) {
            curColor.a = curColor.a - FadeRate * Time.deltaTime;
            this.text.color = curColor;
        }else if(alphaDiff < 0.001f) {
            curColor.a = curColor.a + FadeRate * Time.deltaTime;
            this.text.color = curColor;
        }
    }

    public void FadeOut() {
        this.targetAlpha = 0.0f;
    }

    public void FadeIn() {
        this.targetAlpha = 1.0f;
    }
}
