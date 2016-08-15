using UnityEngine;
using System.Collections;

public class QCAnimationAddon : MonoBehaviour {
	
	//The animator controllers referenced within the cutscene
	public Animator[] midCutsceneAnimators;
	
	//Checks what position in the cutscene we want to perform each action at
	public int[] animStartAtTransition;
	
	//The variables in the animator controllers (bools only) that we change in specific cutscene events
	public string[] midCutsceneAnimatorVariables;
	
	//Used by the editor script to remember which animator variable is currently selected
	public int[] midCutsceneAnimatorVarChoices;
	
	//The target values of the bools we change. 
	public bool[] midCutsceneAnimatorVariableTargets;

	//Delay to wait before performing each action
	public float[] actionStartDelay;
	
	//Used for dropdown selction
	public bool[] showEvent;

	//The amount of events in the cutscene that we are attached to
	public int cutsceneEventAmount;

	private Animator currentAnimator;
	private string currentVariable;
	private bool currentVariableTarget;

	void OnCutsceneStart()
	{
		OnCutsceneEnterTransition(0);
	}

	void OnCutsceneEnterTransition(int transitionNum)
	{		
		//Deal with animators and animator vars
		for(int cnt = 0; cnt < animStartAtTransition.Length; cnt++)//foreach(int s in animStartAtTransition)
		{
			if(animStartAtTransition[cnt] == transitionNum)
			{
				if(midCutsceneAnimators[cnt] != null)
				{
					if(actionStartDelay[cnt] == 0)
					{						
						SetAnimatorVariable(midCutsceneAnimatorVariables[cnt], midCutsceneAnimatorVariableTargets[cnt], midCutsceneAnimators[cnt]);
					} else {
						currentAnimator = midCutsceneAnimators[cnt];
						currentVariableTarget = midCutsceneAnimatorVariableTargets[cnt];
						currentVariable = midCutsceneAnimatorVariables[cnt];
						StartCoroutine("FinishAfterDelay", actionStartDelay[cnt]);
					}
				} else {
					Debug.LogWarning("Cutscene animator " + midCutsceneAnimatorVariables[cnt] + " did not have a value set");
				}
			}
		}

	}

	void OnCutsceneEnd()
	{
		OnCutsceneEnterTransition(cutsceneEventAmount);
		StopCoroutine("FinishAfterDelay");
	}

	IEnumerator FinishAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		
		SetAnimatorVariable(currentVariable, currentVariableTarget, currentAnimator);
	}

	//This method controls the setting of variables in the target animators
	private void SetAnimatorVariable(string targetVariable, bool targetValue, Animator targetAnimator)
	{
		targetAnimator.SetBool(targetVariable, targetValue);
		//Debug.Log("Set value successfully");
	}
}
