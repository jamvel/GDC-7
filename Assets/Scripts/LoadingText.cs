using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingText : MonoBehaviour {
    private Text loadingText;

    void Start() {
        loadingText = this.GetComponent<Text>();
    }
    void Update() {
        loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
    }
    
}
