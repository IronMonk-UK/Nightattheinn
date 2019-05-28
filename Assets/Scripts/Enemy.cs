//using System; - Was causing conflict for random.range
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	[SerializeField] EnemyData enemyData;
	[SerializeField] float speed;
	float lastHit;

	[SerializeField] int maxHealth;
	[SerializeField] int currentHealth;
	[SerializeField] int damage;
	[SerializeField] float attackDegrees;
	[SerializeField] float attackRadius;
	[SerializeField] float cooldown;
	[SerializeField] bool onCooldown;
	[SerializeField] float nextAttackTime;
	[SerializeField] bool ranged;
	[SerializeField] GameObject bulletPrefab;
	[SerializeField] float thrust;
	[SerializeField] bool skirmisher;
	bool skirmishing;

	[SerializeField] bool followAdventurers = true;
	[SerializeField] GameObject target;
	[SerializeField] float distanceToTarget;
	[SerializeField] float attackDistance;

	[SerializeField] GameObject modelHolder;
	[SerializeField] GameObject model;

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

	public int Health { get { return currentHealth; } set { currentHealth = value; } }
	public bool FollowAdventurers { get { return followAdventurers; } set { followAdventurers = value; } }
	public EnemyData _EnemyData {
		get {
			return enemyData;
		} set {
			if (enemyData == value) return;
			enemyData = value;
			SetData();
		}
	}

	void Awake() {
		SetData();
	}

	private void SetData() {
		ranged = enemyData.Ranged;
		damage = enemyData.Damage;
		speed = enemyData.Speed;
		maxHealth = enemyData.Health;
		currentHealth = maxHealth;
		cooldown = enemyData.Cooldown;
		thrust = enemyData.Thrust;
		bulletPrefab = enemyData.BulletPrefab;
		//gameObject.GetComponent<Renderer>().material = enemyData.Colour;
		skirmisher = enemyData.Skirmisher;
		attackDegrees = enemyData.AttackDegrees;
		attackRadius = enemyData.AttackRadius;
		attackDistance = attackRadius;

		if(model != null) {
			Destroy(model);
		}
		model = Instantiate(enemyData.Model, transform.position, modelHolder.transform.rotation);
		model.transform.parent = modelHolder.transform;
		model.transform.localPosition = new Vector3(0, 0, 0);
	}

	void FixedUpdate() {
		float step = speed * Time.deltaTime;
		if (followAdventurers && !knocked) {
			Follow(step);
		}
		if(GameManager.instance._Time >= nextAttackTime) {
			onCooldown = false;
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
		target = GetClosestEnemy(GameManager.instance.Adventurers).gameObject;
		distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
		transform.LookAt(GetClosestEnemy(GameManager.instance.Adventurers));
		if (skirmishing) {
			Vector3 skirmishPosition = new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
			skirmishPosition = transform.position += skirmishPosition;
			transform.Translate(skirmishPosition * step, Space.World);
			//transform.position = Vector3.MoveTowards(transform.position, skirmishPosition, step);
			//if(transform.position == skirmishPosition)
				skirmishing = false;
		} else if(distanceToTarget >= attackDistance) {
			transform.position = Vector3.MoveTowards(transform.position, GetClosestEnemy(GameManager.instance.Adventurers).position, step);
		} else {
			Attack();
		}	
	}

	private void Attack() {
		if (!onCooldown) {
			if (ranged) {
				Vector3 spawnPoint = transform.position + (transform.forward);
				Bullet bullet = Instantiate(bulletPrefab, spawnPoint, Quaternion.Euler(0, transform.eulerAngles.y, 0)).GetComponent<Bullet>();
				bullet.Actor = gameObject;
				bullet.Damage = damage;
				bullet.Thrust = thrust;
			} else {
				Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius);
				Vector3 characterToCollider;
				float dot;
				float dotToDeg;
				foreach (Collider c in hitColliders) {
					characterToCollider = (c.transform.position - transform.position).normalized;
					dot = Vector3.Dot (characterToCollider, transform.forward);
					dotToDeg = Mathf.Acos(dot) * Mathf.Rad2Deg;
					if (c == target.GetComponent<Collider>()) {
						if (dot >= Mathf.Cos((attackDegrees / 2) * Mathf.Deg2Rad)) {
							Adventurer adventurer = c.gameObject.GetComponent<Adventurer>();
							adventurer.TakeDamage(damage);
						} else {
						}
					}
				}
			}
			onCooldown = true;
			nextAttackTime = GameManager.instance._Time + cooldown;
			if (skirmisher) {
				skirmishing = true;
			}
		}
	}

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

	public void TakeDamage(int dam) {
		currentHealth -= dam;
		if(currentHealth <= 0) {
			ZombieDead();
		}
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
		//Gizmos.DrawRay(transform.position, knockback);
		Gizmos.DrawRay(transform.position, transform.forward * attackRadius);
		float rayRange = attackRadius;
		Gizmos.color = Color.red;
		float primTotalFOV = attackDegrees;
		float primHalfFOV = primTotalFOV / 2;
		float primTheta = 0;
		float primX = rayRange * Mathf.Cos(primTheta);
		float primY = rayRange * Mathf.Sin(primTheta);
		Vector3 primPos = transform.position + new Vector3(primX, 0, primY);
		Vector3 primNewPos = primPos;
		Vector3 primLastPos = primPos;
		for (primTheta = 0.1f; primTheta < Mathf.PI * 2; primTheta += 0.1f) {
			primX = rayRange * Mathf.Cos(primTheta);
			primY = rayRange * Mathf.Sin(primTheta);
			primNewPos = transform.position + new Vector3(primX, 0, primY);
			Gizmos.DrawLine(primPos, primNewPos);
			primPos = primNewPos;
		}
		Gizmos.DrawLine(primPos, primLastPos);
		Gizmos.color = Color.yellow;
		//Draw primary left radian
		Vector3 primLeftRayDirection;
		Quaternion primLeftRayRotation = Quaternion.AngleAxis(-primHalfFOV, Vector3.up);
		primLeftRayDirection = primLeftRayRotation * (transform.forward);
		Gizmos.DrawRay(transform.position, primLeftRayDirection * rayRange);
		float primLeftDot = Vector3.Dot(primLeftRayDirection, transform.forward);
		primLeftDot = Mathf.Acos(primLeftDot) * Mathf.Rad2Deg;
		//Draw primary right radian
		Vector3 primRightRayDirection;
		Quaternion primRightRayRotation = Quaternion.AngleAxis(primHalfFOV, Vector3.up);
		primRightRayDirection = primRightRayRotation * (transform.forward); ;
		Gizmos.DrawRay(transform.position, primRightRayDirection * rayRange);
		float primRightDot = Vector3.Dot(primRightRayDirection, transform.forward);
		primRightDot = Mathf.Acos(primRightDot) * Mathf.Rad2Deg;
	}
}
