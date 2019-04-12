using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Adventurer : MonoBehaviour {

	[Header("Class")]
	[SerializeField] AdventurerClass adventurerClass;
	[SerializeField] string className;

	[Header("Floats & Integers")]
	[SerializeField] float attackSpeed;
	[SerializeField] float moveSpeed;
	[SerializeField] int health;
	[SerializeField] int mana;
	[SerializeField] int damage;

	[Header("Prefabs")]
	[SerializeField] GameObject bullet;

	[Header("Scriptable Objects")]
	[SerializeField] ClassData characterClassData;
	[SerializeField] SkillData primaryAttackData;
	[SerializeField] GameEvent primaryAttackEvent;

	[SerializeField]
	GameManager instance;

	float nextAttackTime;

	bool downed = false;

	Vector3 forward, right;
	Vector3 cameraTransform;

	public bool Downed { get { return downed; } }

	public enum AdventurerClass { Mage, Ranger, Warrior }

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
		nextAttackTime = attackSpeed;
		SetMovementVectors();
		cameraTransform = Camera.main.transform.position;
		ChangeClass();
	}

	private void Start() {
		GameManager.instance.Adventurers.Add(gameObject);
	}

	private void Update() {
		ChangeClassDebug();
		if (!downed) {
			Move();
			Attack();
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
		attackSpeed = characterClassData.AttackSpeed;
		health = characterClassData.Health;
		mana = characterClassData.Mana;
		primaryAttackData = characterClassData.PrimarySkill;
		adventurerClass = characterClassData._AdventurerClass;
	}

	private void Attack() {
		if (Input.GetButton("Fire1") && GameManager.instance._Time >= nextAttackTime) {
			primaryAttackData.Attack(transform.position, transform.rotation, transform.eulerAngles.y, gameObject);
			nextAttackTime = GameManager.instance._Time + attackSpeed;
		}
		if (Input.GetButton("Fire2")) {
			Debug.Log("Fire 2");
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
		Vector3 cameraPos = new Vector3(cameraTransform.x + transform.position.x, 17.5f, cameraTransform.z + transform.position.z);
		transform.position += moveRight;
		transform.position += moveForward;
		Camera.main.transform.position = cameraPos;
	}

	float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
		return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
	}
}