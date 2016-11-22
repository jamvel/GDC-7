using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeImage : MonoBehaviour {
    public float FadeRate;
    private Image image;
    private float targetAlpha = 0f;
    // Use this for initialization
    void Start() {
        this.image = this.GetComponent<Image>();
        if (this.image == null) {
            Debug.LogError("Error: No image on " + this.name);
        }
        this.targetAlpha = this.image.color.a;
    }

    // Update is called once per frame
    void Update() {
        Color curColor = this.image.color;
        if (targetAlpha == 0 && image.color.a == 0) {
            //do nothing
        }else {
            float alphaDiff = curColor.a - this.targetAlpha;
            if (alphaDiff > 0.0001f) {
                curColor.a = curColor.a - FadeRate * Time.deltaTime;
                this.image.color = curColor;
            }
            else if (alphaDiff < 0.0001f) {
                curColor.a = curColor.a + FadeRate * Time.deltaTime;
                this.image.color = curColor;
            }
        }
        
    }

    public void FadeOut() {
        this.targetAlpha = 0.0f;
    }

    public void FadeIn() {
        this.targetAlpha = 1.0f;
    }
}
