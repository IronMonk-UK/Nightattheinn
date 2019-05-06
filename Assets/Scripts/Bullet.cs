using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	[SerializeField] GameObject adventurer;
	Adventurer adventurerClass;

	[SerializeField] float thrust;
	float time;

	int damage;
	int enemiesHit;


	bool aoe;
	//int aoeDamage;
	float aoeRadius;
	//float aoeTime;
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

	bool destroyed = false;
	bool stop;
	
	Vector3 direction;
	Vector3 forward;

	public float Thrust { get { return thrust; } set { thrust = value; } }

	public GameObject Adventurer { get { return adventurer; } set { adventurer = value; } }
	public Vector3 Direction { get { return direction; } set { direction = value; } }
	public int Damage { get { return damage; } set { damage = value; } }
	public int EnemiesHit { get { return enemiesHit; } set { enemiesHit = value; } }

	public bool Aoe { get { return aoe; } set { aoe = value; } }
	//public int AoeDamage { get { return aoeDamage; } set { aoeDamage = value; } }
	public float AoeRadius { get { return aoeRadius; } set { aoeRadius = value; } }
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
		adventurerClass = adventurer.GetComponent<Adventurer>();
		forward = gameObject.transform.position - adventurer.transform.position;
	}

	void Update() {
		time += Time.deltaTime;
		if (!stop) {
			transform.Translate(forward * (Time.deltaTime * thrust), Space.World);
		}
	}

	private void OnTriggerEnter(Collider col) {
		if(col.gameObject.tag == "Enemy") {
			if(!pierce || (pierce && pierceCount == pierceAmount)) {
				Debug.Log("Trigger A");
				DeleteBullet();
				TriggerEffects(col);
			}else if(pierce && pierceCount <= PierceAmount) {
				Debug.Log("Trigger B. Pierce Count: " + pierceCount);
				TriggerEffects(col);
				pierceCount++;
			}
		}
		if(col.gameObject.tag == "Wall" && adventurerClass._AdventurerClass == global::Adventurer.AdventurerClass.Ranger) {
		Debug.Log("A bullet has hit a wall");
			Destroy(gameObject);
		}
	}

	public void TriggerEffects(Collider col) {
		Enemy enemy = col.gameObject.GetComponent<Enemy>();
		if (aoe) {
			Debug.Log("An AoE attack has triggered!");
			Collider[] hitColliders = Physics.OverlapSphere(transform.position, aoeRadius);
			foreach (Collider c in hitColliders) {
				if (c.gameObject.tag == "Enemy") {
					Debug.Log("An AoE attack has hit an enemy!" + stunTime);
					enemy = c.gameObject.GetComponent<Enemy>();
					enemy.Health -= damage;
					enemy.CheckIfDead();
					if(knockback) { enemy.GetKnocked(transform.position, knockbackTime, knockbackForce); }
					if(stun) { enemy.GetStunned(stunTime); }
					if (slow) { enemy.GetSlowed(slowTime, slowForce); }
				}
			}
		} else {
			Debug.Log("A non-AoE attack has hit an enemy!");
			enemy.Health -= damage;
			enemy.CheckIfDead();
			if(knockback) { enemy.GetKnocked(transform.position, knockbackTime, knockbackForce); }
			if(stun) { enemy.GetStunned(stunTime); }
			if (slow) { enemy.GetSlowed(slowTime, slowForce); }
		}
		if (destroyed) {
			Destroy(gameObject);
		}
	}

		/*if(adventurerClass._AdventurerClass == global::Adventurer.AdventurerClass.Mage) {
			if(!destroyed) {
				Debug.Log("Destroyed");
				//destroyed = true;
				setAoe();
			}
		}*/


	private void DeleteBullet() {
		Destroy(gameObject.GetComponent<CapsuleCollider>());
		Destroy(gameObject.GetComponent<MeshRenderer>());
		Destroy(gameObject.GetComponent<Rigidbody>());
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
