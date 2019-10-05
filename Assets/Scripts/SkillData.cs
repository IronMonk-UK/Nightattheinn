using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill Data", menuName = "Skill Data")]
public class SkillData : ScriptableObject{
	[Header("-Generic Variables-")]
	[SerializeField] SkillType skillType;
	[SerializeField] Adventurer.AdventurerClass adventurerClass;
	[SerializeField] string skillName;
	[SerializeField] int damage;
	[SerializeField] float cooldown;
	[SerializeField] int manaCost;
	[SerializeField] bool continuousAttack;
	[SerializeField] int skillCost;
	[Header("Particle Affect")]
	[SerializeField] GameObject particleAffect;
	[Header("Animation")]
	[SerializeField] bool hasAnim;
	[SerializeField] string animTrigger;
	[SerializeField] GameObject trail;
	[SerializeField] AnimationClip anim;
	[Header("Audio")]
	[SerializeField] AudioClip audioClip;
	[Header("-Statuses-")]
	[Header("Knockback")]
	[SerializeField] bool knockback;
	[SerializeField] float knockbackForce;
	[SerializeField] float knockbackTime;
	[Header("Slow")]
	[SerializeField] bool slow;
	[SerializeField] float slowForce;
	[SerializeField] float slowTime;
	[Header("Stun")]
	[SerializeField] bool stun;
	[SerializeField] float stunTime;
	[Header("-Ranged Variables-")]
	[SerializeField] bool useCone; //If true - uses a cone attack radius, if false - uses bullets
	[SerializeField] float thrust;
	[SerializeField] GameObject bulletPrefab;
	[SerializeField] float bulletDuration; //If 0 - infinite
	[Header("AoE")]
	[SerializeField] bool aoe;
	[SerializeField] float aoeRadius;
	[SerializeField] GameObject aoePrefab;
	[SerializeField] bool aoeScale;
	[SerializeField] bool aoeRotate;
	[Header("Pierce")]
	[SerializeField] bool pierce;
	[SerializeField] int pierceAmount; //Will pierce x amount of enemies, stop on following enemy (x + 1)
	[Header("-Melee Variables-")]
	[SerializeField] float attackRadius;
	[SerializeField] float attackRange;
	[Header("Destroy Projectiles")]
	[SerializeField] bool destroyProjectiles;

	public string SkillName { get { return skillName; } }
	public int Damage { get { return damage; } }
	public GameObject BulletPrefab { get { return bulletPrefab; } }
	public GameObject ParticleAffect { get { return particleAffect; } }
	public SkillType _SkillType { get { return skillType; } }
	public Adventurer.AdventurerClass _AdventurerClass { get { return adventurerClass; } }
	public float AttackDegrees { get { return attackRadius; } }
	public float AttackRadius { get { return attackRange; } }
	public float Cooldown { get { return cooldown; } }
	public int ManaCost { get { return manaCost; } }
	public bool UseCone { get { return useCone; } }
	public bool HasAnim { get { return hasAnim; } }
	public string AnimTrigger { get { return animTrigger; } }
	public GameObject Trail { get { return trail; } }
	public AnimationClip Anim { get { return anim; } }
	public AudioClip _AudioClip { get { return audioClip; } }
	public int SkillCost { get { return skillCost; } }

	public enum SkillType { Primary, Secondary, Passive }
	/*  Function for an Adventurers ranged attack
		Takes a null animation standard in case a Melee attack is miscalled
		Takes centre, current rotation, and the euler Y of the adventurer, the camera right, and the adventurer GO itself
	*/
	public void RangedAttack (Animator anim, Vector3 centre, Quaternion rotation, Vector3 right, float eulerY, GameObject adventurer) {
		// Confirms it is not a cone attack
		if (!useCone) {
			// Spawns a bullet at a fixed position in front of the adventurer and assigns the adventurer attacking, damage, and forward thrust
			Vector3 spawnPoint = centre + (rotation * new Vector3(0.75f, 0.5f, -0.75f));
			Bullet bullet = Instantiate(bulletPrefab, spawnPoint, Quaternion.Euler(0, eulerY - 45, 0)).GetComponent<Bullet>();
			bullet.Actor = adventurer;
			bullet.Damage = damage;
			bullet.Thrust = thrust;
			// Checks if attack is AoE - Attaches AoE related data to the bullet
			if (aoe) {
				Vector3 aoeTransform = new Vector3(bullet.transform.position.x, 0, bullet.transform.position.z);
				bullet.Aoe = true; bullet.AoeRadius = aoeRadius;
				if (aoeScale) {
					bullet.AoeScale = true;
				}
				if (aoeRotate) {
					bullet.AoeRotate = true;
				}
				bullet.AoePrefab = Instantiate(aoePrefab, aoeTransform, bullet.transform.rotation, bullet.transform);
				bullet.AoePrefab.SetActive(false);
				bullet.AoePrefab.transform.localScale = new Vector3(0, 0, 0);
			}
			// Checks if attack has affects - If so sets them on the bullet
			if (pierce) {
				bullet.Pierce = true; bullet.PierceAmount = pierceAmount;
			}
			if (knockback) {
				bullet.Knockback = true;
			}
			if (slow) {
				bullet.Slow = true; bullet.SlowForce = slowForce; bullet.SlowTime = slowTime;
			}
			if (stun) {
				bullet.Stun = true; bullet.StunTime = stunTime;
			}
		// If the attack is a cone attack, melee attack is made instead
		} else {
			MeleeAttack(anim, centre, rotation, right, adventurer);
		}
	}
	/*  Function for an Adventurers melee attackk
		Takes the attack animation, adventurer centre, current rotation, the adventurer GO, and camera right
	*/
	public void MeleeAttack (Animator anim, Vector3 centre, Quaternion rotation, Vector3 right, GameObject adventurer) {
		// Creates a sphere with the adventurer at centre, and a radius based on attack range
		Collider[] hitColliders = Physics.OverlapSphere(centre, attackRange);
		Vector3 characterToCollider;
		float dot;
		// Checks each collider hit in the sphere
		foreach (Collider c in hitColliders) {
			// Takes the normalized distance between the target and the adventurer centre
			characterToCollider = (c.transform.position - centre).normalized;
			// Gets the Dot product between the normalized distance and the position in front of the adventurer
			dot = Vector3.Dot(characterToCollider, rotation * -right);
			/*
				If the collider belongs to an enemy, and the dot value is less than the cosine of 
				the attack radius divided by two to create the two sides of the target segment
				the enemy is attacked and effects are applied
			*/
			if (c.gameObject.tag == "Enemy") {
				if (dot >= Mathf.Cos((attackRadius / 2) * Mathf.Deg2Rad)) {
					Enemy enemy = c.gameObject.GetComponent<Enemy>();
					Debug.Log("Dealing " + damage + " Damage.");
					enemy.TakeDamage(damage, adventurer);
					if (knockback) {
						enemy.GetKnocked(centre, knockbackTime, knockbackForce);
					}
					if (stun) {
						enemy.GetStunned(stunTime);
					}
					if (slow) {
						enemy.GetSlowed(slowTime, slowForce);
					}
				}
			}
			// If the attack is capable of destroying projectiles and hits an enemy projectile, it is destroyed
			if (destroyProjectiles && c.gameObject.tag == "EnemyProjectile") {
				Destroy(c.gameObject);
			}
		}
	}
}
