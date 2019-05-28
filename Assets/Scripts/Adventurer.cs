using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Adventurer : MonoBehaviour {

	[Header("Class")]
	[SerializeField] AdventurerClass adventurerClass;
	[SerializeField] string className;
	[SerializeField] GameObject weapon;

	[Header("Floats & Integers")]
	[SerializeField] float moveSpeed;
	[SerializeField] int maxHealth;
	[SerializeField] int currentHealth;
	[SerializeField] int maxMana;
	[SerializeField] int currentMana;

	[Header("Primary Skill")]
	[SerializeField] int primaryDamage;
	[SerializeField] float primCooldown;
	[SerializeField] bool primOnCooldown;

	[Header("Secondary Skill")]
	[SerializeField] int secondaryDamage;
	[SerializeField] float secCooldown;
	[SerializeField] bool secOnCooldown;

	[Header("Prefabs")]
	[SerializeField] GameObject bullet;

	[Header("Scriptable Objects")]
	[SerializeField] ClassData characterClassData;
	[SerializeField] SkillData primaryAttackData;
	[SerializeField] SkillData secondaryAttackData;

	[Header("Animation")]
	[SerializeField] Animator anim;

	[Header("Adventurer Model")]
	[SerializeField] GameObject modelHolder;
	[SerializeField] GameObject model;

	[Header("Particle System")]
	[SerializeField] GameObject particleParent;
	[SerializeField] ParticleSystem primParticleEffect;
	[SerializeField] bool hasPrimParticleEffect;
	[SerializeField] ParticleSystem secParticleEffect;
	[SerializeField] bool hasSecParticleEffect;

	[Header("UI")]
	[SerializeField] Text healthText;
	[SerializeField] Slider healthBar;
	[SerializeField] Image healthBarFill;
	Color maxHealthColour = Color.green;
	Color minHealthColour = Color.red;
	[SerializeField] Text manaText;
	[SerializeField] Slider manaBar;
	[SerializeField] Image manaBarFill;
	Color maxManaColour = Color.blue;
	Color minManaColour = Color.cyan;
 
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
		if(!downed) {
			Move();
			Attack();
			if(GameManager.instance._Time >= nextPrimAttackTime) {
				primOnCooldown = false;
			}
			if(GameManager.instance._Time >= nextSecAttackTime) {
				secOnCooldown = false;
			}

		} else {
			Debug.Log("Adventurer is downed!");
		}
	}

	private void ChangeClassDebug() {
		if(Input.GetKeyDown(KeyCode.Alpha1)) {
			CharacterClassData = GameManager.instance.Characters[0]; //Mage
		}
		if(Input.GetKeyDown(KeyCode.Alpha2)) {
			CharacterClassData = GameManager.instance.Characters[1]; //Ranger
		}
		if(Input.GetKeyDown(KeyCode.Alpha3)) {
			CharacterClassData = GameManager.instance.Characters[2]; //Warrior
		}
	}

	public void TakeDamage(int dam) {
		currentHealth -= dam;
		healthText.text = currentHealth + "/" + maxHealth;
		healthBar.value = currentHealth;
		if(currentHealth <= maxHealth / 2) {
			healthBarFill.color = minHealthColour;
		}
		if(currentHealth <= 0) {
			adventurerDowned();
		}
	}

	private void ChangeClass() {
		maxHealth = characterClassData.Health;
		currentHealth = maxHealth;
		healthText.text = currentHealth + "/" + maxHealth;
		healthBar.maxValue = maxHealth;
		healthBar.value = maxHealth;
		healthBarFill.color = maxHealthColour;

		maxMana = characterClassData.Mana;
		currentMana = maxMana;
		manaText.text = currentMana + "/" + maxMana;
		manaBar.maxValue = maxMana;
		manaBar.value = maxMana;
		manaBarFill.color = maxManaColour;
		className = characterClassData.ClassName;
		moveSpeed = characterClassData.MovementSpeed;
		primaryAttackData = characterClassData.PrimarySkill;
		primCooldown = characterClassData.PrimarySkill.Cooldown;
		secondaryAttackData = characterClassData.SecondarySkill;
		secCooldown = characterClassData.SecondarySkill.Cooldown;
		adventurerClass = characterClassData._AdventurerClass;

		if(primaryAttackData.ParticleEffect != null && primaryAttackData.BulletPrefab == null) {
			primParticleEffect = Instantiate(primaryAttackData.ParticleEffect, transform.position, transform.rotation).GetComponent<ParticleSystem>();
			primParticleEffect.gameObject.transform.parent = particleParent.transform;
			hasPrimParticleEffect = true;
		}else if(primaryAttackData.ParticleEffect == null) {
			if(primParticleEffect != null) {
				Destroy(primParticleEffect.gameObject);
			}
			primParticleEffect = null;
			hasPrimParticleEffect = false;
		}
		if(secondaryAttackData.ParticleEffect != null && secondaryAttackData.BulletPrefab == null) {
			secParticleEffect = Instantiate(secondaryAttackData.ParticleEffect, transform.position, transform.rotation).GetComponent<ParticleSystem>();
			secParticleEffect.gameObject.transform.parent = particleParent.transform;
			hasSecParticleEffect = true;
		}else if(secondaryAttackData.ParticleEffect == null) {
			if(secParticleEffect != null) {
				Destroy(secParticleEffect.gameObject);
			}
			secParticleEffect = null;
			hasSecParticleEffect = false;
		}

		if(model != null) {
			Destroy(model);
		}
		model = Instantiate(characterClassData.Model, transform.position, modelHolder.transform.rotation);
		model.transform.parent = modelHolder.transform;
		model.transform.localPosition = new Vector3(0, 0, 0);
	}

	private void Attack() {
		if(Input.GetButton("Fire1") && !primOnCooldown) {
			if(primaryAttackData.BulletPrefab != null) {
				primaryAttackData.RangedAttack(anim, transform.position, transform.rotation, right, transform.eulerAngles.y, gameObject);
				primOnCooldown = true;
				nextPrimAttackTime = GameManager.instance._Time + primCooldown;
			} else {
				anim.ResetTrigger("Attack");
				primaryAttackData.MeleeAttack(anim, transform.position, transform.rotation, right);
				primOnCooldown = true;
				nextPrimAttackTime = GameManager.instance._Time + primCooldown;
			}
		}
		if(Input.GetButton("Fire2") && currentMana >= secondaryAttackData.ManaCost && !secOnCooldown) {
			if(secondaryAttackData.BulletPrefab != null) {
				secondaryAttackData.RangedAttack(anim, transform.position, transform.rotation, right, transform.eulerAngles.y, gameObject);
				secOnCooldown = true;
				nextSecAttackTime = GameManager.instance._Time + secCooldown;
			} else {
				anim.ResetTrigger("Attack");
				secondaryAttackData.MeleeAttack(anim, transform.position, transform.rotation, right);
				secOnCooldown = true;
				nextSecAttackTime = GameManager.instance._Time + secCooldown;
			}
			currentMana -= secondaryAttackData.ManaCost;
			manaText.text = currentMana + "/" + maxMana;
			manaBar.value = currentMana;
			if(currentMana <= maxMana / 2) {
				manaBarFill.color = minManaColour;
			}
		}
		if(Input.GetButton("Fire3")) {
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
	[SerializeField] float primLeftDot;
	[SerializeField] float primRightDot;
	[SerializeField] float secLeftDot;
	[SerializeField] float secRightDot;

	private void OnDrawGizmos() {		
		//Draw line in front of adventurer
		Gizmos.color = Color.green;
		Gizmos.DrawRay(transform.position, (transform.rotation * -right) * 2);
		float primRayRange = 0;
		if(primaryAttackData._AdventurerClass == AdventurerClass.Warrior || primaryAttackData.UseCone) {
			primRayRange = primaryAttackData.AttackRadius;
			float primTotalFOV = primaryAttackData.AttackDegrees;
			float primHalfFOV = primTotalFOV / 2;
			float primTheta = 0;
			float primX = primRayRange * Mathf.Cos(primTheta);
			float primY = primRayRange * Mathf.Sin(primTheta);
			//Draw primary range circle around adventurer
			Gizmos.color = Color.blue;
			Vector3 primPos = transform.position + new Vector3(primX, 0, primY);
			Vector3 primNewPos = primPos;
			Vector3 primLastPos = primPos;
			for (primTheta = 0.1f; primTheta < Mathf.PI * 2; primTheta += 0.1f) {
				primX = primRayRange * Mathf.Cos(primTheta);
				primY = primRayRange * Mathf.Sin(primTheta);
				primNewPos = transform.position + new Vector3(primX, 0, primY);
				Gizmos.DrawLine(primPos, primNewPos);
				primPos = primNewPos;
			}
			Gizmos.DrawLine(primPos, primLastPos);
			// Gizmos.DrawWireSphere(transform.position, rayRange);
			//Draw primary left radian
			Vector3 primLeftRayDirection;
			Quaternion primLeftRayRotation = Quaternion.AngleAxis(-primHalfFOV, Vector3.up);
			primLeftRayDirection = primLeftRayRotation * (transform.rotation * -right);
			Gizmos.color = Color.red;
			Gizmos.DrawRay(transform.position, primLeftRayDirection * primRayRange);
			primLeftDot = Vector3.Dot(primLeftRayDirection, transform.rotation * -right);
			primLeftDot = Mathf.Acos(primLeftDot) * Mathf.Rad2Deg;
			//Draw primary right radian
			Vector3 primRightRayDirection;
			Quaternion primRightRayRotation = Quaternion.AngleAxis(primHalfFOV, Vector3.up);
			primRightRayDirection = primRightRayRotation * (transform.rotation * -right); ;
			Gizmos.DrawRay(transform.position, primRightRayDirection * primRayRange);
			primRightDot = Vector3.Dot(primRightRayDirection, transform.rotation * -right);
			primRightDot = Mathf.Acos(primRightDot) * Mathf.Rad2Deg;
		}
		if (secondaryAttackData._AdventurerClass == AdventurerClass.Warrior || secondaryAttackData.UseCone) {
			float secTotalFOV = secondaryAttackData.AttackDegrees;
			float secRayRange = secondaryAttackData.AttackRadius;
			float secHalfFOV = secTotalFOV / 2;
			float secTheta = 0;
			float secX = secRayRange * Mathf.Cos(secTheta);
			float secY = secRayRange * Mathf.Sin(secTheta);
			//Draw secondary range circle around adventurer if it is not equal to the primary range circle
			if(secRayRange != primRayRange) {
				Gizmos.color = Color.cyan;
				Vector3 secPos = transform.position + new Vector3(secX, 0, secY);
				Vector3 secNewPos = secPos;
				Vector3 secLastPos = secPos;
				for (secTheta = 0.1f; secTheta < Mathf.PI * 2; secTheta += 0.1f)
				{
					secX = secRayRange * Mathf.Cos(secTheta);
					secY = secRayRange * Mathf.Sin(secTheta);
					secNewPos = transform.position + new Vector3(secX, 0, secY);
					Gizmos.DrawLine(secPos, secNewPos);
					secPos = secNewPos;
				}
				Gizmos.DrawLine(secPos, secLastPos);
			}
			// Gizmos.DrawWireSphere(transform.position, rayRange);
			//Draw secondary left radian
			Vector3 leftRayDirection;
			Quaternion leftRayRotation = Quaternion.AngleAxis(-secHalfFOV, Vector3.up);
			leftRayDirection = leftRayRotation * (transform.rotation * -right);
			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(transform.position, leftRayDirection * secRayRange);
			secLeftDot = Vector3.Dot(leftRayDirection, transform.rotation * -right);
			secLeftDot = Mathf.Acos(secLeftDot) * Mathf.Rad2Deg;
			//Draw secondary right radian
			Vector3 rightRayDirection;
			Quaternion rightRayRotation = Quaternion.AngleAxis(secHalfFOV, Vector3.up);
			rightRayDirection = rightRayRotation * (transform.rotation * -right);
			Gizmos.DrawRay(transform.position, rightRayDirection * secRayRange);
			secRightDot = Vector3.Dot(rightRayDirection, transform.rotation * -right);
			secRightDot = Mathf.Acos(secRightDot) * Mathf.Rad2Deg;
		}	
	}
}