using UnityEngine;
using System.Collections;

/// <summary>
/// This script has a public method, "ThisMethod" which can be called with BroadcastMessage
/// </summary>

namespace QuickCutscene.Demo {

public class QCDemoBroadcastMessageReceiver : MonoBehaviour {

	public void ThisMethod()
	{
		Debug.Log("This method has been called!");
	}
}

}