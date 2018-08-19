/************************************************
Created By:		Ben Cutler
Company:		Tetricom Studios
Product:		Top Down Sword Controller
Date:			10/9/17
*************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Player))]
public class TopDownSwordController : MonoBehaviour {

	public float swordDamage;
	public Transform swordPivot;
	public Transform sword;
	public float smoothTime = 0.1f;
	public LayerMask groundMask;

	private Player player;
	private Camera cam;
	private TopDownSwordCollider swordCollider;
	private float swordPivotSmoothing;
	private float swordRotationSmoothing;

	private float previousTargetAngle;

	private int velocity;

	private void Awake () {
		player = GetComponent<Player> ();
		cam = Camera.main;
		swordCollider = sword.GetComponent<TopDownSwordCollider> ();
		swordCollider.damage = swordDamage;
		Cursor.lockState = CursorLockMode.Confined;
	}

	private void Update () {
		if (!player.dead) {
			SwordControlls ();
		}
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Cursor.lockState = CursorLockMode.None;
		}
	}

	private void SwordControlls () {
		Ray ray = cam.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, groundMask)) {
			Vector3 lookPoint = hit.point + Vector3.up * swordPivot.position.y;
			Vector3 direction = (lookPoint - transform.position).normalized;
			int sideMultiplier = (lookPoint.x < transform.position.x) ? -1 : 1;
			float targetAngle = Vector3.Angle (Vector3.forward, direction) * sideMultiplier;
			float nextAngle = Mathf.SmoothDampAngle (swordPivot.eulerAngles.y, targetAngle, ref swordPivotSmoothing, smoothTime);

			float targetSwordAngle = 0;
			if (targetAngle < previousTargetAngle) {
				targetSwordAngle = -90;
			} else if (targetAngle > previousTargetAngle) {
				targetSwordAngle = 90;
			}
			float swordAngle = Mathf.SmoothDampAngle (swordPivot.eulerAngles.z, targetSwordAngle, ref swordRotationSmoothing, smoothTime);
			swordPivot.localRotation = Quaternion.Euler (swordAngle * Vector3.forward + nextAngle * Vector3.up);

			velocity = Mathf.RoundToInt (Mathf.Abs (targetAngle - previousTargetAngle));
			swordCollider.velocity = velocity;
			previousTargetAngle = targetAngle;
		}
	}
}