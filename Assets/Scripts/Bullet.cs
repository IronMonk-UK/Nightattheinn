/* 
 * Copyright (c) Iron Monk Studios 2022
 * www.ironmonkstudios.co.uk 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	[SerializeField] GameObject actor;
	//Adventurer adventurerClass;
	bool adventurer;
	bool enemy;

	[SerializeField] float thrust;
	[SerializeField] int outOfBoundsDistance;
	float time;

	int damage;
	int enemiesHit;

	[SerializeField] GameObject bulletPrefab;

	bool aoe;
	//int aoeDamage;
	float aoeRadius;
	bool aoeTriggered;
	float aoeStartTime;
	float aoeTime;
	bool aoeScale;
	bool aoeRotate;
	GameObject aoePrefab;
	Vector3 aoePrefabScale;
	bool pierce;
	int pierceAmount;
	int pierceCount = 0;

	bool knockback;
	float knockbackForce;
	float knockbackTime;
	bool slow;
	float slowForce;
	float slowTime;
	bool stun;
	float stunTime;

	bool triggered;
	bool destroyed = false;
	bool stop;
	
	Vector3 direction;
	Vector3 forward;

	public float Thrust { get { return thrust; } set { thrust = value; } }

	public GameObject Actor { get { return actor; } set { actor = value; } }
	public Vector3 Direction { get { return direction; } set { direction = value; } }
	public int Damage { get { return damage; } set { damage = value; } }
	public int EnemiesHit { get { return enemiesHit; } set { enemiesHit = value; } }

	public bool Aoe { get { return aoe; } set { aoe = value; } }
	//public int AoeDamage { get { return aoeDamage; } set { aoeDamage = value; } }
	public float AoeRadius { get { return aoeRadius; } set { aoeRadius = value; } }
	public GameObject AoePrefab { get { return aoePrefab; } set { aoePrefab = value; } }
	public bool AoeScale { get { return aoeScale; } set { aoeScale = value; } }
	public bool AoeRotate { get { return aoeRotate; } set { aoeRotate = value; } }
	//public float AoeTime { get { return aoeTime; } set { aoeTime = value; } }
	public bool Knockback { get { return knockback; } set { knockback = value; } }
	public float KnockbackForce { get { return knockbackForce; } set { knockbackForce = value; } }
	public float KnockbackTime { get { return knockbackTime; } set { knockbackTime = value; } }
	public bool Pierce { get { return pierce; } set { pierce = value; } }
	public int PierceAmount { get { return pierceAmount; } set { pierceAmount = value; } }
	public bool Slow { get { return slow; } set { slow = value; } }
	public float SlowForce { get { return slowForce; } set { slowForce = value; } }
	public float SlowTime { get { return slowTime; } set { slowTime = value; } }
	public bool Stun { get { return stun; } set { stun = value; } }
	public float StunTime { get { return stunTime; } set { stunTime = value; } }

	void Awake() {
		direction = new Vector3(0, transform.localPosition.y, 0);
	}

	void Start() {	
		forward = gameObject.transform.position - actor.transform.position;
		if (actor.tag == "Adventurer") {
			adventurer = true;
		} else if (actor.tag == "Enemy") {
			enemy = true;
		}
		if (aoe) {
			aoePrefabScale = new Vector3(aoeRadius, aoeRadius, aoeRadius);
			if (slow) {
				aoeTime = slowTime;
			}
			else if (stun) {
				aoeTime = stunTime;
			}
			else {
				aoeTime = 1;
			}
		}
	}

	void Update() {
		time += Time.deltaTime;
		if (!stop) {
			transform.Translate(new Vector3(forward.x, 0, forward.z) * (Time.deltaTime * thrust), Space.World);
		}
		if (aoeTriggered) {
			if (aoeScale && aoePrefab.transform.localScale.x <= (aoePrefabScale.x * 4)) {
				if (time < aoeStartTime + aoeTime) {
					aoePrefab.transform.localScale = Vector3.Lerp(aoePrefab.transform.localScale, aoePrefabScale * 4, (aoeTime * Time.deltaTime) * 3);
				}
			} else if (aoeRotate) {
				aoePrefab.transform.localScale = aoePrefabScale * 4;
				Debug.Log("Rotating AoE Object!");
				aoePrefab.transform.Rotate(0, 0.5f, 0);
			}
			if (time > aoeStartTime + aoeTime) {
				Destroy(gameObject);
			}
		}
		CheckOutOfBounds();
	}

	private void OnTriggerEnter(Collider col) {
		if (!triggered) {
			if (adventurer) {		
				if (col.gameObject.tag == "Enemy") {
					if (!pierce || (pierce && pierceCount == pierceAmount)) {
						Debug.Log("Enemy Hit");
						DeleteBullet();
						TriggerAffects(col);
						triggered = true;
					} else if (pierce && pierceCount <= PierceAmount) {
						TriggerAffects(col);
						pierceCount++;
					}
				}
			} else if (enemy) {
				if (col.gameObject.tag == "Adventurer") {
					DeleteBullet();
					TriggerAdventurerAffects(col);
					triggered = true;
				}
			}
			if (col.gameObject.tag == "Wall" || col.gameObject.tag == "Obstacle") {
			Debug.Log("A bullet has hit a wall");
				DeleteBullet();
				TriggerAffects(col);
				triggered = true;
			}
		}
	}

	private void CheckOutOfBounds() {
		if (transform.position.x >= outOfBoundsDistance || transform.position.x <= -outOfBoundsDistance || transform.position.z >= outOfBoundsDistance || transform.position.z <= -outOfBoundsDistance) {
			Destroy(gameObject);
		}
	}

	private void TriggerAdventurerAffects(Collider col) {
		Adventurer adventurer = col.gameObject.GetComponent<Adventurer>();
		adventurer.TakeDamage(damage);
		if (destroyed) {
			Destroy(gameObject);
		}
	}

	public void TriggerAffects(Collider col) {
		Enemy enemy;
		if (aoe) {
			Debug.Log("AoE Affect Triggered!");
			aoePrefab.SetActive(true);
			aoeStartTime = time;
			aoeTriggered = true;
			Collider[] hitColliders = Physics.OverlapSphere(transform.position, aoeRadius);
			foreach (Collider c in hitColliders) {
				if (c.gameObject.tag == "Enemy") {
					enemy = c.gameObject.GetComponent<Enemy>();
					enemy.TakeDamage(damage, actor);
					CheckForAffects(enemy);
					/*
					if (knockback) {enemy.GetKnocked(transform.position, knockbackTime, knockbackForce); }
					if (stun) { enemy.GetStunned(stunTime); }
					if (slow) { enemy.GetSlowed(slowTime, slowForce); }
					*/
				}
			}
		} else if (col.gameObject.tag == "Enemy") {
			enemy = col.gameObject.GetComponent<Enemy>();
			enemy.TakeDamage(damage, actor);
			CheckForAffects(enemy);
			/*
			if (knockback) { enemy.GetKnocked(transform.position, knockbackTime, knockbackForce); }
			if (stun) { enemy.GetStunned(stunTime); }
			if (slow) { enemy.GetSlowed(slowTime, slowForce); }
			*/
		}
		if (destroyed && !aoe) {
			Destroy(gameObject);
		}
	}

	private void CheckForAffects(Enemy enemy) {
		if (knockback) {
			enemy.GetKnocked(transform.position, knockbackTime, knockbackForce);
		}
		if (stun) {
			enemy.GetStunned(stunTime);
		}
		if (slow) {
			enemy.GetSlowed(slowTime, slowForce);
		}
	}

	private void DeleteBullet() {
		Debug.Log("Deleting Bullet!");
		Destroy(gameObject.GetComponent<CapsuleCollider>());
		Destroy(gameObject.GetComponent<Rigidbody>());
		Destroy(bulletPrefab);
		destroyed = true;
		stop = true;
	}

	private void setAoe() {
		SphereCollider AoE = gameObject.AddComponent<SphereCollider>();
		AoE.isTrigger = true;
		AoE.radius = aoeRadius;
		time = 0;
	}

	private void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.position, aoeRadius);
	}
}
