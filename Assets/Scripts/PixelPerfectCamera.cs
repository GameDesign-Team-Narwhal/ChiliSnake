using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera/Pixel Perfect Camera")]
public class PixelPerfectCamera : MonoBehaviour {

	public float pixelsToUnits = 1f;
	public static float scale = 1f;

	public float nativeHeight = 160;

	void Awake () {
		var camera = GetComponent<Camera> ();

        //this may have been set by a previous scene, so we have to reset it
        scale = 1f;

		if (camera.orthographic) {
			scale = Screen.height/nativeHeight;
			pixelsToUnits *= scale;
			camera.orthographicSize = (Screen.height / 2.0f) / pixelsToUnits;

            Debug.Log("pixelsToUnits = " + pixelsToUnits + " scale = " + scale);
		}
	}

}
