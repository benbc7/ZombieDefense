/************************************************
Created By:		Ben Cutler
Company:		Tetricom Studios
Product:		RigidBody Controller
Date:			10/8/17
*************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (CapsuleCollider), typeof (Rigidbody))]
public class PlayerController : MonoBehaviour {

	private const float skinWidth = 0.015f;

	public int rayCount;
	public LayerMask wallMask;

	private Vector3 velocity;
	private Rigidbody rb;
	private CapsuleCollider capsule;

	[HideInInspector]
	public bool grounded;

	private Vector3 groundVelocity;

	private void Start () {
		rb = GetComponent<Rigidbody> ();
		capsule = GetComponent<CapsuleCollider> ();
	}

	private void FixedUpdate () {
		rb.AddForce (CalculateVelocityChange (), ForceMode.VelocityChange);
		grounded = false;
	}

	public void Move (Vector3 velocity) {
		this.velocity = velocity;
	}

	private Vector3 CalculateVelocityChange () {
		Vector3 currentRelativeVelocity = rb.velocity - groundVelocity;
		Vector3 velocityChange = velocity - currentRelativeVelocity;
		velocityChange.y = 0;
		return CheckMovementCollisions (velocityChange);
	}

	private Vector3 CheckMovementCollisions (Vector3 velocityChange) {
		RaycastHit hit;
		float spacing = capsule.height / rayCount;
		Vector3 bottom = new Vector3 (capsule.bounds.center.x, capsule.bounds.min.y + skinWidth, capsule.bounds.center.z);
		Vector3 direction = velocityChange.normalized;
		float distance = velocityChange.magnitude;
		for (int i = 0; i <= rayCount; i++) {
			Vector3 origin = bottom + Vector3.up * spacing * i;
			Debug.DrawLine (origin, origin + direction * distance);
			if (Physics.Raycast (origin, direction, out hit, distance, wallMask)) {
				if (hit.rigidbody != null) {
					if (hit.rigidbody.mass < 200) {
						continue;
					}
				}
				if (Vector3.Angle (hit.normal, Vector3.up) < 30) {
					continue;
				}
				Vector3 targetVelocityChange = (hit.point + hit.normal * capsule.radius) - origin;
				targetVelocityChange.y = 0;
				return targetVelocityChange;
			}
		}

		return velocityChange;
	}

	private void TrackGrounded (Collision collision) {
		var maxHeight = capsule.bounds.min.y + capsule.radius * .9f;
		foreach (var contact in collision.contacts) {
			if (contact.point.y < maxHeight) {
				if (isKinematic (collision)) {
					groundVelocity = collision.rigidbody.velocity;
					transform.parent = collision.transform;
				} else if (isStatic (collision)) {
					transform.parent = collision.transform;
				} else {
					groundVelocity = Vector3.zero;
				}
				grounded = true;
			} else if (Vector2.Distance (contact.point.XZ (), capsule.bounds.center.XZ ()) > capsule.radius - skinWidth) {
			}
			break;
		}
	}

	private bool isKinematic (Collision collision) {
		return isKinematic (collision.transform);
	}

	private bool isKinematic (Transform otherTransform) {
		Rigidbody otherRB = otherTransform.GetComponent<Rigidbody> ();
		return otherRB && otherRB.isKinematic;
	}

	private bool isStatic (Collision collision) {
		return isStatic (collision.transform);
	}

	private bool isStatic (Transform transform) {
		return transform.gameObject.isStatic;
	}

	private void OnCollisionExit (Collision collision) {
		if (collision.transform == transform.parent)
			transform.parent = null;
	}

	private void OnCollisionStay (Collision col) {
		TrackGrounded (col);
	}

	private void OnCollisionEnter (Collision col) {
		TrackGrounded (col);
	}

	private void OnDrawGizmos () {
		if (grounded) {
			Gizmos.color = Color.red;
			Gizmos.DrawCube (transform.position - Vector3.up * (capsule.height / 2f), new Vector3 (0.5f, 0.2f, 0.5f));
		}
	}
}