using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	[SerializeField]
	GameObject adventurer;

	[SerializeField]
	float thrust;
	[SerializeField]
	float aoeTime;
	float time;

	[SerializeField]
	int aoeRadius;
	int damage;

	bool destroyed = false;
	
	[SerializeField]
	Vector3 direction;

	//Ray bulletRay;

	public GameObject Adventurer { get { return adventurer; } set { adventurer = value; } }
	public Vector3 Direction { get { return direction; } set { direction = value; } }
	public int Damage { get { return damage; } set { damage = value; } }

	void Awake() {
		direction = new Vector3(0, transform.localPosition.y, 0);
	}

	void Start () {
		GetComponent<Rigidbody>().velocity = -transform.up * thrust;
	}

	void Update () {
		time += Time.deltaTime;
		if(!destroyed && time >= 5) { DestroyBullet(); }
		else if (destroyed && time >= 1) { Destroy(gameObject); }
	}

	public void DestroyBullet() {
		if (!destroyed) {
			Debug.Log("Destroyed");
			destroyed = true;
			Destroy(gameObject.GetComponent<CapsuleCollider>());
			Destroy(gameObject.GetComponent<MeshRenderer>());
			Destroy(gameObject.GetComponent<Rigidbody>());
			SphereCollider AoE = gameObject.AddComponent<SphereCollider>();
			AoE.isTrigger = true;
			AoE.radius = aoeRadius;
			time = 0;
		}
	}

	private void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.position, aoeRadius);
	}
}
