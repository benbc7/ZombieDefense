/************************************************
Created By:		Ben Cutler
Company:		Tetricom Studios
Product:
Date:
*************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonSwordCollider : MonoBehaviour {

	public float damage;

	public bool sounds;
	public AudioClip[] swordSound;
	public AudioSource audioSource;

	private void OnCollisionEnter (Collision collision) {
		IDamagable enemy = collision.transform.GetComponent<IDamagable> ();
		if (enemy != null) {
			enemy.TakeDamage (damage);
			if (sounds) {
				audioSource.PlayOneShot (swordSound [Random.Range (0, swordSound.Length)]);
				audioSource.pitch = Random.Range (0.75f, 1.25f);
				audioSource.volume = Random.Range (0.25f, 1f);
			}
		}
	}
}