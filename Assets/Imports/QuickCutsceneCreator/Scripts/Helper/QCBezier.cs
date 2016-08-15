using UnityEngine;
using System.Collections;

/// <summary>
/// Used for the creation of bezier points. 
/// Bezier2 is a quadratic curve, and in practice will give less weight to the
/// midpoint, and cost less than Bezier3
/// Bezier3 is a cubic curve, in practice it will give more weight to the midpoint,
/// although cost slightly more
/// 
/// Credit goes to Bunny83 on Stackexchange for original javascript implementations.
/// </summary>

namespace QuickCutscene.Utils {
	
	/// <summary>
	/// Path type for gizmo/handle drawing
	/// </summary>
	public enum PathType {None=1, PathOnly=2, PathAndFrustum=3};	
	
	/// <summary>
	/// Enumerations for the camera speed options, accessed by the QuickTakeCutsceneControllerEditor script
	/// </summary>
	public enum CameraSpeedOptions {Slow, Medium, Fast, Faster, Custom, Curve, MobileCurve, Instant, Lerp};
	public enum CameraRotationSpeedOptions {SuperSlow, Slow, Medium, Fast, Faster, SuperFast, Custom, Instant, FollowTarget, Lerp};

	public class QCBezier {

		//"Mobile" curve (2-point bezier)
		public static Vector3 Bezier2(float t, Vector3 Start, Vector3 Control, Vector3 End)
		{
			return (((1-t)*(1-t)) * Start) + (2 * t * (1 - t) * Control) + ((t * t) * End);
		}

		//Regular 3-point bezier curve
		public static Vector3 Bezier3(float t, Vector3 s, Vector3 st, Vector3 et, Vector3 e)
		{
			return (((-s + 3*(st-et) + e)* t + (3*(s+et) - 6*st))* t + 3*(st-s))* t + s;
		}

	}
}
