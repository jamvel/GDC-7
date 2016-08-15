using UnityEngine;
using System.Collections;

/// <summary>
/// Used to make the scene view camera move to simulate a QuickCutscene in-editor view.
/// Adapted from http://wiki.unity3d.com/index.php/SceneViewCameraFollower
/// Only updates when it is unfolded (updates via OnDrawGizmos)
/// </summary>

namespace QuickCutscene.utils {

	[ExecuteInEditMode]
	public class QCSceneViewCameraFollow : MonoBehaviour 
	{
	#if UNITY_EDITOR

		[SerializeField] bool on = true;
		[SerializeField] bool showLogs;
		[SerializeField] bool onlyInPlayMode = false;
		[SerializeField] SceneViewFollower[] sceneViewFollowers;
		private ArrayList sceneViews;

		public void OnCutsceneStart()
		{
			if(showLogs)
			Debug.Log("Reached cutscene start at time " + Time.time);
		}

		public void OnCutsceneEnterTransition(int transitionNumber)
		{	
			if(showLogs)
			Debug.Log("Reached cutscene transition #" + transitionNumber + " at time " + Time.time);
		}
		
		public void OnCutsceneEnd()
		{
			if(showLogs)
			Debug.Log("Reached cutscene end");
		}

		void LateUpdate()
		{
			if(sceneViewFollowers != null && sceneViews != null)
			{
				foreach(SceneViewFollower svf in sceneViewFollowers)
				{
					if(svf.targetTransform == null) svf.targetTransform = transform;
					svf.size = Mathf.Clamp(svf.size, .01f, float.PositiveInfinity);
					svf.sceneViewIndex = Mathf.Clamp(svf.sceneViewIndex, 0, sceneViews.Count-1);
				}
			}
			
			if(Application.isPlaying)
				Follow();
		}

		public void OnDrawGizmos()
		{
			if(!Application.isPlaying)
				Follow();
		}

		void Follow()
		{
			sceneViews = UnityEditor.SceneView.sceneViews;
			if(sceneViewFollowers == null || !on || sceneViews.Count == 0) return;
			
			foreach(SceneViewFollower svf in sceneViewFollowers)
			{	
				if(!svf.enable) continue;
				UnityEditor.SceneView sceneView = (UnityEditor.SceneView) sceneViews[svf.sceneViewIndex];
				if(sceneView != null)
				{
					if((Application.isPlaying && onlyInPlayMode) || !onlyInPlayMode)
					{
						sceneView.orthographic = svf.orthographic;
						sceneView.LookAtDirect(svf.targetTransform.position + svf.positionOffset, (svf.enableFixedRotation) ? Quaternion.Euler(svf.fixedRotation) : svf.targetTransform.rotation, svf.size);	
					}
				}
			}	
		}

		[System.Serializable]
		public class SceneViewFollower
		{
			public bool enable;
			public Vector3 positionOffset;
			public bool enableFixedRotation;
			public Vector3 fixedRotation;
			public Transform targetTransform;
			public float size;
			public bool orthographic;
			public int sceneViewIndex;
			
			SceneViewFollower()
			{
				enable = false;
				positionOffset = Vector3.zero;
				enableFixedRotation = false;
				fixedRotation = Vector3.zero;
				size = 5;
				orthographic = false;
				sceneViewIndex = 0;
			}
		}

	#endif
	}

}