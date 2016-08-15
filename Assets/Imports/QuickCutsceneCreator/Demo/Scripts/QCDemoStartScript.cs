using UnityEngine;
using System.Collections;

/// <summary>
/// This script creates a GUI button which, when pressed, will end the referenced cutscene.
/// </summary>

namespace QuickCutscene.Demo {

public class QCDemoStartScript : MonoBehaviour {

	//The cutscene to start
	public QuickCutsceneController demoCutscene;

		void Update()
		{
			if(Input.GetKey(KeyCode.Alpha0))
			{
				Application.LoadLevel(Application.loadedLevel);
			}
		}

	//Create and control the button
	void OnGUI()
	{
		if(demoCutscene.playingCutscene)
		{
			if(GUI.Button(new Rect(0, 0,100,20), "Stop cutscene"))
			{
				demoCutscene.EndCutscene();
			}
		}
	}

}

}