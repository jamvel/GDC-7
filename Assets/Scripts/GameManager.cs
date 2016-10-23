using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour {
	
   public class MagicNode {
        public string id;
        public List<Magic> list;

        public MagicNode(string id, List<Magic> list) {
            this.id = id;
            this.list = list;
        }
    }

    /*
	 * There can only be on Game manager during runtime
	 * Game manager loads into Scene_0 on Start of game
	 * 
	 */

    public GameObject player; //assign player to game object for tracking

	public static GameManager instance;
    public static List<MagicNode> magicDB;
    [HideInInspector] public bool canPause; //boolean to check if player can pause game
    [HideInInspector] public bool paused;
    public Canvas UI_Canvas;

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
			DestroyImmediate(gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad(gameObject);
	}

	void Start(){
        //SceneManager.LoadScene("Menu_Scene"); //Load Menu Scene
        magicDB = buildMagicDatabase();

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

    List<MagicNode> buildMagicDatabase() {
        List<MagicNode> db = new List<MagicNode>();
        /*
         * Magic Item[0] : Fireball
         * Level 0
         *  - Level 1
         */
        db.Add(new MagicNode("fireball",new List<Magic>())); //add new Magic Node
        db[0].list.Add(new Magic(0, 25, 10)); //level 0
        db[0].list.Add(new Magic(1, 35, 25)); //level 1
        return db;
    }
		
}