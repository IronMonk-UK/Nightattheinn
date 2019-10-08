﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class Adventurer : MonoBehaviour {

	[Header("Class")]
	[SerializeField] AdventurerClass adventurerClass;
	[SerializeField] string className;
	[SerializeField] GameObject weapon;
	[SerializeField] bool joyStick;
	[SerializeField] InputData inputData;

	[Header("Floats & Integers")]
	[SerializeField] int playerNumber;
	[SerializeField] float moveSpeed;
	[SerializeField] int maxHealth;
	[SerializeField] int currentHealth;
	[SerializeField] int maxMana;
	[SerializeField] int currentMana;
	[SerializeField] int manaRegen;
	[SerializeField] int manaDelay;
	[SerializeField] int killCount;
	[SerializeField] int flashTime;
	[SerializeField] int gold;

	[Header("Primary Skill")]
	[SerializeField] int primaryDamage;
	[SerializeField] float primCooldown;
	[SerializeField] bool primOnCooldown;
	[SerializeField] string primAnimTrigger;
	[SerializeField] GameObject primTrail;
	[SerializeField] AnimationClip primAnim;
	[SerializeField] bool primIgnoreAudio;

	[Header("Secondary Skill 01")]
	[SerializeField] int secondary01Damage;
	[SerializeField] float sec01Cooldown;
	[SerializeField] bool sec01OnCooldown;
	[SerializeField] string sec01AnimTrigger;
	[SerializeField] GameObject sec01Trail;
	[SerializeField] AnimationClip sec01Anim;
	[SerializeField] int sec01ManaCost;
	[SerializeField] bool sec01IgnoreAudio;
	
	[Header("Secondary Skill 02")]
	[SerializeField] int secondary02Damage;
	[SerializeField] float sec02Cooldown;
	[SerializeField] bool sec02OnCooldown;
	[SerializeField] string sec02AnimTrigger;
	[SerializeField] GameObject sec02Trail;
	[SerializeField] AnimationClip sec02Anim;
	[SerializeField] int sec02ManaCost;
	[SerializeField] bool sec02IgnoreAudio;

	[Header("Audio")]
	[SerializeField] AudioSource audioSource;
	[SerializeField] AudioClip currentClip;
	[SerializeField] AudioClip primAudio;
	[SerializeField] AudioClip sec01Audio;

	[Header("Prefabs")]
	[SerializeField] GameObject bullet;

	[Header("Scriptable Objects")]
	[SerializeField] ClassData characterClassData;
	[SerializeField] SkillData primaryAttackData;
	[SerializeField] SkillData secondaryAttack01Data;
	[SerializeField] SkillData secondaryAttack02Data;

	[Header("Animation")]
	[SerializeField] Animator anim;
	[SerializeField] GameObject vfx;
	[SerializeField] GameObject trail;

	[Header("Adventurer Model")]
	[SerializeField] GameObject modelHolder;
	[SerializeField] GameObject model;
	[SerializeField] Material modelMaterial;

	[Header("Particle System")]
	[SerializeField] GameObject particleParent;
	[SerializeField] ParticleSystem primParticleAffect;
	[SerializeField] bool hasPrimParticleAffect;
	[SerializeField] ParticleSystem sec01ParticleAffect;
	[SerializeField] bool hasSec01ParticleAffect;

	[Header("NavMesh Agent")]
	[SerializeField] NavMeshAgent navMeshAgent;

	[Header("UI")]
	[SerializeField] PlayerUI ui;
	[SerializeField] Text healthText;
	[SerializeField] Slider healthBar;
	[SerializeField] Image healthBarFill;
	Color maxHealthColour = Color.green;
	Color minHealthColour = Color.red;
	[SerializeField] Text manaText;
	[SerializeField] Slider manaBar;
	[SerializeField] Image manaBarFill;
	[SerializeField] Text manaCostText;
	Color maxManaColour = Color.blue;
	Color minManaColour = Color.cyan;
	[SerializeField] Text primSkillText;
	[SerializeField] Slider primSkillBar;
	[SerializeField] Image primSkillFill;
	[SerializeField] Text sec01SkillText;
	[SerializeField] Slider sec01SkillBar;
	[SerializeField] Image sec01SkillFill;
	[SerializeField] Text killCountText;
	[SerializeField] Text goldCountText;

	// Keeping for further development of the hit radius
	// Currently takes centre of GO, want to change to far edges
	// Will help debug later
	[Header("Debug Variables")]
	[SerializeField] float primLeftDot;
	[SerializeField] float primRightDot;
	[SerializeField] float secLeftDot;
	[SerializeField] float secRightDot;
 
	//[SerializeField] GameManager instance;

	float nextPrimAttackTime;
	float nextSecAttack01Time;

	bool downed = false;

	Vector3 forward, right;
	Vector3 cameraTransform;
	public int PlayerNumber { get { return playerNumber; } set { playerNumber = value; } }
	public bool Downed { get { return downed; } }
	public enum AdventurerClass { Mage, Ranger, Fighter }
	public AdventurerClass _AdventurerClass { get { return adventurerClass; } }
	public bool Joystick { get { return joyStick; } set { joyStick = value; } }
	public InputData InputData { get { return inputData; } set { inputData = value; } }
	public Animator Anim { get { return anim; } }
	public float PrimCooldown { get { return primCooldown; } }
	public float Sec01Cooldown { get { return sec01Cooldown; } }
	public Text HealthText { get { return healthText; } set { healthText = value; } }
	public Slider HealthBar { get { return healthBar; } set { healthBar = value; } }
	public Image HealthBarFill { get { return healthBarFill; } set { healthBarFill = value; } }
	public Text ManaText { get { return manaText; } set { manaText = value; } }
	public Slider ManaBar { get { return manaBar; } set { manaBar = value; } }
	public Image ManaBarFill { get { return manaBarFill; } set { manaBarFill = value; } }
	public SkillData PrimaryAttackData {
		get {
			return primaryAttackData;
		} set {
			primaryAttackData = value;
		}
	}
	public SkillData SecondaryAttack01Data {
		get {
			return secondaryAttack01Data;
		} set {
			secondaryAttack01Data = value;
		}
	}
	public SkillData SecondaryAttack02Data {
		get {
			return secondaryAttack02Data;
		} set {
			secondaryAttack02Data = value;
		}
	}

	public ClassData CharacterClassData {
		get {
			return characterClassData;
		} set {
			if (characterClassData == value) return;
			characterClassData = value;
			ChangeClass();
		}
	}

	public PlayerUI UI {
		get {
			return ui;
		} set {
			if (ui == value) return;
			ui = value;
			SetUI();
		}
	}

	public int Gold {
		get {
			return gold;
		} set {
			if (gold == value) return;
			gold = value;
			goldCountText.text = "Gold: " + gold;
		}
	}

	public int KillCount {
		get {
			return killCount;
		} set {
			if (killCount == value) return;
			killCount = value;
			killCountText.text = "Kill Count: " + killCount;
		}
	}

	private void Awake() {
		anim = gameObject.GetComponent<Animator>();
		SetMovementVectors();
		cameraTransform = Camera.main.transform.position;
		navMeshAgent.avoidancePriority = 99;
	}

	private void Start() {
		GameManager.instance.Adventurers.Add(gameObject);
	}

	private void Update() {
		DebugTools();
		if (!downed) {
			Move();
			Attack();
			RefreshMana();
			UpdateUI();
			if (GameManager.instance._Time >= nextPrimAttackTime) {
				primOnCooldown = false;
				primSkillBar.value = primSkillBar.maxValue;
			} else if (primSkillBar.value >= 0) {
				primSkillBar.value -= Time.deltaTime;
			}
			if (GameManager.instance._Time >= nextSecAttack01Time) {
				sec01OnCooldown = false;
				sec01SkillBar.value = sec01SkillBar.maxValue;
			} else if (sec01SkillBar.value >= 0) {
				sec01SkillBar.value -= Time.deltaTime;
			}
			if (hasSec01ParticleAffect && (((joyStick && Input.GetAxis(inputData.Fire2) == 0 && sec01ParticleAffect.gameObject.activeInHierarchy)) || (!joyStick && Input.GetButtonUp(inputData.Fire2)))) { 
				Debug.Log("Disabling GO");
				sec01ParticleAffect.gameObject.SetActive(false);
				audioSource.Stop();
			}
		} else {
			Debug.Log("Adventurer is downed!");
		}
	}

	private void RefreshMana() {
		if (currentMana < maxMana) {
			if (manaDelay == 0) { manaDelay = Mathf.RoundToInt(GameManager.instance._Time + 1); }
			if (manaDelay < GameManager.instance._Time) {
				currentMana += manaRegen;
				manaDelay = Mathf.RoundToInt(GameManager.instance._Time + 1);
			}
		}
	}

	private void UpdateUI() {
		healthText.text = currentHealth + "/" + maxHealth;
		healthBar.value = currentHealth;
		if (currentHealth <= maxHealth / 2) {
			healthBarFill.color = minHealthColour;
		}
		manaText.text  = currentMana + "/" + maxMana;
		manaBar.value = currentMana;
		if (currentMana <= maxMana / 2) {
			manaBarFill.color = minManaColour;
		}
	}

	private void DebugTools() {
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			CharacterClassData = GameManager.instance.Characters[0]; //Fighter
		}
		if (Input.GetKeyDown(KeyCode.Alpha2)) {
			CharacterClassData = GameManager.instance.Characters[1]; //Mage
		}
		if (Input.GetKeyDown(KeyCode.Alpha3)) {
			CharacterClassData = GameManager.instance.Characters[2]; //Ranger
		}
		if (Input.GetKeyDown(KeyCode.K)) {
			adventurerDowned();
		}
	}

	public void TakeDamage(int dam) {
		currentHealth -= dam;
		modelMaterial.color = Color.red;
		Invoke("ResetMaterial", flashTime);
		if (currentHealth <= 0) {
			adventurerDowned();
		}
	}

	private void ChangeClass() {
		maxHealth = characterClassData.Health;
		currentHealth = maxHealth;
		maxMana = characterClassData.Mana;
		currentMana = maxMana;
		manaRegen = characterClassData.ManaRegen;
		
		className = characterClassData.ClassName;
		moveSpeed = characterClassData.MovementSpeed;
		primaryAttackData = characterClassData.PrimarySkill;
		primCooldown = characterClassData.PrimarySkill.Cooldown;
		secondaryAttack01Data = characterClassData.SecondarySkill;
		sec01Cooldown = characterClassData.SecondarySkill.Cooldown;
		adventurerClass = characterClassData._AdventurerClass;

		if (primaryAttackData.AnimTrigger != null) {
			primAnimTrigger = primaryAttackData.AnimTrigger;
			primTrail = primaryAttackData.Trail;
			primAnim = primaryAttackData.Anim;
			SetAnimationEvents(primAnim, "PrimaryAttack", "PrimDestroyTrail");
		}
		if (secondaryAttack01Data.AnimTrigger != null) {
			sec01AnimTrigger = secondaryAttack01Data.AnimTrigger;
			sec01Trail = secondaryAttack01Data.Trail;
			sec01Anim = secondaryAttack01Data.Anim;
			SetAnimationEvents(sec01Anim, "SecondaryAttack", "SecDestroyTrail");
		}

		if (primaryAttackData.ParticleAffect != null && primaryAttackData.BulletPrefab == null) {
			hasPrimParticleAffect = true;
			primParticleAffect = Instantiate(primaryAttackData.ParticleAffect, transform.position, transform.rotation).GetComponent<ParticleSystem>();
			primParticleAffect.gameObject.transform.parent = particleParent.transform;
		} else if (primaryAttackData.ParticleAffect == null) {
			if (primParticleAffect != null) {
				Destroy(primParticleAffect.gameObject);
			}
			primParticleAffect = null;
			hasPrimParticleAffect = false;
		}
		if (secondaryAttack01Data.ParticleAffect != null && secondaryAttack01Data.BulletPrefab == null) {
			hasSec01ParticleAffect = true;
			sec01ParticleAffect = Instantiate(secondaryAttack01Data.ParticleAffect, transform.position, transform.rotation).GetComponent<ParticleSystem>();
			sec01ParticleAffect.gameObject.transform.parent = particleParent.transform;
		} else if (secondaryAttack01Data.ParticleAffect == null) {
			if (sec01ParticleAffect != null) {
				Destroy(sec01ParticleAffect.gameObject);
			}
			sec01ParticleAffect = null;
			hasSec01ParticleAffect = false;
		}

		sec01ManaCost = secondaryAttack01Data.ManaCost;

		primAudio = primaryAttackData._AudioClip;
		sec01Audio = secondaryAttack01Data._AudioClip;

		if (model != null) {
			Destroy(model);
		}
		model = Instantiate(characterClassData.Model, transform.position, modelHolder.transform.rotation);
		model.transform.parent = modelHolder.transform;
		model.transform.localPosition = new Vector3(0, 0, 0);
		modelMaterial = model.GetComponentInChildren<MeshRenderer>().material;
	}

	public void SetUI() {
		healthText = ui.HealthText;
		healthBar = ui.HealthSlider;
		healthBarFill = ui.HealthFill;
		healthText.text = currentHealth + "/" + maxHealth;
		healthBar.maxValue = maxHealth;
		healthBar.value = maxHealth;
		healthBarFill.color = maxHealthColour;

		manaText = ui.ManaText;
		manaBar = ui.ManaSlider;
		manaBarFill = ui.ManaFill;
		manaCostText = ui.ManaCostText;
		manaText.text = currentMana + "/" + maxMana;
		manaBar.maxValue = maxMana;
		manaBar.value = maxMana;
		manaBarFill.color = maxManaColour;
		manaCostText.text = "Mana Cost: " + sec01ManaCost;

		primSkillText = ui.Skill01Text;
		primSkillBar = ui.Skill01Slider;
		primSkillFill = ui.Skill01Fill;
		primSkillBar.maxValue = primCooldown;
		primSkillBar.value = primSkillBar.maxValue;
		primSkillText.text = primaryAttackData.SkillName;

		sec01SkillText = ui.Skill02Text;
		sec01SkillBar = ui.Skill02Slider;
		sec01SkillFill = ui.Skill02Fill;
		sec01SkillBar.maxValue = sec01Cooldown;
		sec01SkillBar.value = sec01SkillBar.maxValue;
		sec01SkillText.text = secondaryAttack01Data.SkillName;

		killCountText = ui.KillCount;
		killCountText.text = "Kill Count: " + killCount;

		goldCountText = ui.GoldCount;
		goldCountText.text = "Gold: " + gold;
	}
	
	private void Attack() {
		if (((joyStick && Input.GetAxis(inputData.Fire1) > 0) || (!joyStick && Input.GetButton(inputData.Fire1))) && !primOnCooldown) { 
			PlayAudio(primAudio, primIgnoreAudio, out primIgnoreAudio);
			if (primaryAttackData.HasAnim) {
				AddTrail(primTrail);
				anim.SetTrigger(primAnimTrigger);
			} else {
				if (hasPrimParticleAffect) {
					primParticleAffect.gameObject.SetActive(true);
					primParticleAffect.Play();
				}
				PrimaryAttack();
			}
		}
		if (((joyStick && Input.GetAxis(inputData.Fire2) > 0) || (!joyStick && Input.GetButton(inputData.Fire2))) && !sec01OnCooldown) { 
			PlayAudio(sec01Audio, sec01IgnoreAudio, out sec01IgnoreAudio);
			if (secondaryAttack01Data.HasAnim) {
				AddTrail(sec01Trail);
				anim.SetTrigger(sec01AnimTrigger);
			} else {
				if (hasSec01ParticleAffect) {
					sec01ParticleAffect.gameObject.SetActive(true);
					sec01ParticleAffect.Play();
				}
				SecondaryAttack();
			}
		}
		if (Input.GetButton(inputData.Fire3)) { 
			Debug.Log("Fire 3");
		}
		if (hasPrimParticleAffect && (((joyStick && Input.GetAxis(inputData.Fire1) == 0 && primParticleAffect.gameObject.activeInHierarchy)) || (!joyStick && Input.GetButtonUp(inputData.Fire1)))) {
			primParticleAffect.gameObject.SetActive(false);
		}

	}

	private void SetAnimationEvents(AnimationClip anim, string atkFunction, string destroyFunction) {
		if (anim && anim.events.Length == 0) {
			AnimationEvent evtAtk = new AnimationEvent();
			AnimationEvent evtTrl = new AnimationEvent();
			evtAtk.time = anim.length / 2;
			evtTrl.time = anim.length;
			evtAtk.functionName = atkFunction;
			evtTrl.functionName = destroyFunction;
			anim.AddEvent(evtAtk);
			anim.AddEvent(evtTrl);
		}
	}

	private void AddTrail(GameObject trailPrefab) {
		TrailRenderer trP = trailPrefab.GetComponent<TrailRenderer>();
		if (!trail.GetComponent<TrailRenderer>()) {
			trail.AddComponent<TrailRenderer>().GetCopyOf(trP);
		}
		TrailRenderer tr = trail.GetComponent<TrailRenderer>();
		tr.material = new Material(Shader.Find("Sprites/Default"));

		GradientAlphaKey[] gak = trP.colorGradient.alphaKeys;
		GradientColorKey[] gck = trP.colorGradient.colorKeys;

		Gradient gradient = new Gradient();
		gradient.SetKeys( gck, gak);
		tr.colorGradient = gradient;
	}

	private void PrimDestroyTrail() {
		DestroyTrail(out primIgnoreAudio);
	}
	private void SecDestroyTrail() {
		DestroyTrail(out sec01IgnoreAudio);
	}

	private void DestroyTrail(out bool ignoreAudio) {
		Destroy(trail.GetComponent<TrailRenderer>());
		ignoreAudio = false;
	}

	private void PlayAudio(AudioClip clip, bool ignoreAudio, out bool outIgnoreAudio) {
		if (currentClip != clip) {
			outIgnoreAudio = true;
			currentClip = clip;
			audioSource.PlayOneShot(currentClip);
		} else {
			if (!audioSource.isPlaying || !ignoreAudio) {
				outIgnoreAudio = true;
				audioSource.PlayOneShot(currentClip);
			}
		}
		outIgnoreAudio = true;
	}


	private void PrimaryAttack() {
		MakeAttack(primaryAttackData, primCooldown, primAnimTrigger, out primOnCooldown, out nextPrimAttackTime);
	}
	private void SecondaryAttack() {
		MakeAttack(secondaryAttack01Data, sec01Cooldown, sec01AnimTrigger, out sec01OnCooldown, out nextSecAttack01Time);
	}

	private void MakeAttack(SkillData attackData, float cooldown, string animTrigger, out bool onCooldown, out float nextAttackTime) {
		if (attackData.BulletPrefab != null) {
			attackData.RangedAttack(anim, transform.position, transform.rotation, right, transform.eulerAngles.y, gameObject);
		} else {
			attackData.MeleeAttack(anim, transform.position, transform.rotation, right, gameObject);
		}
		onCooldown = true;
		nextAttackTime = GameManager.instance._Time + cooldown;
		if (attackData == secondaryAttack01Data) {
			Debug.Log("Making Secondary Attack!");
			currentMana -= sec01ManaCost;
		}		
		if (animTrigger != "") anim.ResetTrigger(animTrigger);
	}

	private void ResetMaterial() {
		modelMaterial.color = Color.white;
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
		Vector3 screenPos = Camera.main.WorldToViewportPoint(gameObject.transform.position);
		if (screenPos.x <= 0.05 || screenPos.x >= 0.95) {
			Debug.Log("X too far");
		}
		if (screenPos.y <= 0.05 || screenPos.y >= 0.95) {
			Debug.Log("Y too far");
		}
		screenPos.x = Mathf.Clamp(screenPos.x, 0.05f, 0.95f);
		screenPos.y = Mathf.Clamp(screenPos.y, 0.05f, 0.95f);
		transform.position = Camera.main.ViewportToWorldPoint(screenPos);

		if (joyStick) {
			JoystickRotation();
		} else {
			KeyboardRotation();
		}

		Vector3 moveRight = right * moveSpeed * Time.deltaTime * Input.GetAxis(inputData.LeftHorizontal);
		Vector3 moveForward = forward * moveSpeed * Time.deltaTime * Input.GetAxis(inputData.LeftVertical);
		transform.position += moveRight;
		transform.position += moveForward;
	}

	private void JoystickRotation() {
		float rh = Input.GetAxis(inputData.RightHorizontal) * 45;
		float rv = Input.GetAxis(inputData.RightVertical) * 45;
		Vector3 direction = new Vector3(rv, 0, rh);
		transform.LookAt(transform.position + direction);
	}

	private void KeyboardRotation() {
		Vector2 posOnScreen = Camera.main.WorldToViewportPoint(transform.position);
		Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);
		float angle = AngleBetweenTwoPoints(posOnScreen, mouseOnScreen);
		transform.rotation = Quaternion.Euler(new Vector3(0, -angle, 0));
	}

	private float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
		return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
	}

	private void OnDrawGizmos() {		
		//Draw line in front of adventurer
		Gizmos.color = Color.green;
		Gizmos.DrawRay(transform.position, (transform.rotation * -Vector3.Normalize(new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z))) * 2);
		float primRayRange = 0;
		if (primaryAttackData && (primaryAttackData._AdventurerClass == AdventurerClass.Fighter || primaryAttackData.UseCone)) {
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
			primLeftRayDirection = primLeftRayRotation * (transform.rotation * -Vector3.Normalize(new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z)));
			Gizmos.color = Color.red;
			Gizmos.DrawRay(transform.position, primLeftRayDirection * primRayRange);
			primLeftDot = Vector3.Dot(primLeftRayDirection, transform.rotation * -Vector3.Normalize(new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z)));
			primLeftDot = Mathf.Acos(primLeftDot) * Mathf.Rad2Deg;
			//Draw primary right radian
			Vector3 primRightRayDirection;
			Quaternion primRightRayRotation = Quaternion.AngleAxis(primHalfFOV, Vector3.up);
			primRightRayDirection = primRightRayRotation * (transform.rotation * -Vector3.Normalize(new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z))); ;
			Gizmos.DrawRay(transform.position, primRightRayDirection * primRayRange);
			primRightDot = Vector3.Dot(primRightRayDirection, transform.rotation * -Vector3.Normalize(new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z)));
			primRightDot = Mathf.Acos(primRightDot) * Mathf.Rad2Deg;
		}
		if (secondaryAttack01Data && (secondaryAttack01Data._AdventurerClass == AdventurerClass.Fighter || secondaryAttack01Data.UseCone)) {
			float secTotalFOV = secondaryAttack01Data.AttackDegrees;
			float secRayRange = secondaryAttack01Data.AttackRadius;
			float secHalfFOV = secTotalFOV / 2;
			float secTheta = 0;
			float secX = secRayRange * Mathf.Cos(secTheta);
			float secY = secRayRange * Mathf.Sin(secTheta);
			//Draw secondary range circle around adventurer if it is not equal to the primary range circle
			if (secRayRange != primRayRange) {
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
			leftRayDirection = leftRayRotation * (transform.rotation * -Vector3.Normalize(new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z)));
			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(transform.position, leftRayDirection * secRayRange);
			secLeftDot = Vector3.Dot(leftRayDirection, transform.rotation * -Vector3.Normalize(new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z)));
			secLeftDot = Mathf.Acos(secLeftDot) * Mathf.Rad2Deg;
			//Draw secondary right radian
			Vector3 rightRayDirection;
			Quaternion rightRayRotation = Quaternion.AngleAxis(secHalfFOV, Vector3.up);
			rightRayDirection = rightRayRotation * (transform.rotation * -Vector3.Normalize(new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z)));
			Gizmos.DrawRay(transform.position, rightRayDirection * secRayRange);
			secRightDot = Vector3.Dot(rightRayDirection, transform.rotation * -Vector3.Normalize(new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z)));
			secRightDot = Mathf.Acos(secRightDot) * Mathf.Rad2Deg;
		}	
	}
}

public static class ExtensionMethods {

	public static T GetCopyOf<T>(this Component comp, T other) where T : Component {
		Type type = null;
		try {
			type = comp.GetType();
		} catch { }
		if (type != other.GetType()) return null; //type mismatch
		BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
		PropertyInfo[] pinfos = type.GetProperties(flags);
		foreach(var pinfo in pinfos) {
			if (pinfo.CanWrite) {
				try {
					pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
				} catch { }
			}
		}
		FieldInfo[] finfos = type.GetFields(flags);
		foreach (var finfo in finfos) {
			finfo.SetValue(comp, finfo.GetValue(other));
		}
		return comp as T;
	}
}