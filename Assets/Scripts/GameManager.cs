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
using UnityEngine.SceneManagement;

public enum MenuState {
	UnPaused, Paused, PlayerDied
}

public class GameManager : MonoBehaviour {

	public GameObject smallEnemy;
	public Transform[] spawnPoints;
	public Player player;
	public Text enemyCounter;
	public GameObject menu;
	public Text menuTitleText;

	private MenuState menuState = MenuState.UnPaused;
	private int sceneIndex;
	private int enemiesKilled;

	private void Start () {
		sceneIndex = SceneManager.GetActiveScene ().buildIndex;
		ChangeMenuState (MenuState.UnPaused);
		StartCoroutine (SpawnZombies ());
	}

	private void Update () {
		if (Input.GetKeyDown (KeyCode.Escape) && menuState != MenuState.PlayerDied) {
			ChangeMenuState ((menuState == MenuState.UnPaused) ? MenuState.Paused : MenuState.UnPaused);
		}
	}

	private IEnumerator SpawnZombies () {
		while (!player.dead) {
			int i = Random.Range (0, spawnPoints.Length);
			Instantiate (smallEnemy, spawnPoints [i].position, Quaternion.identity);
			yield return new WaitForSeconds (Random.Range (0.5f, 5f));
		}
	}

	public void ChangeMenuState (MenuState state) {
		bool hasCursor = state != MenuState.UnPaused || sceneIndex == 0;
		Cursor.visible = hasCursor;
		Cursor.lockState = (hasCursor) ? CursorLockMode.None : CursorLockMode.Locked;
		menu.SetActive (state != MenuState.UnPaused);
		menuTitleText.text = (state == MenuState.Paused) ? "Paused" : "You Died";
		menuState = state;
	}

	public void ExitGame () {
		Application.Quit ();
	}

	public void LoadScene (int sceneIndex) {
		SceneManager.LoadScene (sceneIndex);
	}

	private void EnemyDied () {
		enemiesKilled++;
		enemyCounter.text = "Zombies Killed: " + enemiesKilled;
	}
}