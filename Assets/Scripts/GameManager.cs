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

    public AudioClip click;
    private AudioSource buttonClick;

    public Canvas UI_Canvas;
    public int nodeIndex = 0;
    public int coins = 0;

    public bool tier; //true - tier1 , false -tier2
    public int perk;
    public int curse;

    private bool loadLock = false;
    private bool deathScreen = false;
    private PlayerController controller;
    private Player playerScript;
    private GameObject Camera2;

    /*
     * UI
     */
    private GameObject pauseMenu;
    private GameObject deathOverlay;

    void Awake() {
		if(instance != null && instance != this) {
            Debug.Log("Destroy");
			DestroyImmediate(this);
			return;
		}
		instance = this;
		DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 60;
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
        deathOverlay = UI_Canvas.transform.Find("Death_Overlay").gameObject;
        
        canPause = true;
        paused = false;

        buttonClick = gameObject.AddComponent<AudioSource>();
        buttonClick.clip = click;

        Camera2 = GameObject.Find("Camera2");
        if(Camera2 != null) {
            Camera2.SetActive(false);
        }
        //DontDestroyOnLoad(player);
        //DontDestroyOnLoad(UI_Canvas);
    }

    void Update() {
        if (SceneManager.GetActiveScene().buildIndex != 2) {
            if (Input.GetKeyDown(KeyCode.Escape) && canPause) {
                if (paused) {
                    paused = false;
                    pauseMenu.SetActive(false);
                    Time.timeScale = 1;
                }
                else {
                    paused = true;
                    pauseMenu.SetActive(true);
                    Time.timeScale = 0;
                    AudioListener.volume = 0;
                }
            }

            if (playerScript.currentHealth <= 0 && !deathScreen && !playerScript.updateOnce) {
                playerScript.updateOnce = true;
                deathScreen = true;
                controller.playerDeath();
                controller.enabled = false; //disable player controller
                deathOverlay.SetActive(true);
                StartCoroutine(WaitForKeyDown());
            }
        }
    }

    IEnumerator WaitForKeyDown() {
        while (!Input.GetKeyDown(KeyCode.F)) {
            yield return null;
        }  
        deathScreen = false;
        UI_Canvas.gameObject.SetActive(false);
        Camera2.SetActive(true);
        LoadRespawnScene();
        /*
         * To Load spawn after death
         */

        //playerScript.currentHealth = playerScript.maxHealth;
        //playerScript.updateHealthBar();
        //controller.enabled = true; //enable player controller
        //deathOverlay.SetActive(false);


    }

    public void AssignObjects(GameObject newPlayer,Canvas newCanvas , GameObject Camera2) {
        player = newPlayer;
        controller = player.GetComponent<PlayerController>();
        playerScript = player.GetComponent<Player>();

        UI_Canvas = newCanvas;
        playerScript.UI_Canvas = UI_Canvas; //set UI Canvas in player script
        pauseMenu = UI_Canvas.transform.Find("Pause_Menu").gameObject;
        deathOverlay = UI_Canvas.transform.Find("Death_Overlay").gameObject;
        if(Camera2 != null) {
            this.Camera2 = Camera2;
            Camera2.SetActive(false);
        }
    }

    public void AssignCurse(int index) {
        if(index == 0) { //rotate 180
            GameObject Camera =  GameObject.Find("Main Camera");
            Camera.transform.rotation = Quaternion.Euler(0, 0, 180);
            player.GetComponent<PlayerController>().invertedControls = true;
        }else if(index == 1) { //colour blind
            GameObject Cam = GameObject.Find("Main Camera");
            Cam.GetComponent<RenderWithShader>().enabled = true;
        }
        else if(index == 2) { //airControl
            player.GetComponent<PlayerController>().airControl = false;
        }else if(index == 3) {
            //acid
        }else {
            //do nothing
        }
    }

    public void AssignPerk(int index) {
        if (tier) { //tier1

        }
        else { //tier2

        }
    }

    public void LoadLevelScene() {
        buttonClick.Play();
        StartCoroutine(LoadNewScene(3,3));
    }

    public void LoadRespawnScene() {
        StartCoroutine(LoadNewScene(2,2));
    }

    public void SetPerk(bool tier,int index) {
        this.tier = tier;
        perk = index;
    }

    public void SetCurse(int index) {
        curse = index;
    }

    IEnumerator LoadNewScene(float time, int index) {
        if (!loadLock) {
            loadLock = true;
            // This line waits for 3 seconds before executing the next line in the coroutine.
            yield return new WaitForSeconds(time);

            // Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
            AsyncOperation async = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);

            // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
            while (!async.isDone) {
                yield return null;
            }
            if (async.isDone) {
                Debug.Log("Load done");
                if (index != 2) {
                    AssignObjects(GameObject.Find("Player").gameObject, GameObject.Find("UI_Canvas_Main").GetComponent<Canvas>(),GameObject.Find("Camera2").gameObject);
                    AssignCurse(curse);
                    Debug.Log("PerkTier -> " + tier);
                    Debug.Log("Perk -> " + perk);
                    Debug.Log("Curse -> " + curse);
                }
            }
            loadLock = false;
        }
    }
}