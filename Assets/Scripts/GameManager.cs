using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	
    /*
	 * There can only be on Game manager during runtime
	 * Game manager loads into Scene_0 on Start of game
	 * 
	 */

    public GameObject player; //assign player to game object for tracking

	public static GameManager instance;
    [HideInInspector] public bool canPause; //boolean to check if player can pause game
    [HideInInspector] public bool paused;

    public Perk []perks_tier1;
    public Perk[] perks_tier2;
    public Curse[] curses;

    public Canvas UI_Canvas;
    public int nodeIndex = 0;
    public int coins = 0;

    private bool deathScreen = false;
    private PlayerController controller;
    private Player playerScript;

    /*
     * UI
     */
    private GameObject pauseMenu;
    private GameObject audioSlider;
    private GameObject deathOverlay;

    void Awake() {
		if(instance != null && instance != this) {
            Debug.Log("Destroy");
			DestroyImmediate(this);
			return;
		}
		instance = this;
		DontDestroyOnLoad(gameObject);
	}

	void Start(){
        //SceneManager.LoadScene("Menu_Scene"); //Load Menu Scene

        controller = player.GetComponent<PlayerController>();
        playerScript = player.GetComponent<Player>();

        /*
         * UI 
         */
        playerScript.UI_Canvas = UI_Canvas; //set UI Canvas in player script
        pauseMenu = UI_Canvas.transform.Find("Pause_Menu").gameObject;
        audioSlider = pauseMenu.transform.Find("AudioSlider").gameObject;
        deathOverlay = UI_Canvas.transform.Find("Death_Overlay").gameObject;
        
        canPause = true;
        paused = false;

        //DontDestroyOnLoad(player);
        //DontDestroyOnLoad(UI_Canvas);
	}

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && canPause ) {
            if (paused) {
                paused = false;
                pauseMenu.SetActive(false);
                Time.timeScale = 1;
                AudioListener.volume = audioSlider.GetComponent<Slider>().value;
            }
            else {
                paused = true;
                pauseMenu.SetActive(true);
                Time.timeScale = 0;
                AudioListener.volume = 0;
            }
        }

        if(playerScript.currentHealth <= 0 && !deathScreen) {
            deathScreen = true;
            controller.playerDeath();
            controller.enabled = false; //disable player controller
            deathOverlay.SetActive(true);
            StartCoroutine(WaitForKeyDown());
        }
    }

    IEnumerator WaitForKeyDown() {
        while (!Input.GetKey(KeyCode.F))
            yield return null;
        deathScreen = false;

        /*
         * To Load spawn after death
         */

        playerScript.currentHealth = playerScript.maxHealth;
        playerScript.updateHealthBar();
        controller.enabled = true; //enable player controller
        deathOverlay.SetActive(false);
    }

    public void AssignObjects(GameObject newPlayer,Canvas newCanvas) {
        player = newPlayer;
        controller = player.GetComponent<PlayerController>();
        playerScript = player.GetComponent<Player>();

        UI_Canvas = newCanvas;
        playerScript.UI_Canvas = UI_Canvas; //set UI Canvas in player script
        pauseMenu = UI_Canvas.transform.Find("Pause_Menu").gameObject;
        audioSlider = pauseMenu.transform.Find("AudioSlider").gameObject;
        deathOverlay = UI_Canvas.transform.Find("Death_Overlay").gameObject;
    }

    public void LoadLevelScene() {
        StartCoroutine(LoadNewScene(3,3));
    }

    public void LoadRespawnScene() {
        StartCoroutine(LoadNewScene(3,2));
    }

    IEnumerator LoadNewScene(float time , int index) {
        // This line waits for 3 seconds before executing the next line in the coroutine.
        yield return new WaitForSeconds(time);

        // Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
        AsyncOperation async = SceneManager.LoadSceneAsync(index,LoadSceneMode.Single);

        // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        while (!async.isDone) {
            yield return null;
        }
        if (async.isDone) {
            Debug.Log("Load done");
            if (index != 2) {
                AssignObjects(GameObject.Find("Player").gameObject, GameObject.Find("UI_Canvas_Main").GetComponent<Canvas>());
            }
        }
        
    }

}