using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour {

	[SerializeField]
	float speed;
	float lastHit;

	[SerializeField]
	int health;
	[SerializeField]
	int damage;

	void Awake () {
		
	}
	
	void Update () {
		float step = speed * Time.deltaTime;
			transform.LookAt(GetClosestEnemy(GameManager.gm.Adventurers));
			transform.position = Vector3.MoveTowards(transform.position, GetClosestEnemy(GameManager.gm.Adventurers).position, step);
		
	}

	private void OnCollisionEnter(Collision col) {
		if(col.gameObject.tag == "Adventurer") {
			col.gameObject.GetComponent<Adventurer>().TakeDamage(damage);
			lastHit = GameManager.gm._Time;
		}
	}

	private void OnCollisionStay(Collision col) {
		if (col.gameObject.tag == "Adventurer" && GameManager.gm._Time >= lastHit + 2) {
			col.gameObject.GetComponent<Adventurer>().TakeDamage(damage);
			lastHit = GameManager.gm._Time;
		}
	}

	//Trigger to detect when a bullet makes contact
	//Destroys bullet and takes away health
	private void OnTriggerEnter(Collider col) {
		Debug.Log("Hit!");
		if(col.gameObject.tag == "AdventurerProjectile") {
			col.GetComponent<Bullet>().DestroyBullet();
			health--;
			if(health <= 0) {
				zombieDead();
			}
		}	
	}

	//Simple function for now, designed as such for potential modular coding later
	private void zombieDead() {
		Destroy(gameObject);
	}

	//Transform takes the adventurers array from GameManager and looks at each of them, determining the closest, non-downed adventurer to them using the magic of maths
	Transform GetClosestEnemy(GameObject[] adventurers) {
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
