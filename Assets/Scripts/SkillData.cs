using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill Data", menuName = "Skill Data")]
public class SkillData : ScriptableObject{
	[Header("Generic Variables")]
	[SerializeField] SkillType skillType;
	[SerializeField] Adventurer.AdventurerClass adventurerClass;
	[SerializeField] string skillName;
	[SerializeField] int damage;
	[SerializeField] float cooldown;
	///[SerializeField] bool onCooldown;
	[SerializeField] int manaCost;
	[Header("Statuses")]
	[SerializeField] bool knockback;
	[SerializeField] float knockbackForce;
	[SerializeField] float knockbackTime;
	[SerializeField] bool stun;
	[SerializeField] float stunTime;
	[SerializeField] bool slow;
	[SerializeField] float slowForce;
	[SerializeField] float slowTime;
	[Header("Ranged Variables")]
	[SerializeField] bool useCone; //If true - uses a cone attack radius, if false - uses bullets
	[SerializeField] GameObject bulletPrefab;
	[Header("Melee Variables")]
	[SerializeField] float attackDegrees;
	[SerializeField] float attackRadius;

	public string SkillName { get { return skillName; } }
	public int Damage { get { return damage; } }
	public GameObject BulletPrefab { get { return bulletPrefab; } }
	public SkillType _SkillType { get { return skillType; } }
	public Adventurer.AdventurerClass _AdventurerClass { get { return adventurerClass; } }
	public float AttackDegrees { get { return attackDegrees; } }
	public float AttackRadius { get { return attackRadius; } }
	public float Cooldown { get { return cooldown; } }
	public int ManaCost { get { return manaCost; } }
	public bool UseCone { get { return useCone; } }
	//public bool OnCooldown { set { onCooldown = value; } }

	public enum SkillType { Primary, Secondary, CC }

	public void RangedAttack(Animator anim, Vector3 centre, Quaternion rotation, Vector3 right, float eulerY, GameObject adventurer) {
		if(!useCone) {
			Vector3 spawnPoint = centre + (rotation * new Vector3(0.75f, 0, -0.75f));
			Bullet bullet = Instantiate(bulletPrefab, spawnPoint, Quaternion.Euler(90, eulerY - 45, 0)).GetComponent<Bullet>();
			bullet.Adventurer = adventurer;
			bullet.Damage = damage;
			//onCooldown = true;
		} else {
			MeleeAttack(anim, centre, rotation, right);
		}
	}

	public void MeleeAttack(Animator anim, Vector3 centre, Quaternion rotation, Vector3 right) {
		anim.SetTrigger("Attack");
		Collider[] hitColliders = Physics.OverlapSphere(centre, attackRadius);
		Vector3 characterToCollider;
		float dot;
		float dotToDeg;
		foreach(Collider c in hitColliders) {
			characterToCollider = (c.transform.position - centre).normalized;
			dot = Vector3.Dot(characterToCollider, rotation * -right);
			dotToDeg = Mathf.Acos(dot) * Mathf.Rad2Deg;
			//if(c.gameObject.tag == "Enemy" && !onCooldown) {
			if(c.gameObject.tag == "Enemy") {
				if(dot >= Mathf.Cos((attackDegrees / 2) * Mathf.Deg2Rad)) {
					Enemy enemy = c.gameObject.GetComponent<Enemy>();
					Debug.Log("Dealing " + damage + " Damage.");
					enemy.Health -= damage;
					enemy.CheckIfDead();
					if(knockback) {
						enemy.GetKnocked(centre, knockbackTime, knockbackForce);
					}
					if(stun) {
						enemy.GetStunned(stunTime);
					}
					if (slow) {
						enemy.GetSlowed(slowTime, slowForce);
					}
					//onCooldown = true;
				} else {
					Debug.Log("Target in sphere but not seen!");
				}
			}
		}
	}

}
