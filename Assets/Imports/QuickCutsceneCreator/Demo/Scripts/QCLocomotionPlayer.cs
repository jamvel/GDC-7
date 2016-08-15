 /// <summary>
/// 
/// </summary>

using UnityEngine;
using System;
using System.Collections;

namespace QuickCutscene.Demo {
  
[RequireComponent(typeof(Animator))]  

//Name of class must be name of file as well

public class QCLocomotionPlayer : MonoBehaviour {

    protected Animator animator;

    private float speed = 0;
    private float direction = 0;
    private QuickCutscene.Demo.QCLocomotion locomotion = null;

	// Use this for initialization
	void Start () 
	{
        animator = GetComponent<Animator>();
        locomotion = new QCLocomotion(animator);
	}
    
	void Update () 
	{
        if (animator && Camera.main)
		{
            JoystickToEvents.Do(transform,Camera.main.transform, ref speed, ref direction);
            locomotion.Do(speed * 6, direction * 180);
		}		
	}
}

}