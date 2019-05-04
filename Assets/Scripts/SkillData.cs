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
	[SerializeField] bool onCooldown;
	[SerializeField] int manaCost;
	[Header("Statuses")]
	[SerializeField] bool knockback;
	[SerializeField] float knockbackForce;
	[SerializeField] float knockbackTime;
	[SerializeField] bool stun;
	[SerializeField] float stunTime;
	[Header("Ranged Variables")]
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
	public bool OnCooldown { set { onCooldown = value; } }

	public enum SkillType { Primary, Secondary, CC }

	public void RangedAttack(Vector3 position, Quaternion rotation, float eulerY, GameObject adventurer) {
		Vector3 spawnPoint = position + (rotation * new Vector3(0.75f, 0, -0.75f));
		Bullet bullet = Instantiate(bulletPrefab, spawnPoint, Quaternion.Euler(90, eulerY - 45, 0)).GetComponent<Bullet>();
		bullet.Adventurer = adventurer;
		bullet.Damage = damage;
		onCooldown = true;
	}

	public void MeleeAttack(Animator anim, Vector3 centre, Quaternion rotation, Vector3 right) {
		anim.SetTrigger("Attack");
		Collider[] hitColliders = Physics.OverlapSphere(centre, attackRadius);
		Vector3 characterToCollider;
		float dot;
		float dotToDeg;
		foreach (Collider c in hitColliders) {
			characterToCollider = (c.transform.position - centre).normalized;
			dot = Vector3.Dot(characterToCollider, rotation * -right);
			dotToDeg = Mathf.Acos(dot) * Mathf.Rad2Deg;
			if(c.gameObject.tag == "Enemy" && !onCooldown) {
				if(dot >= Mathf.Cos((attackDegrees / 2) * Mathf.Deg2Rad)) {
					Enemy enemy = c.gameObject.GetComponent<Enemy>();
					Debug.Log("Dealing " + damage + " Damage.");
					enemy.Health -= damage;
					enemy.CheckIfDead();
					if (knockback) {
						enemy.GetKnocked(centre, knockbackTime, knockbackForce);
					}
					if (stun) {
						enemy.GetStunned(stunTime);
						Debug.Log("Enemy stunned!");
					}
					onCooldown = true;
				} else {
					Debug.Log("Target in sphere but not seen!");
				}
			}
		}
	}

}
