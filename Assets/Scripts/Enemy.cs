using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	[SerializeField] float speed;
	float lastHit;

	[SerializeField] int health;
	[SerializeField] int damage;

	public bool followAdventurers = true;

	[SerializeField] bool knocked = false;
	float knockedTime;
	float kbForce; // 10
	float kbTime; // 1
	Vector3 knockback;

	[SerializeField] bool stunned = false;
	float stunnedTime;
	float stunTime;

	[SerializeField] bool slowed = false;
	float slowedTime;
	float slowForce;
	float slowTime;

	public int Health { get { return health; } set { health = value; } }

	void Awake() {
	}
	
	void FixedUpdate() {
		float step = speed * Time.deltaTime;
		if (followAdventurers && !knocked) {
			Follow(step);
		}
		CheckForStatuses();
	}

	private void CheckForStatuses() {
		if(knocked) {
			followAdventurers = false;
			transform.Translate(knockback * (Time.deltaTime * kbForce), Space.World);
			knockedTime += Time.deltaTime;
			if (knockedTime >= kbTime) {
				knocked = false;
				followAdventurers = true;
				knockedTime = 0;
			}
		}
		if(stunned) {
			followAdventurers = false;
			stunnedTime += Time.deltaTime;
			if(stunnedTime >= stunTime) {
				stunned = false;
				followAdventurers = true;
				stunnedTime = 0;
			}
		}
		if (slowed) {
			slowedTime += Time.deltaTime;
			if(slowedTime >= slowTime) {
				slowed = false;
				speed += slowForce;
				slowedTime = 0;
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
			//GetShot(col);
		}
	}
	/*
	private void GetShot(Collider col) {
		Bullet bullet = col.GetComponent<Bullet>();
		bullet.TriggerEffects();
		bullet.EnemiesHit++;
		health -= bullet.Damage;
		CheckIfDead();
	}
	*/
	public void GetKnocked(Vector3 centre, float time, float force) {
		kbTime = time;
		knockback = gameObject.transform.position - centre;
		Debug.Log(knockback);
		knocked = true;
		kbForce = force;
	}

	public void GetStunned(float time) {
		stunTime = time;
		stunned = true;
	}

	public void GetSlowed(float time, float force) {
		slowTime = time;
		if (!slowed) {
			speed -= force;
			slowed = true;
		}
		slowForce = force;
	}

	public void CheckIfDead() {
		if(health <= 0) {
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

	private void OnDrawGizmos() {
		Gizmos.color = Color.blue;
		Gizmos.DrawRay(transform.position, knockback);
	}
}
