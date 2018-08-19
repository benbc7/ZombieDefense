using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	private const float Y_ANGLE_MIN = 0f;
	private const float Y_ANGLE_MAX = 50f;

	[Header ("Camera Behaviour")]
	public LayerMask layerMask;
	public float camHeight = 1f;
	public float sensitivityX = 0f;
	public float sensitivityY = 0f;

	[Header ("Field of View")]
	public float defaultFOV = 68f;
	public float runningFOV = 80f;
	public float fovSmoothTime = 0.2f;

	private float currentFOV;
	private bool running = false;
	private float smoothVelocity;

	private Transform player;
	private Transform camTransform;

	private Camera cam;

	private float maxDistance = 7.5f;
	private float distance = 0f;
	private float currentX = 0f;
	private float currentY = 0f;

	public void GetPlayer (Transform playerTransform) {
		player = playerTransform;
	}

	private void Start () {
		camTransform = transform;
		cam = GetComponent<Camera> ();
		currentFOV = defaultFOV;
	}

	public void SetRunning () {
		running = !running;
		StopCoroutine (FOVChange ());
		StartCoroutine (FOVChange ());
	}

	private IEnumerator FOVChange () {
		while (true) {
			currentFOV = Mathf.SmoothDamp (currentFOV, (running) ? runningFOV : defaultFOV, ref smoothVelocity, fovSmoothTime);
			cam.fieldOfView = currentFOV;
			if (running && currentFOV >= runningFOV - 1) {
				break;
			} else if (!running && currentFOV <= defaultFOV + 1) {
				break;
			}
			yield return null;
		}
	}

	private void Update () {
		if (player != null) {
			RaycastHit hit;

			currentX += Input.GetAxis ("Mouse X") * sensitivityX;
			currentY += -Input.GetAxis ("Mouse Y") * sensitivityY;

			currentY = Mathf.Clamp (currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);

			//Debug.DrawRay (player.position, transform.TransformDirection (Vector3.back) * maxDistance, Color.red);
			if (Physics.Raycast (player.position, transform.TransformDirection (Vector3.back), out hit, maxDistance, layerMask)) {
				distance = hit.distance;
			} else
				distance = maxDistance;
		}
	}

	private void LateUpdate () {
		if (player != null) {
			Vector3 dir = new Vector3 (0, 0, -distance);
			Quaternion rotation = Quaternion.Euler (currentY, currentX, 0);
			camTransform.position = player.position + rotation * dir + Vector3.up * camHeight;

			camTransform.LookAt (player.position + Vector3.up * camHeight);
			player.rotation = Quaternion.Euler (0, currentX, 0);
		}
	}
}