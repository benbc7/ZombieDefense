/************************************************
Created By:		Ben Cutler
Company:		Tetricom Studios
Product:		RigidBody Controller
Date:			10/8/17
*************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (PlayerController))]
public class Player : MonoBehaviour, IDamagable {

	[Header ("Movement")]
	public float moveSpeed = 5;
	public float runMultiplier = 1.5f;
	public float accelerationTimeGrounded = 0.1f;
	public bool thirdPerson;

	[Header ("Jumping")]
	public float jumpHeight = 4;
	public float timeToJumpApex = 0.4f;
	public float accelerationTimeAirborne = 0.2f;

	[Header ("Health")]
	public float startingHealth = 10f;
	public Image healthBar;

	private float currentHealth;
	public bool dead;
	private bool attacking;
	private PlayerController playerController;
	private CameraController cameraController;
	private Animator swordAnimator;
	private Vector3 moveVelocity;
	private float jumpVelocity;
	private float gravity;
	private float currentRunMultiplier = 1f;
	private float velocityXSmoothing;
	private float velocityZSmoothing;

	private void Start () {
		playerController = GetComponent<PlayerController> ();
		gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		jumpVelocity = Mathf.Abs (gravity) * timeToJumpApex;
		Physics.gravity = Vector3.up * gravity;
		currentHealth = startingHealth;

		if (thirdPerson) {
			healthBar.rectTransform.parent.rotation = Quaternion.Euler (Vector3.zero);
			swordAnimator = GetComponent<Animator> ();
			cameraController = Camera.main.GetComponent<CameraController> ();
			cameraController.GetPlayer (transform);
			if (cameraController == null) {
				cameraController = Camera.main.gameObject.AddComponent<CameraController> ();
			}
		}
	}

	private void Update () {
		if (!dead) {
			RunInput ();
			MoveInput ();

			if (thirdPerson && !attacking) {
				SwordInput ();
			}
		}
	}

	private void SwordInput () {
		if (Input.GetMouseButtonDown (0)) {
			swordAnimator.SetTrigger ("Horizontal");
			attacking = true;
		} else if (Input.GetMouseButtonDown (1)) {
			swordAnimator.SetTrigger ("Vertical");
			attacking = true;
		}
	}

	public void ResetAttacking () {
		attacking = false;
	}

	private void MoveInput () {
		Vector3 moveInput = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
		currentRunMultiplier = (moveInput.z >= 0) ? currentRunMultiplier : 1;
		Vector3 targetVelocity = transform.TransformDirection (moveInput.normalized) * moveSpeed * currentRunMultiplier;

		if (attacking) {
			targetVelocity = Vector3.zero;
		}

		moveVelocity.x = Mathf.SmoothDamp (moveVelocity.x, targetVelocity.x, ref velocityXSmoothing, (playerController.grounded) ? accelerationTimeGrounded : accelerationTimeAirborne);
		moveVelocity.z = Mathf.SmoothDamp (moveVelocity.z, targetVelocity.z, ref velocityZSmoothing, (playerController.grounded) ? accelerationTimeGrounded : accelerationTimeAirborne);
		playerController.Move (moveVelocity);
	}

	private void RunInput () {
		if (Input.GetKeyDown (KeyCode.LeftShift)) {
			currentRunMultiplier = runMultiplier;
			if (thirdPerson) {
				cameraController.SetRunning ();
			}
		}
		if (Input.GetKeyUp (KeyCode.LeftShift)) {
			currentRunMultiplier = 1;
			if (thirdPerson) {
				cameraController.SetRunning ();
			}
		}
	}

	private void Die () {
		dead = true;
		GameObject.Find ("GameManager").GetComponent<GameManager> ().ChangeMenuState (MenuState.PlayerDied);
	}

	public void TakeDamage (float damage) {
		if (!dead) {
			currentHealth -= damage;
			healthBar.fillAmount = currentHealth / startingHealth;
			if (currentHealth <= 0) {
				Die ();
			}
		}
	}
}