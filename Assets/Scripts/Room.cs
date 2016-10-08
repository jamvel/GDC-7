using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Room : MonoBehaviour{
	public int id{ get; set;}
	public Scene scene{ get; set;}
	public int type {get;set;} //size type
	public Room[] connections{get; set;} // Room connects to other rooms


	public Room(int id, Scene scene , int type , Room[] connections){
		this.id = id;
		this.scene = scene;
		this.type = type;
		this.connections = connections;
	}
}
