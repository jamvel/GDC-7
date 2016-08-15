using UnityEngine;
using System.Collections;

/// <summary>
/// This script adds extra legacy animation options for each event in a cutscene.
/// Requires a Quick Cutscene Controller component
/// </summary>

namespace QuickCutscene.Utils {

	public enum LegacyAnimationOptions {Play, Crossfade};

	[RequireComponent(typeof(QuickCutsceneController))]
	public class QCLegacyAnimationAddon : MonoBehaviour {

		//Do we want to play an animation or crossfade to it
		public LegacyAnimationOptions[] animOptions;

		//The Animation components of each object
		public Animation[] legacyAnimators;

		//The list # for each animation clip we have chosen
		public int[] usedAnimationChoices;

		//The name of each animation clip we have chosen
		public string[] usedAnimations;

		//The amount of events in the cutscene that we are attached to
		public int cutsceneEventAmount;

		//Checks what position in the cutscene we want to perform each action at
		public int[] startAtTransition;

		//Quick references
		Animation currentLegacyAnimator;
		string currentLegacyAnimationClip;
		LegacyAnimationOptions currentLegacySetting;

		public void OnCutsceneStart()
		{
			//Perform the action if the option is meant to be performed on cutscene start
			for(int i = 0; i < usedAnimations.Length; i++)
			{
				if(startAtTransition[i] == 0)
				{
					currentLegacyAnimator = legacyAnimators[i];
					currentLegacyAnimationClip = usedAnimations[i];
					currentLegacySetting = animOptions[i];
					FinishAction();
				}
			}

			//Debug.Log("Cutscene start got called here");
		}

		public void OnCutsceneEnterTransition(int transitionNumber)
		{
			for(int i = 0; i < usedAnimations.Length; i++)
			{
				//If we want to perform this action at this transition, do it
				if(startAtTransition[i] == transitionNumber)
				{
					currentLegacyAnimator = legacyAnimators[i];
					currentLegacyAnimationClip = usedAnimations[i];
					currentLegacySetting = animOptions[i];
					FinishAction();
				}
			}

			//Debug.Log("We have entered the start of transition # " + transitionNumber);
		}

		public void OnCutsceneEnd()
		{
			//Perform the action if the option is meant to be performed on cutscene end
			for(int i = 0; i < usedAnimations.Length; i++)
			{
				if(startAtTransition[i] == cutsceneEventAmount)
				{
					currentLegacyAnimator = legacyAnimators[i];
					currentLegacyAnimationClip = usedAnimations[i];
					currentLegacySetting = animOptions[i];
					FinishAction();
				}
			}
		}

		void FinishAction()
		{
			switch(currentLegacySetting)
			{
				case LegacyAnimationOptions.Crossfade:
				currentLegacyAnimator.CrossFade(currentLegacyAnimationClip);
				break;

				case LegacyAnimationOptions.Play:
				currentLegacyAnimator.Play(currentLegacyAnimationClip);
				break;
			}
		}
	}

}