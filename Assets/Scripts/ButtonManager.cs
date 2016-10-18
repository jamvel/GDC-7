using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour {

    private bool paused = false;

    public void exitButton(){
        Application.Quit();
    }

    public void pauseButton(){
        if(paused){
            paused = false;
            Time.timeScale = 1;
            AudioListener.volume = 1;
        }
        else{
            paused = true;
            Time.timeScale = 0;
            AudioListener.volume = 0;
        }
    }
}
