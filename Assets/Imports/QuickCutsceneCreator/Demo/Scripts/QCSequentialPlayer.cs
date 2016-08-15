using UnityEngine;
using System.Collections;

/// <summary>
/// Quick script that finds every cutscene in the scene, and allows you to start the first 4 cutscenes via numpad button.
/// </summary>

namespace QuickCutscene.Demo {

public class QCSequentialPlayer : MonoBehaviour {

	public QuickCutsceneController[] cutscenes;

	// Use this for initialization
	void Start () {
		cutscenes = FindObjectsOfType(typeof(QuickCutsceneController)) as QuickCutsceneController[];
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Keypad1))
		{
			if(cutscenes.Length > 0)
			{
				cutscenes[0].ActivateCutscene();
			}
		}

		if(Input.GetKeyDown(KeyCode.Keypad2))
		{
			if(cutscenes.Length > 1)
			{
				cutscenes[1].ActivateCutscene();
			}
		}

		if(Input.GetKeyDown(KeyCode.Keypad3))
		{
			if(cutscenes.Length > 2)
			{
				cutscenes[2].ActivateCutscene();
			}
		}

		if(Input.GetKeyDown(KeyCode.Keypad4))
		{
			if(cutscenes.Length > 3)
			{
				cutscenes[3].ActivateCutscene();
			}
		}
	}

}

}