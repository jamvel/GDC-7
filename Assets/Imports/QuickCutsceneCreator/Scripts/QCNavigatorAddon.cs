using UnityEngine;
using System.Collections;

/// <summary>
/// This script adds NavMeshAgent options for each event in a cutscene.
/// Requires a Quick Cutscene Controller component
/// </summary>

namespace QuickCutscene.Utils {
	
	public enum NavAgentActions {SetDestination, Stop, Warp};
	
	[RequireComponent(typeof(QuickCutsceneController))]
	public class QCNavigatorAddon : MonoBehaviour {
		
		//Variables Start ====================================================================
		
		//Type of action we want the agent to perform
		public NavAgentActions[] navActions;
		
		//The NavMeshAgent components of each object
		public NavMeshAgent[] navAgents;
		
		//Used for dropdown selction
		public bool[] showEvent;
		
		//The amount of events in the cutscene that we are attached to
		public int cutsceneEventAmount;
		
		//Checks what position in the cutscene we want to perform each action at
		public int[] startAtTransition;
		
		//Delay to wait before performing each action
		public float[] actionStartDelay;
		
		//The array of transforms used for the navmeshagent destinations
		public Transform[] navDestinations;
		
		//Used for setting the speed of the navmeshagents
		public float[] navAgentSpeed = new float[1] {3.5f};

		//Do we want to pre-calc paths?
		public bool preCalcPaths;

		//Quick references
		private NavAgentActions currentNavAction;
		private NavMeshAgent currentNavAgent;
		private float currentDelayTime;
		private Vector3 currentNavDest;
		private float currentNavSpeed;
		
		
		//Variables End ======================================================================
		
		void OnCutsceneStart()
		{
			if(preCalcPaths)
			{
				//Hack to make the paths pre-generate - NavMeshAgent.CalculatePath doesn't want to store anywhere
				for(int i = 0; i < navActions.Length; i++)
				{
					if(navAgents[i] != null)
					{
						if(navActions[i] == NavAgentActions.SetDestination)
						{
							navAgents[i].SetDestination(navDestinations[i].position);
							navAgents[i].speed = 0f;
						}
					} else {
						Debug.LogWarning ("Nav agent # " + i + " was not set");
					}					
				}
			}

			//Perform the action if the option is meant to be performed on cutscene start
			for(int i = 0; i < navActions.Length; i++)
			{
				if(startAtTransition[i] == 0)
				{
					SetCurrentActions(i);
					
					//Finish the action, with a delay if the action has a delay set
					if(currentDelayTime == 0)
					{						
						FinishAction();
					} else 
					{
						StartCoroutine("FinishAfterDelay", currentDelayTime);
					}
					
				}
			}
			
			//Debug.Log("Cutscene start got called here");
		}
		
		void OnCutsceneEnterTransition(int transitionNumber)
		{
			for(int i = 0; i < navActions.Length; i++)
			{
				if(startAtTransition[i] == transitionNumber)
				{
					SetCurrentActions(i);
					
					//Finish the action, with a delay if the action has a delay set
					if(currentDelayTime == 0)
					{						
						FinishAction();
					} else 
					{
						StartCoroutine("FinishAfterDelay", currentDelayTime);
					}
				}
			}
			
		}
		
		void OnCutsceneEnd()
		{
			//Perform the action if the option is meant to be performed on cutscene end
			for(int i = 0; i < navActions.Length; i++)
			{
				if(startAtTransition[i] == cutsceneEventAmount)
				{
					SetCurrentActions(i);
					
					//Finish the action, with a delay if the action has a delay set
					if(currentDelayTime == 0)
					{						
						FinishAction();
					} else {
						Debug.LogWarning("You are trying to delay an event that is called at the end of a cutscene");
						StartCoroutine("FinishAfterDelay", currentDelayTime);
					}
				}
			}
			
			StopCoroutine("FinishAfterDelay");
		}

		void SetCurrentActions(int actionIndex)
		{
			if(navAgents[actionIndex] == null || navDestinations[actionIndex] == null)
			{
				Debug.LogWarning("Nav agent or destination # " + actionIndex + " was not set");
				return;
			}
			currentNavAction = navActions[actionIndex];
			currentNavAgent = navAgents[actionIndex];
			currentDelayTime = actionStartDelay[actionIndex];
			currentNavDest = navDestinations[actionIndex].position;
			currentNavSpeed = navAgentSpeed[actionIndex];
		}
		
		void FinishAction()
		{			
			if(currentNavAgent != null)
			{
				switch(currentNavAction)
				{
				case NavAgentActions.SetDestination:
					if(!preCalcPaths)
					{
						currentNavAgent.SetDestination(currentNavDest);
					}
					currentNavAgent.speed = currentNavSpeed;
					//Debug.Log("Set Nav dest");
					break;
					
				case NavAgentActions.Stop:
					currentNavAgent.Stop();
					break;
					
				case NavAgentActions.Warp:
					currentNavAgent.Warp(currentNavDest);
					break;
				}
			} else {
				Debug.LogWarning("No current Nav Agent was found");
			}
			
		}
		
		IEnumerator FinishAfterDelay(float delay)
		{
			yield return new WaitForSeconds(delay);
			
			FinishAction();
		}
		
	}
	
}