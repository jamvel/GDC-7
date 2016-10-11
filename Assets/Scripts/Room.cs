using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Room : MonoBehaviour{
	public int id{ get; set;}
	public Scene scene{ get; set;}
	public int type {get;set;}

	public Room(int id, Scene scene , int type){
		this.id = id;
		this.scene = scene;
		this.type = type;
	}
}
