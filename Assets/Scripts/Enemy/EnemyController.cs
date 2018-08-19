/************************************************
Created By:		Ben Cutler
Company:		Tetricom Studios
Product:
Date:
*************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : Unit, IDamagable {

	[Header ("Enemy Fields")]
	public bool drawGizmos;
	public float startingHealth;
	public Image healthBar;

	[Tooltip ("Attack hits three times in the animation")]
	public float attackDamage;
	public float attackRange;
	public float attackDelay;
	public Transform attackTransform;
	public float attackColliderSize;
	public LayerMask attackLayerMask;

	private float currentHealth;
	private bool dead = false;
	private Animator animator;
	private bool attacking;
	private IDamagable player;

	private void Start () {
		animator = transform.GetChild (0).GetComponent<Animator> ();
		attackTransform = transform.GetChild (1);
		player = target.GetComponent<IDamagable> ();
		currentHealth = startingHealth;
		StartFollowing ();
	}

	private void LateUpdate () {
		if (followingPath && !dead) {
			float distance = Vector3.Distance (transform.position, target.position);
			if (distance < attackRange) {
				StartAttacking ();
			}
		}
	}

	private void StartFollowing () {
		attacking = false;
		animator.SetBool ("Walking", true);
		StopAllCoroutines ();
		StartCoroutine (UpdatePath ());
	}

	private void StartAttacking () {
		followingPath = false;
		animator.SetBool ("Walking", false);
		StopAllCoroutines ();
		StartCoroutine (Attack ());
	}

	private IEnumerator Attack () {
		attacking = true;
		while (attacking && !dead) {
			animator.SetTrigger ("Attack");
			yield return new WaitForSeconds (0.1f);
			if (Physics.CheckSphere (attackTransform.position, 0.25f * transform.localScale.z, attackLayerMask)) {
				player.TakeDamage (attackDamage);
			}
			yield return new WaitForSeconds (0.4f);
			if (Physics.CheckSphere (attackTransform.position, 0.25f * transform.localScale.z, attackLayerMask)) {
				player.TakeDamage (attackDamage / 2);
			}
			yield return new WaitForSeconds (0.25f);
			if (Physics.CheckSphere (attackTransform.position, 0.25f * transform.localScale.z, attackLayerMask)) {
				player.TakeDamage (attackDamage / 2);
			}

			yield return new WaitForSeconds (attackDelay);
			if (Vector3.Distance (transform.position, target.position) > attackRange) {
				break;
			}
		}

		StartFollowing ();
	}

	private void Die () {
		dead = true;
		attacking = false;
		followingPath = false;
		StopAllCoroutines ();
		GetComponent<CapsuleCollider> ().enabled = false;
		animator.SetTrigger ("Die");
		GameObject.Find ("GameManager").SendMessage ("EnemyDied");
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

	public void OnDrawGizmos () {
		if (drawGizmos) {
			DrawWithGizmos ();
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere (transform.position + Vector3.up, attackRange);
			Gizmos.DrawSphere (attackTransform.position, attackColliderSize);
		}
	}
}