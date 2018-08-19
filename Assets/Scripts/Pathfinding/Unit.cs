using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

	private const float minPathUpdateTime = 0.2f;
	private const float pathUpdateMoveThreshold = 0.5f;

	[Header ("Pathfinding Fields")]
	public float speed = 20;
	public float turnSpeed = 3;
	public float turnDistance = 5;
	public float stoppingDistance = 10;

	protected Transform target;

	protected Path path;
	protected bool followingPath;

	private void Awake () {
		target = GameObject.FindGameObjectWithTag ("Player").transform;
	}

	public void OnPathFound (Vector3 [] waypoints, bool pathSuccessful) {
		if (pathSuccessful) {
			path = new Path (waypoints, transform.position, turnDistance, stoppingDistance);

			StopCoroutine ("FollowPath");
			StartCoroutine ("FollowPath");
		}
	}

	protected IEnumerator UpdatePath () {

		if (Time.timeSinceLevelLoad < 0.3f) {
			yield return new WaitForSeconds (0.3f);
		}
		PathRequestManager.RequestPath (new PathRequest (transform.position, target.position, OnPathFound));

		float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
		Vector3 targetPositionOld = target.position;

		while (true) {
			yield return new WaitForSeconds (minPathUpdateTime);
			if ((target.position - targetPositionOld).sqrMagnitude > sqrMoveThreshold) {
				PathRequestManager.RequestPath (new PathRequest (transform.position, target.position, OnPathFound));
				targetPositionOld = target.position;
			}
		}
	}

	private IEnumerator FollowPath () {

		followingPath = true;
		int pathIndex = 0;
		transform.LookAt (path.lookPoints [0]);

		float speedPercent = 1;

		while (followingPath) {
			Vector2 pos2D = new Vector2 (transform.position.x, transform.position.z);
			while (path.turnBoundaries [pathIndex].HasCrossedLine (pos2D)) {
				if (pathIndex == path.finishLineIndex) {
					followingPath = false;
					break;
				} else {
					pathIndex++;
				}
			}

			if (followingPath) {

				if (pathIndex >= path.slowDownIndex && stoppingDistance > 0) {
					speedPercent = Mathf.Clamp01 (path.turnBoundaries [path.finishLineIndex].DistanceFromPoint (pos2D) / stoppingDistance);
					if (speedPercent < 0.01f) {
						followingPath = false;
					}
				}

				Quaternion targetRotation = Quaternion.LookRotation (path.lookPoints [pathIndex] - transform.position);
				transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
				transform.Translate (Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);
			}

			yield return null;
		}
	}

	public void DrawWithGizmos () {
		if (path != null) {
			path.DrawWithGizmos ();
		}
	}
}