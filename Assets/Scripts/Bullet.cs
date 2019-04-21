using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	[SerializeField]
	GameObject adventurer;
	Adventurer adventurerClass;

	[SerializeField]
	float thrust;
	[SerializeField]
	float aoeTime;
	float time;

	[SerializeField]
	int aoeRadius;
	int damage;
	int enemiesHit;

	bool destroyed = false;
	
	[SerializeField]
	Vector3 direction;

	//Ray bulletRay;

	public GameObject Adventurer { get { return adventurer; } set { adventurer = value; } }
	public Vector3 Direction { get { return direction; } set { direction = value; } }
	public int Damage { get { return damage; } set { damage = value; } }
	public int EnemiesHit { get { return enemiesHit; } set { enemiesHit = value; } }

	void Awake() {
		direction = new Vector3(0, transform.localPosition.y, 0);
	}

	void Start () {
		GetComponent<Rigidbody>().velocity = -transform.up * thrust;
		adventurerClass = adventurer.GetComponent<Adventurer>();
	}

	void Update () {
		time += Time.deltaTime;
		if(adventurerClass._AdventurerClass == global::Adventurer.AdventurerClass.Mage) {
			if(!destroyed && time >= 5) { TriggerEffects(); }
			else if (destroyed && time >= 1) { Destroy(gameObject); }
		}
	}

	private void OnTriggerEnter(Collider col) {
		Debug.Log("A bullet has hit a wall");
		if(col.gameObject.tag == "Wall" && adventurerClass._AdventurerClass == global::Adventurer.AdventurerClass.Ranger) {
			Destroy(gameObject);
		}
	}

	public void TriggerEffects() {
		if(adventurerClass._AdventurerClass == global::Adventurer.AdventurerClass.Mage) {
			if (!destroyed) {
				Debug.Log("Destroyed");
				destroyed = true;
				setAoe();
			}
		}
	}

	private void setAoe() {
		Destroy(gameObject.GetComponent<CapsuleCollider>());
		Destroy(gameObject.GetComponent<MeshRenderer>());
		Destroy(gameObject.GetComponent<Rigidbody>());
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
