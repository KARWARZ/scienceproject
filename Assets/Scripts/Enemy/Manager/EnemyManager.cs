﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;

/// Created By Timo Heijne
/// <summary>
/// This script keeps track of our enemies. Also when in crazy mode how many kills one got
/// </summary>
public class EnemyManager : MonoBehaviour {

	public static EnemyManager instance;

	public Enemy[] enemies;
	private Enemy activeEnemy;
	
	public enum Gamemodes {
		Normal,
		Crazy
	}
	public Gamemodes mode= Gamemodes.Normal;
	
	// Use this for initialization
	void Start () {
		SpawnEnemies();
		if (EnemyManager.instance != null) {
			Destroy(gameObject);
		} else {
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	void SpawnEnemies() {
		foreach (var enemy in enemies) {
			if (enemy.activeGameObject == null) {
				enemy.activeGameObject = Instantiate(enemy.prefab);
				enemy.healthScript = enemy.activeGameObject.GetComponent<Health>();
				enemy.healthScript.CurHealth = enemy.health;
				enemy.activeGameObject.name = enemy.name;
				enemy.activeGameObject.SetActive(false);

				enemy.healthScript.HasDied += RegisterDeath;
			}
		}
	}

	public void SetMode(Gamemodes newMode) {
		mode = newMode;
	}

	private void RegisterDeath() {
		activeEnemy.killed += 1;
		for (int i = 0; i < enemies.Length; i++) {
			if (enemies[i].name == activeEnemy.name) {
				// Why don't we have IndexOf?
				if (i == enemies.Length - 1) {
					if (mode == Gamemodes.Normal) {
						// Reached last enemy.. We done boi
						activeEnemy.activeGameObject.SetActive(false);
					} else {
						SetActiveEnemy(enemies[0]);
					}
				} else {
					SetActiveEnemy(enemies[i+1]);
				}
			}
		}
	}

	private void SetActiveEnemy(Enemy enemy) {
		activeEnemy.activeGameObject.SetActive(false);
		activeEnemy = enemy;
		activeEnemy.healthScript.CurHealth = activeEnemy.health;
		activeEnemy.activeGameObject.SetActive(true);
		
	}

	public void ResetStats() {
		foreach (var enemy in enemies) {
			enemy.killed = 0;
			enemy.healthScript.CurHealth = enemy.health;
		}
	}
}