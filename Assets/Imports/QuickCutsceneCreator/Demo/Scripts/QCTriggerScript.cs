using UnityEngine;
using System.Collections;
using QuickCutscene;

/// <summary>
/// Starts the target Cutscene when OnTriggerEnter is called
/// </summary>

namespace QuickCutscene.utils {

	public class QCTriggerScript : MonoBehaviour {

		[SerializeField] QuickCutsceneController targetCutscene;

		void OnTriggerEnter(Collider other)
		{
			//If we aren't currently playing the cutscene, play it, then disable this script
			if(!targetCutscene.playingCutscene)
			{
				targetCutscene.ActivateCutscene();
				CallVanish();
			}
		}

		public void CallVanish()
		{
			StartCoroutine("Finish");
			StartCoroutine("Vanish");
		}

		IEnumerator Vanish()
		{
			transform.localScale = new Vector3(transform.localScale.x - 0.05f, 2f, transform.localScale.z - 0.05f);
			yield return new WaitForSeconds(0.02f);

			if(transform.localScale.z > 0f)
			{
				StartCoroutine("Vanish");
			} else {
				transform.localScale = Vector3.zero;
			}

		}

		IEnumerator Finish()
		{
			yield return new WaitForSeconds(3f);
			StopCoroutine("Vanish");
			this.enabled = false;
		}

	}

}