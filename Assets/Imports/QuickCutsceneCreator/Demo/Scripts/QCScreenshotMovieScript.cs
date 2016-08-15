using UnityEngine;
using System.Collections;
using System.IO;

/// <summary>
/// WORK IN PROGRESS
/// Captures screenshots whilst maintaining a stable framerate.
/// Loosely adapted from a script on the Unity community wiki.
/// </summary>

namespace QuickCutscene.Utils {

public class QCScreenshotMovieScript : MonoBehaviour {

	[SerializeField] int frameRate = 25;
	[SerializeField] string pasta;
	[SerializeField] int secondsToRender;

	[SerializeField] bool manualControl;

	string fileName = "MovieScreenshot";
	float lastFrameTime = 0.0f;
	int capturedFrame = 0;
	//string status = "Idle";

	public bool doScreenshot = false;

	void Start () {

		Time.captureFramerate=frameRate;
		System.IO.Directory.CreateDirectory(pasta);
		
	}

	// Update is called once per frame
	void Update () {		

		if(Input.GetKeyDown(KeyCode.G))
		{
			doScreenshot = true;
		}

		
		if(doScreenshot)
		{
			float f = capturedFrame / frameRate;

			if(f < secondsToRender)
			{
				doScreenshot = true;
			} else {
				doScreenshot = false;
				Debug.Log("Finished capturing sequence");
			}
			
			if(manualControl)
			{
				if(Input.GetKey(KeyCode.F))
				{
					doScreenshot = true;
				} else {
					doScreenshot = false;
				}
				
			}

			// name is "realFolder/0005 shot.png"
			if(lastFrameTime < Time.frameCount + (1/30)) { // 24fps
				//status = "Captured frame " + capturedFrame;
				
				Application.CaptureScreenshot(Application.dataPath + "/../"+ pasta +"/" + fileName + " " + capturedFrame + ".png");
				capturedFrame++;
				lastFrameTime = Time.time;
			}
		}

	} 


}

}