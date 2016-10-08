using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	/*
	 * There can only be on Game manager during runtime
	 * Game manager loads into Scene_0 on Start of game
	 * 
	 */
	GameObject player; //assign player to game object for tracking


	public static GameManager instance;

	void Awake() {
		if(instance != null && instance != this) {
			DestroyImmediate(gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad(gameObject);
	}






}
