﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	[SerializeField] float speed;
	float lastHit;

	[SerializeField] int health;
	[SerializeField] int damage;

	public bool followAdventurers = true;

	bool knocked = false;
	float knockedTime;
	Vector3 knockback;
	float kbForce; // 10
	float kbTime; // 1

	bool stunned = false;
	float stunnedTime;
	float stunTime;

	public int Health { get { return health; } set { health = value; } }

	void Awake () {
	}
	
	void FixedUpdate () {
		float step = speed * Time.deltaTime;
		if (followAdventurers && !knocked) {
			Follow(step);
		}
		CheckForStatuses();
	}

	private void CheckForStatuses() {
		if (knocked) {
			transform.Translate(knockback * (Time.deltaTime * kbForce));
			knockedTime += Time.deltaTime;
			if (knockedTime >= kbTime) {
				knocked = false;
				knockedTime = 0;
			}
		}
		if (stunned) {
			Debug.Log("I have been stunned!");
			followAdventurers = false;
			stunnedTime += Time.deltaTime;
			if(stunnedTime >= stunTime) {
				followAdventurers = true;
				stunned = false;
				stunnedTime = 0;
			}
		}
	}

	private void Follow(float step) {
		transform.LookAt(GetClosestEnemy(GameManager.instance.Adventurers));
		transform.position = Vector3.MoveTowards(transform.position, GetClosestEnemy(GameManager.instance.Adventurers).position, step);
	}

	private void OnCollisionEnter(Collision col) {
		if(col.gameObject.tag == "Adventurer" && GameManager.instance._Time >= lastHit + 2) {
			DealDamage(col);
		}
	}

	private void OnCollisionStay(Collision col) {
		if (col.gameObject.tag == "Adventurer" && GameManager.instance._Time >= lastHit + 2) {
			DealDamage(col);
		}
	}

	//Trigger to detect when a bullet makes contact
	//Destroys bullet and takes away health
	private void OnTriggerEnter(Collider col) {
		if(col.gameObject.tag == "AdventurerProjectile") {
			GetShot(col);
		}
	}

	private void GetShot(Collider col) {
		Bullet bullet = col.GetComponent<Bullet>();
		bullet.TriggerEffects();
		bullet.EnemiesHit++;
		health -= bullet.Damage;
		CheckIfDead();
	}

	public void GetKnocked(Vector3 centre, float time, float force) {
		knockback = gameObject.transform.position - centre;
		knockback = new Vector3(knockback.x, 0, knockback.z);
		kbTime = time;
		kbForce = force;
		knocked = true;
	}

	public void GetStunned(float time) {
		stunned = true;
		stunTime = time;
	}

	public void CheckIfDead() {
		if (health <= 0) {
			ZombieDead();
		}
	}

	private void DealDamage(Collision col) {
		Debug.Log("Adventurer hit!");
		col.gameObject.GetComponent<Adventurer>().TakeDamage(damage);
		lastHit = GameManager.instance._Time;
	}

	//Simple function for now, designed as such for potential modular coding later
	private void ZombieDead() {
		Destroy(gameObject);
	}

	//Transform takes the adventurers array from GameManager and looks at each of them, determining the closest, non-downed adventurer to them using the magic of maths
	Transform GetClosestEnemy(List<GameObject> adventurers) {
		Transform bestTarget = null;
		Vector3 currentPos = transform.position;
		float closestDistanceSqr = Mathf.Infinity;
		foreach(GameObject a in adventurers) {
			if (!a.GetComponent<Adventurer>().Downed) { 
				Vector3 directionToTarget = a.transform.position - currentPos;
				float dSqrToTarget = directionToTarget.sqrMagnitude;
				if(dSqrToTarget < closestDistanceSqr) {
					closestDistanceSqr = dSqrToTarget;
					bestTarget = a.transform;
				}
			}
		}
		return bestTarget;
	}
}
