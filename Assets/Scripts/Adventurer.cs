using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Adventurer : MonoBehaviour {

	[Header("Class")]
	[SerializeField] AdventurerClass adventurerClass;
	[SerializeField] string className;
	[SerializeField] GameObject weapon;

	[Header("Floats & Integers")]
	[SerializeField] float primCooldown;
	[SerializeField] float secCooldown;
	[SerializeField] float moveSpeed;
	[SerializeField] int health;
	[SerializeField] int mana;
	[SerializeField] int primaryDamage;
	[SerializeField] int secondaryDamage;

	[Header("Prefabs")]
	[SerializeField] GameObject bullet;

	[Header("Scriptable Objects")]
	[SerializeField] ClassData characterClassData;
	[SerializeField] SkillData primaryAttackData;
	[SerializeField] SkillData secondaryAttackData;

	[Header("Animation")]
	[SerializeField] Animator anim;
 
	[SerializeField] GameManager instance;

	float nextPrimAttackTime;
	float nextSecAttackTime;

	bool downed = false;

	Vector3 forward, right;
	Vector3 cameraTransform;

	public bool Downed { get { return downed; } }
	public enum AdventurerClass { Mage, Ranger, Warrior }
	public AdventurerClass _AdventurerClass { get { return adventurerClass; } }
	public Animator Anim { get { return anim; } }
	public float PrimCooldown { get { return primCooldown; } }
	public float SecCooldown { get { return secCooldown; } }

	private ClassData CharacterClassData {
		get {
			return characterClassData;
		} set {
			if (characterClassData == value) return;
			characterClassData = value;
			ChangeClass();
		}
	}

	private void Awake() {
		anim = gameObject.GetComponent<Animator>();
		SetMovementVectors();
		cameraTransform = Camera.main.transform.position;
		ChangeClass();
	}

	private void Start() {
		GameManager.instance.Adventurers.Add(gameObject);
	}

	private void FixedUpdate() {
		ChangeClassDebug();
		if (!downed) {
			Move();
			Attack();
			if(GameManager.instance._Time >= nextPrimAttackTime) {
				primaryAttackData.OnCooldown = false;
			}
			if(GameManager.instance._Time >= nextSecAttackTime) {
				secondaryAttackData.OnCooldown = false;
			}

		} else {
			Debug.Log("Adventurer is downed!");
		}
	}

	private void ChangeClassDebug() {
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			CharacterClassData = GameManager.instance.Characters[0]; //Mage
		}
		if (Input.GetKeyDown(KeyCode.Alpha2)) {
			CharacterClassData = GameManager.instance.Characters[1]; //Ranger
		}
		if (Input.GetKeyDown(KeyCode.Alpha3)) {
			CharacterClassData = GameManager.instance.Characters[2]; //Warrior
		}
	}

	public void TakeDamage(int dam) {
		health -= dam;
		if(health <= 0) {
			adventurerDowned();
		}
	}

	private void ChangeClass() {
		className = characterClassData.ClassName;
		moveSpeed = characterClassData.MovementSpeed;
		health = characterClassData.Health;
		mana = characterClassData.Mana;
		primaryAttackData = characterClassData.PrimarySkill;
		primCooldown = characterClassData.PrimarySkill.Cooldown;
		secondaryAttackData = characterClassData.SecondarySkill;
		secCooldown = characterClassData.SecondarySkill.Cooldown;
		adventurerClass = characterClassData._AdventurerClass;
	}

	private void Attack() {
		if (Input.GetButton("Fire1") && GameManager.instance._Time >= nextPrimAttackTime) {
			if(adventurerClass == AdventurerClass.Mage || adventurerClass == AdventurerClass.Ranger) {
				primaryAttackData.RangedAttack(transform.position, transform.rotation, transform.eulerAngles.y, gameObject);
				nextPrimAttackTime = GameManager.instance._Time + primCooldown;
			} else {
				anim.ResetTrigger("Attack");
				primaryAttackData.MeleeAttack(anim, transform.position, transform.rotation, right);
				nextPrimAttackTime = GameManager.instance._Time + primCooldown;
			}
		}
		if (Input.GetButton("Fire2")) {
			if(adventurerClass == AdventurerClass.Mage || adventurerClass == AdventurerClass.Ranger) {
				secondaryAttackData.RangedAttack(transform.position, transform.rotation, transform.eulerAngles.y, gameObject);
				nextSecAttackTime = GameManager.instance._Time + primCooldown;
			} else {
				anim.ResetTrigger("Attack");
				secondaryAttackData.MeleeAttack(anim, transform.position, transform.rotation, right);
				nextSecAttackTime = GameManager.instance._Time + primCooldown;
			}
		}
		if (Input.GetButton("Fire3")) {
			Debug.Log("Fire 3");
		}
	}
	private void adventurerDowned() {
		downed = true;
	}

	private void SetMovementVectors() {
		forward = Camera.main.transform.forward;
		forward = new Vector3(forward.x, 0, forward.z);
		forward = Vector3.Normalize(forward);
		right = Camera.main.transform.right;
		right = new Vector3(right.x, 0, right.z);
		right = Vector3.Normalize(right);
	}

	private void Move() {

		Vector2 posOnScreen = Camera.main.WorldToViewportPoint(transform.position);
		Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);
		float angle = AngleBetweenTwoPoints(posOnScreen, mouseOnScreen);
		transform.rotation = Quaternion.Euler(new Vector3(0, -angle, 0));
		
		Vector3 moveRight = right * moveSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
		Vector3 moveForward = forward * moveSpeed * Time.deltaTime * Input.GetAxis("Vertical");
		Vector3 cameraPos = new Vector3(cameraTransform.x + transform.position.x, cameraTransform.y, cameraTransform.z + transform.position.z);
		transform.position += moveRight;
		transform.position += moveForward;
		Camera.main.transform.position = cameraPos;
	}

	private float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
		return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
	}

	// Keeping for further development of the hit radius
	// Currently takes centre of GO, want to change to far edges
	// Will help debug later
	[SerializeField] float leftDot;
	[SerializeField] float rightDot;

	private void OnDrawGizmos() {
		if (adventurerClass == AdventurerClass.Warrior) {
			float totalFOV = primaryAttackData.AttackDegrees;
			float rayRange = primaryAttackData.AttackRadius;
			float halfFOV = totalFOV / 2;
			float theta = 0;
			float x = rayRange * Mathf.Cos(theta);
			float y = rayRange * Mathf.Sin(theta);
			//Draw line in front of adventurer
			Gizmos.color = Color.blue;
			Gizmos.DrawRay(transform.position, (transform.rotation * -right) * 2);
			//Draw primary range circle around adventurer
			Gizmos.color = Color.green;
			Vector3 pos = transform.position + new Vector3(x, 0, y);
			Vector3 newPos = pos;
			Vector3 lastPos = pos;
			for (theta = 0.1f; theta < Mathf.PI * 2; theta += 0.1f)
			{
				x = rayRange * Mathf.Cos(theta);
				y = rayRange * Mathf.Sin(theta);
				newPos = transform.position + new Vector3(x, 0, y);
				Gizmos.DrawLine(pos, newPos);
				pos = newPos;
			}
			Gizmos.DrawLine(pos, lastPos);
			// Gizmos.DrawWireSphere(transform.position, rayRange);
			//Draw primary left radian
			Vector3 leftRayDirection;
			Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
			leftRayDirection = leftRayRotation * (transform.rotation * -right);
			Gizmos.color = Color.red;
			Gizmos.DrawRay(transform.position, leftRayDirection * rayRange);
			leftDot = Vector3.Dot(leftRayDirection, transform.rotation * -right);
			leftDot = Mathf.Acos(leftDot) * Mathf.Rad2Deg;
			//Draw primary right radian
			Vector3 rightRayDirection;
			Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
			rightRayDirection = rightRayRotation * (transform.rotation * -right);
			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(transform.position, rightRayDirection * rayRange);
			rightDot = Vector3.Dot(rightRayDirection, transform.rotation * -right);
			rightDot = Mathf.Acos(rightDot) * Mathf.Rad2Deg;
		}
	}
}