/* 
 * Copyright (c) Iron Monk Studios 2022
 * www.ironmonkstudios.co.uk 
 */

//using System; - Was causing conflict for random.range
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {
	[SerializeField] EnemyData enemyData;
	[SerializeField] EnemyClass enemyClass;
	[SerializeField] NavMeshAgent navMeshAgent;
	[SerializeField] float speed;
	float lastHit;

	[SerializeField] int maxHealth;
	[SerializeField] int currentHealth;
	[SerializeField] int damage;
	[SerializeField] float attackRadius;
	[SerializeField] float attackRange;
	[SerializeField] float cooldown;
	[SerializeField] bool onCooldown;
	[SerializeField] float nextAttackTime;
	[SerializeField] bool ranged;
	[SerializeField] GameObject bulletPrefab;
	[SerializeField] float thrust;
	[SerializeField] bool skirmisher;
	[SerializeField] bool skirmishing;
	[SerializeField] int goldValue;

	[SerializeField] bool followAdventurers = true;
	[SerializeField] GameObject target;
	[SerializeField] float distanceToTarget;
	[SerializeField] float attackDistance;
	[SerializeField] Vector3 skirmishPosition;
	[SerializeField] int skirmishDistance;

	[SerializeField] GameObject modelHolder;
	[SerializeField] GameObject model;
	[SerializeField] Material originalMaterial;
	[SerializeField] Material modelMaterial;
	[SerializeField] Color currentColor;
	[SerializeField] float flashTime;

	[SerializeField] AudioSource audioSource;
	[SerializeField] AudioClip currentClip;
	[SerializeField] List<AudioClip> audioClips;
	[SerializeField] float audioPlayTime;

	[SerializeField] Animator anim;

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

	[SerializeField] SkillData[] bossAttacks;

	public enum EnemyClass { Minion, Boss }
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

	[Header("Gizmo Debug")]
	[SerializeField] Vector3 translation;
	[SerializeField] Vector3 eulerAngles;
	[SerializeField] Vector3 scale = new Vector3(1, 1, 1);

	MeshFilter mf;
	Vector3[] origVerts;
	Vector3[] newVerts;
	Matrix4x4 m;

	private void Awake() {
		//Debug
		//mf = GetComponent<MeshFilter>();
		//origVerts = mf.mesh.vertices;
		//newVerts = new Vector3[origVerts.Length];
		//Debug
		//anim = gameObject.GetComponent<Animator>();
		SetData();
		if (skirmisher) {
			skirmishPosition = new Vector3(0, 1, 0);
		}
		navMeshAgent.updateUpAxis = true;
		navMeshAgent.updateRotation = true;
	}

	private void SetData() {
		enemyClass = enemyData._EnemyClass;
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
		skirmishDistance = enemyData.SkirmishDistance;
		attackRadius = enemyData.AttackRadius;
		attackRange = enemyData.AttackRange;
		attackDistance = attackRange;
		originalMaterial = enemyData._Material;
		goldValue = enemyData.GoldValue;

		if (model != null) {
			Destroy(model);
		}
		int i = Random.Range(0, enemyData.Models.Length);
		model = Instantiate(enemyData.Models[i], transform.position, modelHolder.transform.rotation);
		model.transform.parent = modelHolder.transform;
		model.transform.localPosition = new Vector3(0, 0, 0);
		modelMaterial = model.GetComponentInChildren<MeshRenderer>().material;

		audioClips.Clear();
		audioClips.Capacity = enemyData._AudioClips.Length;
		foreach(AudioClip clip in enemyData._AudioClips) {
			audioClips.Add(clip);
		}
			PlayAudio(audioClips[0]);
		audioPlayTime = GameManager.instance._Time + Random.Range(5, 20);

		navMeshAgent.stoppingDistance = attackDistance - 0.5f;

		if(enemyData._EnemyClass == EnemyClass.Boss) {
			bossAttacks = new SkillData[enemyData.BossAttacks.Length];
			foreach(SkillData attack in enemyData.BossAttacks) {
				bossAttacks[System.Array.IndexOf(enemyData.BossAttacks, attack)] = attack;
			}
		}
	}

	private void Update() {
		//Debug
		Quaternion rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
		m = Matrix4x4.TRS(translation, rotation, scale);
		//Debug
		float step = speed * Time.deltaTime;
		if (skirmishing) {
			Skirmish(step);
		} else if (followAdventurers && !knocked) {
			Follow(step);
		}
		if (GameManager.instance._Time >= nextAttackTime) {
			onCooldown = false;
		}
		if (audioPlayTime <= GameManager.instance._Time && !audioSource.isPlaying) {
			PlayAudio(audioClips[2]);
			audioPlayTime = GameManager.instance._Time + Random.Range(5, 20);
		}
		CheckForStatuses();
	}

	private void FixedUpdate() { }

	private void CheckForStatuses() {
		if (knocked) {
			followAdventurers = false;
			transform.Translate(knockback * (Time.deltaTime * kbForce), Space.World);
			knockedTime += Time.deltaTime;
			if (knockedTime >= kbTime) {
				knocked = false;
				followAdventurers = true;
				knockedTime = 0;
			}
		}
		if (stunned) {
			followAdventurers = false;
			navMeshAgent.SetDestination(transform.position);
			stunnedTime += Time.deltaTime;
			if (stunnedTime >= stunTime) {
				stunned = false;
				followAdventurers = true;
				stunnedTime = 0;
			}
		}
		if (slowed) {
			slowedTime += Time.deltaTime;
			if (slowedTime >= slowTime) {
				slowed = false;
				speed += slowForce;
				slowedTime = 0;
			}
		}
	}

	private void Skirmish(float step) {
		if (transform.position != skirmishPosition) {
			transform.position = Vector3.MoveTowards(transform.position, skirmishPosition, step * 2);
		}
		if (transform.position == skirmishPosition) {
			skirmishing = false;
		}
	}

	private void PlayAudio(AudioClip clip) {
		if (currentClip != clip) {
			currentClip = clip;
			audioSource.PlayOneShot(currentClip);
		} else {
			if (!audioSource.isPlaying) {
				audioSource.PlayOneShot(currentClip);
			}
		}
	}

	private void Follow(float step) {
		navMeshAgent.speed = speed;
		target = GetClosestEnemy(GameManager.instance.Adventurers).gameObject;
		navMeshAgent.SetDestination(target.transform.position);
		distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
		transform.LookAt(GetClosestEnemy(GameManager.instance.Adventurers));
		if (navMeshAgent.remainingDistance <= attackDistance && !navMeshAgent.pathPending) {
			Attack();
		}
	}
	private void Attack() {
		if (!onCooldown) {
			if (enemyClass == EnemyClass.Minion) {
				PlayAudio(audioClips[1]);
				if (ranged) {
					Vector3 spawnPoint = transform.position + (transform.forward);
					Bullet bullet = Instantiate(bulletPrefab, spawnPoint, Quaternion.Euler(0, transform.eulerAngles.y, 0)).GetComponent<Bullet>();
					bullet.Actor = gameObject;
					bullet.Damage = damage;
					bullet.Thrust = thrust;
				} else {
					Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
					Vector3 characterToCollider;
					float dot;
					float dotToDeg;
					foreach (Collider c in hitColliders) {
						characterToCollider = (c.transform.position - transform.position).normalized;
						dot = Vector3.Dot(characterToCollider, transform.forward);
						dotToDeg = Mathf.Acos(dot) * Mathf.Rad2Deg;
						if (c == target.GetComponent<Collider>()) {
							if (dot >= Mathf.Cos((attackRadius / 2) * Mathf.Deg2Rad)) {
								Adventurer adventurer = c.gameObject.GetComponent<Adventurer>();
								adventurer.TakeDamage(damage);
							} else {
							}
						}
					}
				}
			} else if (enemyClass == EnemyClass.Boss) {
				int index = Random.Range(0, bossAttacks.Length);
				Debug.Log("Boss Attacking with Attack " + index);
				bossAttacks[index].Attack(anim, transform.position, transform.rotation, transform.forward, transform.eulerAngles.y, gameObject);
			}
			onCooldown = true;
			nextAttackTime = GameManager.instance._Time + cooldown;
			if (skirmisher) {
				skirmishing = true;
				skirmishPosition = new Vector3(Random.Range(-skirmishDistance, skirmishDistance) + transform.position.x, 0, Random.Range(-skirmishDistance, skirmishDistance) + transform.position.z);
				GetSkirmishPosition();
			}
		}
	}

	private void GetSkirmishPosition() {
		Vector3 possiblePos = new Vector3(Random.Range(-skirmishDistance, skirmishDistance) + transform.position.x, 0, Random.Range(-skirmishDistance, skirmishDistance) + transform.position.z);
		NavMeshHit hit;
		bool blocked = NavMesh.Raycast(transform.position, possiblePos, out hit, NavMesh.AllAreas);
        //Debug.DrawLine(transform.position, possiblePos, blocked ? Color.red : Color.green, 5);
		if (!blocked) {
			skirmishPosition = possiblePos;
			skirmishing = true;
		} else {
			GetSkirmishPosition();
		}
	}

	public void GetKnocked(Vector3 centre, float time, float force) {
		kbTime = time;
		knockback = gameObject.transform.position - centre;
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

	public void TakeDamage(int dam, GameObject adventurer) {
		currentHealth -= dam;
		PlayAudio(audioClips[2]);
		modelMaterial.color = Color.red;
		Invoke("ResetMaterial", flashTime);
		if (currentHealth <= 0) {
			adventurer.GetComponent<Adventurer>().KillCount++;
			adventurer.GetComponent<Adventurer>().Gold += goldValue;
			EnemyDead();
		}
	}

	//Simple function for now, designed as such for potential modular coding later
	private void EnemyDead() {
		GameManager.instance.ActiveEnemies.Remove(gameObject);
		Destroy(gameObject);
	}

	private void ResetMaterial() {
		modelMaterial.color = Color.white;
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
				if (dSqrToTarget < closestDistanceSqr) {
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
		Gizmos.DrawRay(transform.position, transform.forward * enemyData.AttackRange);
		float rayRange = enemyData.AttackRange;
		Gizmos.color = Color.red;
		float primTotalFOV = enemyData.AttackRadius;
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
		if(enemyData._EnemyClass == EnemyClass.Boss) {
			foreach(SkillData attack in enemyData.BossAttacks) {
				if(attack.OverlapType == SkillData.MeleeOverlap.Rect) {
					//Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, , scale);
					//Gizmos.matrix = rotationMatrix;
					Gizmos.color = Color.cyan;
					Gizmos.DrawWireCube(transform.position + (transform.forward * rayRange), attack.AttackHalfBox * 2);
					//Gizmos.DrawWireCube(transform.position, attack.AttackHalfBox * 2);
				}
			}
		}
	}
}
