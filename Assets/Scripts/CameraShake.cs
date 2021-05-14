using UnityEngine;
using System.Collections;
 
public class CameraShake : MonoBehaviour {
	public float shakeAmount; // The amount to shake this frame.
	public float shakeDuration; // The duration this frame.
 
	float shakePercentage; // A percentage (0-1) representing the amount of shake to be applied when setting rotation.
	float startAmount; //The initial shake amount (to determine percentage), set when ShakeCamera is called.
	float startDuration; //The initial shake duration, set when ShakeCamera is called.
	bool isRunning = false;
 
	public bool smooth = true;
	public float smoothAmount = 5f;
    private Camera cam;
    private GameManager gm;
 
    private void Start() {
        cam = gameObject.GetComponent<Camera>();
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    public static float MapRange (float value, float from1, float from2, float to1, float to2) {
        /* https://forum.unity.com/threads/re-map-a-number-from-one-range-to-another.119437/ */
        // return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        // https://rosettacode.org/wiki/Map_range
        return (to1 + ((value - from1) * (to2 - to1) / (from2 - from1)));
    }

    public static float MapRangeClamped (float value, float from1, float from2, float to1, float to2) {
        return Mathf.Clamp(MapRange(value, from1, from2, to1, to2), to1, to2);
    }


	public void ShakeCamera(float amount, float duration, Vector3 origin) {
        Vector3 bounds = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        bounds.z = origin.z;

        Vector3 focus = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0f));
        focus.z = origin.z;
        
        Vector3 dist = focus - origin;
        
        float shakeFactor = 1 - MapRangeClamped(dist.magnitude, 0f, bounds.magnitude, 0.1f, 1f); // remap the distance the camera is from the origin source to an inverse 1f .. 0f
        float zoomFactor = MapRangeClamped(gm.maxFOV - cam.fieldOfView, gm.minFOV, gm.maxFOV, 0.2f, 1f);


        shakeAmount += amount * shakeFactor * zoomFactor; // Add to the current amount.
		startAmount = shakeAmount; // Reset the start amount, to determine percentage.
		shakeDuration += duration; // Add to the current time.
		startDuration = shakeDuration; // Reset the start time.

		if(!isRunning) StartCoroutine (Shake());//Only call the coroutine if it isn't currently running. Otherwise, just set the variables.
	}
 
 
	IEnumerator Shake() {
		isRunning = true;
 
		while (shakeDuration > 0.01f) {
			Vector3 rotationAmount = Random.insideUnitSphere * shakeAmount;//A Vector3 to add to the Local Rotation
			rotationAmount.z = 0;//Don't change the Z; it looks funny.
 
			shakePercentage = shakeDuration / startDuration;//Used to set the amount of shake (% * startAmount).
 
			shakeAmount = startAmount * shakePercentage;//Set the amount of shake (% * startAmount).
			shakeDuration = Mathf.Lerp(shakeDuration, 0, Time.deltaTime);//Lerp the time, so it is less and tapers off towards the end.
 
 
			if(smooth)
				transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(rotationAmount), Time.deltaTime * smoothAmount);
			else
				transform.localRotation = Quaternion.Euler (rotationAmount); //Set the local rotation the be the rotation amount.

			yield return null;
		}
		transform.localRotation = Quaternion.identity;//Set the local rotation to 0 when done, just to get rid of any fudging stuff.
		isRunning = false;
	}
 
}